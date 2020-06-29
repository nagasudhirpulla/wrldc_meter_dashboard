using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeterDataDashboard.Core.TempHumidity;
using MeterDataDashboard.Core.TempHumidity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MeterDataDashboard.Web.Pages.DeviceMonitoring
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IDeviceDataService _deviceDataService;
        public IndexModel(IDeviceDataService deviceDataService)
        {
            _deviceDataService = deviceDataService;
        }

        public IList<DeviceDataDTO> DeviceData { get; set; }

        public async Task OnGetAsync()
        {
            DeviceData = await _deviceDataService.GetRealTimeDevicesData();
        }
    }
}