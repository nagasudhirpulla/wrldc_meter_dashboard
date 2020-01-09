using MeterDataDashboard.Application;
using MeterDataDashboard.Core.Entities;
using MeterDataDashboard.Core.ScadaData.Services;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;

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
            return res;
        }
    }
}
