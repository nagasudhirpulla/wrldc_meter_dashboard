using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MeterDataDashboard.Core.ReportsData;
using MeterDataDashboard.Core.ReportsData.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeterDataDashboard.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsDataService _reportsDataService;

        public ReportsController(IReportsDataService reportsDataService)
        {
            _reportsDataService = reportsDataService;
        }

        [HttpGet("GetMeasurements")]
        public async Task<IEnumerable<PspMeasurement>> GetMeasurements()
        {
            // https://localhost:44390/api/reports/getmeasurements
            List<PspMeasurement> measurements = await _reportsDataService.GetAllMeasurements();
            return measurements;
        }

        [HttpGet("GetMeasurementsTable")]
        public async Task<IEnumerable<IEnumerable<string>>> GetMeasurementsTable()
        {
            // https://localhost:44390/api/reports/GetMeasurementsTable
            List<List<string>> measTable = (await _reportsDataService.GetAllMeasurements()).Select(m => new List<string> { m.Label }).ToList();
            measTable.Insert(0, new List<string>() { "id" });
            return measTable;
        }

        [HttpGet("getMeasData/{measLabel}/{startTimeStr}/{endTimeStr}")]
        public async Task<IEnumerable<double>> GetMeasurementData(string measLabel, string startTimeStr, string endTimeStr)
        {
            // https://localhost:44390/api/reports/getMeasData/WR_DAY_AVERAGE_FREQ/2020-04-01-00-00-00/2020-04-15-00-00-00
            IEnumerable<double> res = new List<double>();
            DateTime startDate = DateTime.ParseExact(startTimeStr, "yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(endTimeStr, "yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
            try
            {
                res = await _reportsDataService.GetMeasurementData(measLabel, startDate, endDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return res;
        }
    }
}