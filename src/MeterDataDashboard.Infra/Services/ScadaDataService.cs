using MeterDataDashboard.Application;
using MeterDataDashboard.Core.ScadaData.Services;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;

namespace MeterDataDashboard.Infra.Services
{
    public class ScadaDataService : IScadaDataService
    {
        private readonly IConfiguration _configuration;
        public ScadaDataService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<double> FetchScadaData(string tag, DateTime startDate, DateTime endDate)
        {
            List<double> res = new List<double>();
            // Connect to a PostgreSQL database
            NpgsqlConnection conn = new NpgsqlConnection(_configuration["ConnectionStrings:ScadaArchiveConnection"]);
            conn.Open();

            // Define a query
            NpgsqlCommand command = new NpgsqlCommand(@"SELECT meas_time, meas_val FROM public.meas_time_data 
                                                        where meas_tag=@measTag and meas_time 
                                                        between @startTime and @endTime order by meas_time", conn);

            command.Parameters.AddWithValue("@measTag", tag);
            command.Parameters.AddWithValue("@startTime", startDate);
            command.Parameters.AddWithValue("@endTime", endDate);

            // Execute the query and obtain a result set
            NpgsqlDataReader dr = command.ExecuteReader();

            while (dr.HasRows)
            {
                while (dr.Read())
                {
                    DateTime dt = dr.GetDateTime(0);
                    double ts = TimeUtils.ToMillisSinceUnixEpoch(dt);
                    double val = dr.GetDouble(1);
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
