using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeterDataDashboard.Core.PmuData.Services;
using System.Globalization;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MeterDataDashboard.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PmuDataController : ControllerBase
    {
        private readonly IPMUHistDataService _pmuDataService;

        public PmuDataController(IPMUHistDataService pmuDataService)
        {
            _pmuDataService = pmuDataService;
        }

        [HttpGet("GetData/{measId}/{start_date}/{end_date}")]
        public string GetData(string measId, string start_date, string end_date)
        {
            DateTime startDate = DateTime.ParseExact(start_date, "yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(end_date, "yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
            string res = _pmuDataService.FetchData(measId, startDate, endDate);
            return res;
        }
    }
}