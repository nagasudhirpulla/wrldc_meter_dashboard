using MeterDataDashboard.Application;
using MeterDataDashboard.Core.Entities;
using MeterDataDashboard.Core.ScadaData.Services;
using MeterDataDashboard.Core.ScheduleData;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeterDataDashboard.Infra.Services
{
    public class WbesArchiveDataService : IWbesArchiveDataService
    {
        private readonly IConfiguration _configuration;
        public WbesArchiveDataService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<string> FetchSchUtils()
        {
            // Connect to a PostgreSQL database
            NpgsqlConnection conn = new NpgsqlConnection(_configuration["ConnectionStrings:WBESArchiveConnection"]);
            conn.Open();

            // Define query
            NpgsqlCommand cmd = new NpgsqlCommand("select name from public.isgs_gen_names order by name", conn);

            // Execute query
            NpgsqlDataReader dr = cmd.ExecuteReader();

            List<string> utilNames = new List<string>();
            // Read all rows and output the first column in each row
            while (dr.Read())
            {
                utilNames.Add(dr.GetString(0));
            }
            return utilNames;
        }

        public IEnumerable<double> FetchData(string utilName, string schType, DateTime startDate, DateTime endDate)
        {
            List<double> res = new List<double>();
            // Connect to a PostgreSQL database
            NpgsqlConnection conn = new NpgsqlConnection(_configuration["ConnectionStrings:WBESArchiveConnection"]);
            conn.Open();

            // Define a query
            NpgsqlCommand command = new NpgsqlCommand(@"SELECT sch_date, sch_block, sch_val from public.schedules 
                                                        where sch_type=@sch_type and sch_utility=@sch_utility 
                                                        and sch_date between @start_date and @end_date 
                                                        order by sch_date, sch_block", conn);
            command.Parameters.AddWithValue("@sch_utility", utilName);
            command.Parameters.AddWithValue("@sch_type", schType);
            command.Parameters.AddWithValue("@start_date", startDate);
            command.Parameters.AddWithValue("@end_date", endDate);

            // Execute the query and obtain a result set
            NpgsqlDataReader dr = command.ExecuteReader();

            while (dr.HasRows)
            {
                while (dr.Read())
                {
                    DateTime dt = dr.GetDateTime(0);
                    int valBlk = dr.GetInt32(1);
                    dt = dt.AddMinutes((valBlk - 1) * 15);
                    double ts = TimeUtils.ToMillisSinceUnixEpoch(dt);
                    double val = dr.GetDouble(2);
                    res.Add(ts);
                    res.Add(val);
                }
                dr.NextResult();
            }

            dr.Dispose();

            conn.Close();

            // schtype is agc, then fill the voids for each 15 mins
            if (schType.ToLower() == "agc")
            {
                // create a timestamp dictionary of data with zeros
                Dictionary<double, double> timeDict = new Dictionary<double, double>();
                DateTime dictStartTime = startDate.Date + new TimeSpan(startDate.Hour, startDate.Minute, 0);
                for (DateTime currTime = dictStartTime; currTime <= endDate; currTime += new TimeSpan(0, 15, 0))
                {
                    timeDict.Add(TimeUtils.ToMillisSinceUnixEpoch(currTime), 0);
                }
                // override zeros with fetched values
                for (int resIter = 0; resIter < res.Count - 1; resIter += 2)
                {
                    timeDict[res[resIter]] = res[resIter + 1];
                }

                // create a new res object with the new results
                res = new List<double>();
                foreach (KeyValuePair<double, double> entry in timeDict)
                {
                    // do something with entry.Value or entry.Key
                    res.Add(entry.Key);
                    res.Add(entry.Value);
                }
            }
            return res;

        }

        public void PushSchRowsToArchive(List<UtilSchRow> rows)
        {
            // Connect to a PostgreSQL database
            NpgsqlConnection conn = new NpgsqlConnection(_configuration["ConnectionStrings:WBESArchiveConnection"]);
            conn.Open();
            int batchSize = 100;
            // insert in batches of 100 rows
            for (int batchStart = 0; batchStart < rows.Count; batchStart += batchSize)
            {
                int batchEnd = batchStart + batchSize - 1;
                if (batchEnd >= rows.Count)
                {
                    batchEnd = rows.Count - 1;
                }
                string valuesSqlStr = rows.Skip(batchStart).Take(batchEnd - batchStart + 1)
                                          .Select(r => $"('{r.UtilName}', '{r.SchDate.ToString("yyyy-MM-dd")}', {r.Block}, '{r.SchType}', {r.SchVal})")
                                          .Aggregate((current, next) => current + "," + next);
                // Define a query
                NpgsqlCommand command = new NpgsqlCommand(@$"INSERT INTO public.schedules(
        	                                            sch_utility, sch_date, sch_block, sch_type, sch_val)
        	                                            VALUES {valuesSqlStr} on conflict (sch_utility, sch_date, sch_block, sch_type) 
                                                        do update set sch_val = excluded.sch_val", conn);
                // Execute the query and obtain a result set
                int insRes = command.ExecuteNonQuery();
            }

            conn.Close();
        }
    }
}
