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
using MeterDataDashboard.Core.ScadaData.Services;

namespace MeterDataDashboard.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Startup.ApiAuthSchemes)]
    public class ScadaDataController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IScadaDataService _scadaDataService;

        public ScadaDataController(AppDbContext appDbContext, IScadaDataService scadaDataService)
        {
            _appDbContext = appDbContext;
            _scadaDataService = scadaDataService;
        }

        [HttpGet("GetMeasTypes")]
        public async Task<IEnumerable<string>> GetMeasTypes()
        {
            // https://localhost:44390/api/scadadata/getmeastypes
            List<string> scadaMeasTypes = await _appDbContext.ScadaArchiveMeasurements.Select(ms => ms.MeasType).Distinct().ToListAsync();
            scadaMeasTypes.Add("all");
            return scadaMeasTypes;
        }

        [HttpGet("GetMeasurements/{measType}")]
        public async Task<IEnumerable<ScadaArchiveMeasurement>> GetMeasurements(string measType)
        {
            // https://localhost:44390/api/scadadata/GetMeasurements/ict
            bool isGetAll = string.IsNullOrWhiteSpace(measType) || (measType.ToLower() == "all");
            List<ScadaArchiveMeasurement> scadaMeasurements = await _appDbContext.ScadaArchiveMeasurements.Where(sm => isGetAll || (sm.MeasType == measType)).ToListAsync();
            return scadaMeasurements;
        }

        [HttpGet("GetMeasurementsTable")]
        public async Task<IEnumerable<IEnumerable<string>>> GetMeasurementsTable(string measType)
        {
            // https://localhost:44390/api/scadadata/GetMeasurementsTable
            bool isGetAll = string.IsNullOrWhiteSpace(measType) || (measType.ToLower() == "all");
            List<List<string>> scadaMeasurementsTable = await _appDbContext.ScadaArchiveMeasurements.Select(sm => new List<string>() { sm.MeasTag, sm.Description, sm.MeasType }).ToListAsync();
            scadaMeasurementsTable.Insert(0, new List<string>() { "id", "name", "type" });
            return scadaMeasurementsTable;
        }

        [HttpGet("{tag}/{start_date}/{end_date}")]
        public IEnumerable<double> Index(string tag, string start_date, string end_date)
        {
            // https://localhost:44390/api/fictdata/LO-91/2018-05-01/2018-05-10
            IEnumerable<double> res = new List<double>();
            DateTime startDate = DateTime.ParseExact(start_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(end_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            endDate += new TimeSpan(23,59,59);
            try
            {
                res = _scadaDataService.FetchScadaData(tag, startDate, endDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return res;
        }
    }
}