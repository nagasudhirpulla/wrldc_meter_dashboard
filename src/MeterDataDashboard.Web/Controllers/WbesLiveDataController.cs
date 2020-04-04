using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeterDataDashboard.Core.ScheduleData.Services;
using System.Globalization;
using System;
using MeterDataDashboard.Core.ScheduleData;
using System.Threading.Tasks;

namespace MeterDataDashboard.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WbesLiveDataController : ControllerBase
    {
        private readonly IWbesLiveDataService _wbesLiveDataService;

        public WbesLiveDataController(IWbesLiveDataService wbesLiveDataService)
        {
            _wbesLiveDataService = wbesLiveDataService;
        }

        [HttpGet("GetIsgsThermalDownMargins/{start_date}/{end_date}")]
        public async Task<IsgsDownMarginsDTO> GetIsgsThermalDownMargins(string start_date, string end_date)
        {
            DateTime startDate = DateTime.ParseExact(start_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(end_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            IsgsDownMarginsDTO res = await _wbesLiveDataService.GetIsgsThermalDownMarginsForDates(startDate, endDate);
            return res;
        }
    }
}