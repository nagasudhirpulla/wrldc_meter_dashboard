using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MeterDataDashboard.Application;
using MeterDataDashboard.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using MeterDataDashboard.Core.Entities;
using MeterDataDashboard.Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MeterDataDashboard.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class FictDataController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly MeterDbContext _meterDbContext;

        public FictDataController(IConfiguration configuration, MeterDbContext meterDbContext)
        {
            _configuration = configuration;
            _meterDbContext = meterDbContext;
        }

        [HttpGet("GetMeasurements")]
        public async Task<IEnumerable<FictMeasurement>> GetMeasurements()
        {
            // https://localhost:44390/api/fictdata/getmeasurements
            List<FictMeasurement> fictMeasurements = await _meterDbContext.FictMeasurements.ToListAsync();
            return fictMeasurements;
        }

        [HttpGet("{tag}/{start_date}/{end_date}")]
        public IEnumerable<double> Index(string tag, string start_date, string end_date)
        {
            // https://localhost:44390/api/fictdata/LO-91/2018-05-01/2018-05-10
            List<double> res = new List<double>();
            DateTime startDate = DateTime.ParseExact(start_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(end_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            try
            {
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
                        double val = dr.GetDouble(1);
                        res.Add(ts);
                        res.Add(val);
                    }
                    dr.NextResult();
                }

                dr.Dispose();

                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return res;
        }
    }
}