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
using MeterDataDashboard.Core.MeterData.Services;

namespace MeterDataDashboard.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Startup.ApiAuthSchemes)]
    public class FictDataController : ControllerBase
    {
        private readonly AppDbContext _meterDbContext;
        private readonly IMeterDataService _meterDataService;

        public FictDataController(AppDbContext appDbContext, IMeterDataService meterDataService)
        {
            _meterDbContext = appDbContext;
            _meterDataService = meterDataService;
        }

        [HttpGet("GetMeasurementsTable")]
        public async Task<IEnumerable<IEnumerable<string>>> GetMeasurementsTable()
        {
            // https://localhost:44390/api/fictdata/GetMeasurementsTable
            List<List<string>> measTable = await _meterDbContext.FictMeasurements.Select(fm => new List<string> { fm.LocationTag, fm.Description }).ToListAsync();
            measTable.Insert(0, new List<string>() { "id", "Description" });
            return measTable;
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
            IEnumerable<double> res = new List<double>();
            DateTime startDate = DateTime.ParseExact(start_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(end_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            try
            {
                res = _meterDataService.FetchFictData(tag, startDate, endDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return res;
        }
    }
}