using MeterDataDashboard.Application;
using MeterDataDashboard.Core.MeterData.Services;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;

namespace MeterDataDashboard.Infra.Services
{
    public class MeterDataService : IMeterDataService
    {
        private readonly IConfiguration _configuration;
        public MeterDataService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<double> FetchFictData(string tag, DateTime startDate, DateTime endDate)
        {
            List<double> res = new List<double>();
            // Connect to a PostgreSQL database
            NpgsqlConnection conn = new NpgsqlConnection(_configuration["ConnectionStrings:MeterConnection"]);
            conn.Open();

            // Define a query
            NpgsqlCommand command = new NpgsqlCommand(@"SELECT data_time, mwh FROM public.fict_location_energy_data 
                                                        where location_id=@locationId and data_time 
                                                        between @startTime and @endTime order by data_time", conn);

            command.Parameters.AddWithValue("@locationId", tag);
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
                    double val = 4 * dr.GetDouble(1);
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
