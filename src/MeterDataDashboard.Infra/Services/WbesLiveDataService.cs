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

        public List<(string, string)> GetAllThermalIsgsUtils()
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
                    cmd.CommandText = @"select util_id, acronym from WBES_NR7.utility 
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
            string url = $"http://scheduling.wrldc.in/wbes/Report/GetCurrentDayFullScheduleMaxRev?regionid=2&ScheduleDate={targetDt.ToString("dd-MM-yyyy")}";
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

        public async Task<UtilSchData> GetSellerFullSchForDate(DateTime targetDt, string utilId)
        {
            // https://stackoverflow.com/questions/32860666/httpclient-scrape-data-from-website-with-login-c-sharp
            int rev = await GetMaxRevForDate(targetDt);
            UtilSchData schData = new UtilSchData() { SchVals = new List<ScheduleValue>() };
            if (rev == -1) { return null; }
            string url = $"http://scheduling.wrldc.in/wbes/ReportFullSchedule/GetFullInjSummary?scheduleDate={targetDt.ToString("dd-MM-yyyy")}&sellerId={utilId}&revisionNumber={rev}&regionId=2&byDetails=0&isDrawer=0&isBuyer=0";
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
            UtilSchData utilData = new UtilSchData() { SchVals = new List<ScheduleValue>() };
            string dbName = (DateTime.Now.Date - fromDate.Date).TotalDays > 6 ? "WBES_OLD" : "WBES_NR7";
            using (OracleConnection con = new OracleConnection(_oracleConnString))
            {
                using OracleCommand cmd = con.CreateCommand();
                try
                {
                    // get dc data
                    con.Open();
                    cmd.BindByName = true;
                    cmd.CommandText = @$"select table1.declared_for_date, table1.ON_BAR_INSTALLED_CAPACITY, table2.acronym from 
                                            (SELECT util_id, declared_for_date, ON_BAR_INSTALLED_CAPACITY FROM {dbName}.declaration where (util_id, declared_for_date, revision_no) in 
                                            (
                                                select util_id, declared_for_date, max(revision_no) from {dbName}.declaration 
                                                WHERE declared_for_date between :win_start and :win_end and is_scheduled=1
                                                and util_id = :util_id
                                                GROUP BY (util_id, declared_for_date)
                                            )) table1
                                            left join WBES_NR7.utility table2 on table1.util_id = table2.util_id
                                            order by declared_for_date, acronym";

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
    }
}
