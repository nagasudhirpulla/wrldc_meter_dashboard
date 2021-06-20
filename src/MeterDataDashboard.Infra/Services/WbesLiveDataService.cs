using MeterDataDashboard.Core.ScheduleData;
using MeterDataDashboard.Core.ScheduleData.Services;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MeterDataDashboard.Infra.Services
{
    public class WbesLiveDataService : IWbesLiveDataService
    {
        private readonly string _oracleConnString;
        public WbesLiveDataService(IConfiguration configuration)
        {
            _oracleConnString = configuration["ConnectionStrings:WBESOracleConnection"];
        }

        public List<(string utilId, string utilName)> GetAllThermalIsgsUtils()
        {
            List<(string, string)> utils = new List<(string, string)>();
            using (OracleConnection con = new OracleConnection(_oracleConnString))
            {
                using OracleCommand cmd = con.CreateCommand();
                try
                {
                    // get dc data
                    con.Open();
                    cmd.BindByName = true;
                    cmd.CommandText = @"select util_id, acronym from SCHN7.utility 
                                        where is_active = 1 and region_id=2 
                                        and util_type_id=2 and isgs_type_id=1";

                    //Execute the command and use DataReader to read the data
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string genUtilId = reader.GetString(0);
                        string genName = reader.GetString(1);

                        utils.Add((genUtilId, genName));
                    }
                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return utils;
        }

        public async Task<int> GetMaxRevForDate(DateTime targetDt)
        {
            // https://wbes.wrldc.in/Report/GetCurrentDayFullScheduleMaxRev?regionid=2&ScheduleDate=20-06-2021
            string url = $"https://wbes.wrldc.in/Report/GetCurrentDayFullScheduleMaxRev?regionid=2&ScheduleDate={targetDt.ToString("dd-MM-yyyy")}";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3887.7 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            string res = await (await client.GetAsync(url)).Content.ReadAsStringAsync();
            int rev = -1;
            if (!string.IsNullOrWhiteSpace(res))
            {
                int colonStartPos = res.IndexOf(':');
                rev = Convert.ToInt32(res.Substring(colonStartPos + 1, (res.Length - colonStartPos - 2)));
            }
            return rev;
        }

        public async Task<UtilSchData> GetSellerFullSchForDate(string utilId, DateTime targetDt)
        {
            // https://stackoverflow.com/questions/32860666/httpclient-scrape-data-from-website-with-login-c-sharp
            int rev = await GetMaxRevForDate(targetDt);
            UtilSchData schData = new UtilSchData();
            if (rev == -1) { return null; }
            string url = $"https://wbes.wrldc.in/ReportFullSchedule/GetFullInjSummary?scheduleDate={targetDt.ToString("dd-MM-yyyy")}&sellerId={utilId}&revisionNumber={rev}&regionId=2&byDetails=0&isDrawer=0&isBuyer=0";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.KeepAlive = true;
            //SET AUTOMATIC DECOMPRESSION
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            using (WebResponse response = request.GetResponse())
            {
                using StreamReader sr = new StreamReader(response.GetResponseStream());
                string res = sr.ReadToEnd();
                string dataStartSearchStr = "var data = JSON.parse(";
                string dataEndSearchStr = "]]";
                int dataStartInd = res.IndexOf("var data = JSON.parse(") + dataStartSearchStr.Length;
                int dataEndInd = res.IndexOf(dataEndSearchStr, dataStartInd) - 1;
                // get the data string in the format [[...],[...],[...]]
                string dataStr = res.Substring(dataStartInd, dataEndInd - dataStartInd + 1);
                dataStr = dataStr.Replace("\\\"", "");
                // remove the first and last characters to get string as [],[],[],[]
                dataStr = dataStr.Substring(1, dataStr.Length - 2);
                // get row strings in the form "","","",""
                List<string> dataRows = dataStr.Split("],").Select(s => s[1..]).Skip(1).Take(96).ToList();
                for (int rowIter = 0; rowIter < dataRows.Count; rowIter++)
                {
                    string val = dataRows[rowIter].Split(",").Last();
                    schData.SchVals.Add(new ScheduleValue() { Timestamp = targetDt.Date.AddMinutes(15 * rowIter), Val = Convert.ToDouble(val) });
                }
            }
            return schData;
        }

        public UtilSchData GetOnbarInstalledCapacityForDates(string utilId, DateTime fromDate, DateTime toDate)
        {
            UtilSchData utilData = new UtilSchData();
            string dbName = (DateTime.Now.Date - fromDate.Date).TotalDays > 6 ? "SCHOLD" : "SCHN7";
            using (OracleConnection con = new OracleConnection(_oracleConnString))
            {
                using OracleCommand cmd = con.CreateCommand();
                try
                {
                    // get dc data
                    con.Open();
                    cmd.BindByName = true;
                    cmd.CommandText = @$"SELECT declared_for_date, ON_BAR_INSTALLED_CAPACITY FROM {dbName}.declaration where (util_id, declared_for_date, revision_no) in 
                                            (
                                                select util_id, declared_for_date, max(revision_no) from {dbName}.declaration 
                                                WHERE declared_for_date between :win_start and :win_end and is_scheduled=1
                                                and util_id = :util_id
                                                GROUP BY (util_id,declared_for_date)
                                            )
                                            order by declared_for_date";

                    // Assign parameters
                    OracleParameter win_start = new OracleParameter("win_start", fromDate.Date);
                    cmd.Parameters.Add(win_start);

                    OracleParameter win_end = new OracleParameter("win_end", toDate.Date);
                    cmd.Parameters.Add(win_end);

                    OracleParameter util_id = new OracleParameter("util_id", utilId);
                    cmd.Parameters.Add(util_id);

                    //Execute the command and use DataReader to read the data
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        DateTime dcDate = reader.GetDateTime(0);
                        string onBarInstalledCapStr = reader.GetString(1);
                        List<double> onBarInstCapVals = onBarInstalledCapStr.Split(',').Select(s => Convert.ToDouble(s)).ToList();
                        // populate the data
                        if (onBarInstCapVals.Count == 96)
                        {
                            for (int valIter = 0; valIter < onBarInstCapVals.Count; valIter++)
                            {
                                utilData.SchVals.Add(new ScheduleValue() { Timestamp = dcDate.AddMinutes(valIter * 15), Val = onBarInstCapVals[valIter] });
                            }
                        }
                    }
                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return utilData;
        }

        public UtilSchData GetOnbarForDates(string utilId, DateTime fromDate, DateTime toDate)
        {
            UtilSchData utilData = new UtilSchData();
            string dbName = (DateTime.Now.Date - fromDate.Date).TotalDays > 6 ? "SCHOLD" : "SCHN7";
            using (OracleConnection con = new OracleConnection(_oracleConnString))
            {
                using OracleCommand cmd = con.CreateCommand();
                try
                {
                    // get dc data
                    con.Open();
                    cmd.BindByName = true;
                    cmd.CommandText = @$"SELECT declared_for_date, DECLARED_ON_BAR FROM {dbName}.declaration where (util_id, declared_for_date, revision_no) in 
                                            (
                                                select util_id, declared_for_date, max(revision_no) from {dbName}.declaration 
                                                WHERE declared_for_date between :win_start and :win_end and is_scheduled=1
                                                and util_id = :util_id
                                                GROUP BY (util_id,declared_for_date)
                                            )
                                            order by declared_for_date";

                    // Assign parameters
                    OracleParameter win_start = new OracleParameter("win_start", fromDate.Date);
                    cmd.Parameters.Add(win_start);

                    OracleParameter win_end = new OracleParameter("win_end", toDate.Date);
                    cmd.Parameters.Add(win_end);

                    OracleParameter util_id = new OracleParameter("util_id", utilId);
                    cmd.Parameters.Add(util_id);

                    //Execute the command and use DataReader to read the data
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        DateTime dcDate = reader.GetDateTime(0);
                        string onBarStr = reader.GetString(1);
                        List<double> onBarVals = onBarStr.Split(',').Select(s => Convert.ToDouble(s)).ToList();
                        // populate the data
                        if (onBarVals.Count == 96)
                        {
                            for (int valIter = 0; valIter < onBarVals.Count; valIter++)
                            {
                                utilData.SchVals.Add(new ScheduleValue() { Timestamp = dcDate.AddMinutes(valIter * 15), Val = onBarVals[valIter] });
                            }
                        }
                    }
                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return utilData;
        }

        public UtilSchData GetRrasForDates(string utilId, DateTime fromDate, DateTime toDate)
        {
            UtilSchData utilData = new UtilSchData();
            string dbName = (DateTime.Now.Date - fromDate.Date).TotalDays > 6 ? "SCHOLD" : "SCHN7";
            using (OracleConnection con = new OracleConnection(_oracleConnString))
            {
                using OracleCommand cmd = con.CreateCommand();
                try
                {
                    // get dc data
                    con.Open();
                    cmd.BindByName = true;
                    cmd.CommandText = @$"SELECT schedule_date, seller_amount FROM {dbName}.full_schedule 
                                            where (schedule_date,revision_no) in
                                            (
                                            select effective_date, max(WR) from {dbName}.revision_main where effective_date between :win_start and :win_end group by effective_date
                                            ) and
                                            seller_id = :util_id and
                                            buyer_id = :buyer_id and
                                            schedule_type = 9 order by schedule_date";

                    // Assign parameters
                    OracleParameter win_start = new OracleParameter("win_start", fromDate.Date);
                    cmd.Parameters.Add(win_start);

                    OracleParameter win_end = new OracleParameter("win_end", toDate.Date);
                    cmd.Parameters.Add(win_end);

                    OracleParameter util_id = new OracleParameter("util_id", utilId);
                    cmd.Parameters.Add(util_id);

                    OracleParameter buyer_id = new OracleParameter("buyer_id", "e1e5ccad-9f05-4130-9362-baf3033f1f40");
                    cmd.Parameters.Add(buyer_id);

                    //Execute the command and use DataReader to read the data
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        DateTime schDate = reader.GetDateTime(0);
                        string rrasStr = reader.GetString(1);
                        List<double> rrasVals = rrasStr.Split(',').Select(s => Convert.ToDouble(s)).ToList();
                        // populate the data
                        if (rrasVals.Count == 96)
                        {
                            for (int valIter = 0; valIter < rrasVals.Count; valIter++)
                            {
                                utilData.SchVals.Add(new ScheduleValue() { Timestamp = schDate.AddMinutes(valIter * 15), Val = rrasVals[valIter] });
                            }
                        }
                    }
                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return utilData;
        }

        public UtilSchData GetScedForDates(string utilId, DateTime fromDate, DateTime toDate)
        {
            UtilSchData utilData = new UtilSchData();
            string dbName = (DateTime.Now.Date - fromDate.Date).TotalDays > 6 ? "SCHOLD" : "SCHN7";
            using (OracleConnection con = new OracleConnection(_oracleConnString))
            {
                using OracleCommand cmd = con.CreateCommand();
                try
                {
                    // get dc data
                    con.Open();
                    cmd.BindByName = true;
                    cmd.CommandText = @$"select effective_date, actual_up, actual_down from {dbName}.sced 
                                        where 
                                            util_id = :util_id 
                                            and (revision_no, effective_date) in (
                                            select max(revision_no), effective_date from {dbName}.sced 
                                            where 
                                                util_id = :util_id 
                                                and effective_date between :win_start and :win_end 
                                            group by effective_date
                                            ) 
                                        order by effective_date";

                    // Assign parameters
                    OracleParameter win_start = new OracleParameter("win_start", fromDate.Date);
                    cmd.Parameters.Add(win_start);

                    OracleParameter win_end = new OracleParameter("win_end", toDate.Date);
                    cmd.Parameters.Add(win_end);

                    OracleParameter util_id = new OracleParameter("util_id", utilId);
                    cmd.Parameters.Add(util_id);

                    //Execute the command and use DataReader to read the data
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        DateTime schDate = reader.GetDateTime(0);
                        string scedUpStr = reader.GetString(1);
                        string scedDownStr = reader.GetString(2);
                        List<double> scedUpVals = scedUpStr.Split(',').Select(s => Convert.ToDouble(s)).ToList();
                        List<double> scedDownVals = scedDownStr.Split(',').Select(s => Convert.ToDouble(s)).ToList();
                        // populate the data
                        if (scedUpVals.Count == 96 && scedDownVals.Count == 96)
                        {
                            for (int valIter = 0; valIter < scedUpVals.Count; valIter++)
                            {
                                utilData.SchVals.Add(new ScheduleValue() { Timestamp = schDate.AddMinutes(valIter * 15), Val = scedUpVals[valIter] - scedDownVals[valIter] });
                            }
                        }
                    }
                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return utilData;
        }

        public async Task<UtilSchData> GetDownMarginsForDate(string utilId, DateTime fromDate)
        {
            UtilSchData margins = new UtilSchData();
            UtilSchData onBarInstCap = GetOnbarInstalledCapacityForDates(utilId, fromDate, fromDate);
            UtilSchData fullSch = await GetSellerFullSchForDate(utilId, fromDate);
            if (onBarInstCap == null || fullSch == null || onBarInstCap.SchVals.Count != 96 || fullSch.SchVals.Count != 96)
            {
                return margins;
            }
            for (int valIter = 0; valIter < onBarInstCap.SchVals.Count; valIter++)
            {
                double val = fullSch.SchVals[valIter].Val - onBarInstCap.SchVals[valIter].Val * 0.55;
                DateTime timestamp = fullSch.SchVals[valIter].Timestamp;
                margins.SchVals.Add(new ScheduleValue { Timestamp = timestamp, Val = val > 0 ? val : 0 });
            }
            return margins;
        }

        public async Task<UtilSchData> GetDownMarginsForDates(string utilId, DateTime fromDate, DateTime toDate)
        {
            UtilSchData margins = new UtilSchData();
            for (DateTime currDate = fromDate.Date; currDate <= toDate.Date; currDate = currDate.AddDays(1))
            {
                UtilSchData dateMargins = await GetDownMarginsForDate(utilId, currDate);
                margins.SchVals.AddRange(dateMargins.SchVals);
            }
            return margins;
        }

        public async Task<IsgsSchedulesDTO> GetIsgsThermalDownMarginsForDates(DateTime fromDate, DateTime toDate)
        {
            IsgsSchedulesDTO margins = new IsgsSchedulesDTO();
            List<(string utilId, string utilName)> utils = GetAllThermalIsgsUtils();
            bool isFirstIter = true;
            foreach ((string utilId, string utilName) util in utils)
            {
                UtilSchData utilMargins = new UtilSchData();
                for (DateTime currDate = fromDate.Date; currDate <= toDate.Date; currDate = currDate.AddDays(1))
                {
                    UtilSchData dateMargins = await GetDownMarginsForDate(util.utilId, currDate);
                    utilMargins.SchVals.AddRange(dateMargins.SchVals);
                }
                margins.Margins.Add(util.utilName, utilMargins.SchVals.Select(v => v.Val).ToList());
                margins.GenNames.Add(util.utilName);
                if (isFirstIter)
                {
                    margins.Timestamps = utilMargins.SchVals.Select(v => v.Timestamp).ToList();
                    isFirstIter = false;
                }
            }
            return margins;
        }

        public async Task<UtilSchData> GetUpMarginsForDate(string utilId, DateTime fromDate)
        {
            UtilSchData margins = new UtilSchData();
            UtilSchData onBar = GetOnbarForDates(utilId, fromDate, fromDate);
            UtilSchData fullSch = await GetSellerFullSchForDate(utilId, fromDate);
            if (onBar == null || fullSch == null || onBar.SchVals.Count != 96 || fullSch.SchVals.Count != 96)
            {
                return margins;
            }
            for (int valIter = 0; valIter < onBar.SchVals.Count; valIter++)
            {
                double val = onBar.SchVals[valIter].Val - fullSch.SchVals[valIter].Val;
                DateTime timestamp = fullSch.SchVals[valIter].Timestamp;
                margins.SchVals.Add(new ScheduleValue { Timestamp = timestamp, Val = val > 0 ? val : 0 });
            }
            return margins;
        }

        public async Task<UtilSchData> GetUpMarginsForDates(string utilId, DateTime fromDate, DateTime toDate)
        {
            UtilSchData margins = new UtilSchData();
            for (DateTime currDate = fromDate.Date; currDate <= toDate.Date; currDate = currDate.AddDays(1))
            {
                UtilSchData dateMargins = await GetUpMarginsForDate(utilId, currDate);
                margins.SchVals.AddRange(dateMargins.SchVals);
            }
            return margins;
        }

        public async Task<IsgsSchedulesDTO> GetIsgsThermalUpMarginsForDates(DateTime fromDate, DateTime toDate)
        {
            IsgsSchedulesDTO margins = new IsgsSchedulesDTO();
            List<(string utilId, string utilName)> utils = GetAllThermalIsgsUtils();
            bool isFirstIter = true;
            foreach ((string utilId, string utilName) util in utils)
            {
                UtilSchData utilMargins = new UtilSchData();
                for (DateTime currDate = fromDate.Date; currDate <= toDate.Date; currDate = currDate.AddDays(1))
                {
                    UtilSchData dateMargins = await GetUpMarginsForDate(util.utilId, currDate);
                    utilMargins.SchVals.AddRange(dateMargins.SchVals);
                }
                margins.Margins.Add(util.utilName, utilMargins.SchVals.Select(v => v.Val).ToList());
                margins.GenNames.Add(util.utilName);
                if (isFirstIter)
                {
                    margins.Timestamps = utilMargins.SchVals.Select(v => v.Timestamp).ToList();
                    isFirstIter = false;
                }
            }
            return margins;
        }

        public async Task<IsgsSchedulesDTO> GetIsgsRrasForDates(DateTime fromDate, DateTime toDate)
        {
            IsgsSchedulesDTO margins = new IsgsSchedulesDTO();
            List<(string utilId, string utilName)> utils = GetAllThermalIsgsUtils();
            bool isFirstIter = true;
            List<(DateTime, DateTime)> dates = SplitDateRangeForDbFetch(fromDate.Date, toDate.Date);
            foreach ((string utilId, string utilName) in utils)
            {
                // todo check if from and to dates belong in different databases
                UtilSchData utilRras = new UtilSchData();
                foreach ((DateTime, DateTime) date in dates)
                {
                    UtilSchData schData = GetRrasForDates(utilId, date.Item1, date.Item2);
                    utilRras.SchVals.AddRange(schData.SchVals);
                }
                margins.Margins.Add(utilName, utilRras.SchVals.Select(v => v.Val).ToList());
                margins.GenNames.Add(utilName);
                if (isFirstIter)
                {
                    margins.Timestamps = utilRras.SchVals.Select(v => v.Timestamp).ToList();
                    isFirstIter = false;
                }
            }
            return await Task.FromResult(margins);
        }

        public async Task<IsgsSchedulesDTO> GetIsgsScedForDates(DateTime fromDate, DateTime toDate)
        {
            IsgsSchedulesDTO margins = new IsgsSchedulesDTO();
            List<(string utilId, string utilName)> utils = GetAllThermalIsgsUtils();
            bool isFirstIter = true;
            List<(DateTime, DateTime)> dates = SplitDateRangeForDbFetch(fromDate.Date, toDate.Date);
            foreach ((string utilId, string utilName) in utils)
            {
                UtilSchData utilSced = new UtilSchData();
                foreach ((DateTime, DateTime) date in dates)
                {
                    UtilSchData schData = GetScedForDates(utilId, date.Item1, date.Item2);
                    utilSced.SchVals.AddRange(schData.SchVals);
                }
                margins.Margins.Add(utilName, utilSced.SchVals.Select(v => v.Val).ToList());
                margins.GenNames.Add(utilName);
                if (isFirstIter)
                {
                    margins.Timestamps = utilSced.SchVals.Select(v => v.Timestamp).ToList();
                    isFirstIter = false;
                }
            }
            return await Task.FromResult(margins);
        }

        public List<(DateTime, DateTime)> SplitDateRangeForDbFetch(DateTime fromDate, DateTime toDate)
        {
            DateTime d = DateTime.Now.Date;
            DateTime dMinus6 = d - new TimeSpan(6, 0, 0, 0);
            DateTime dMinus7 = d - new TimeSpan(7, 0, 0, 0);

            if (fromDate < dMinus6 && toDate < dMinus6)
            {
                return new List<(DateTime, DateTime)>() { (fromDate, toDate) };
            }
            else if (fromDate >= dMinus6 && toDate >= dMinus6)
            {
                return new List<(DateTime, DateTime)>() { (fromDate, toDate) };
            }
            else
            {
                return new List<(DateTime, DateTime)>() { (fromDate, dMinus7), (dMinus6, toDate) };
            }
        }
    }
}
