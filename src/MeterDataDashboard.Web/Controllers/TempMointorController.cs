using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MeterDataDashboard.Core.TempHumidity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeterDataDashboard.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Startup.ApiAuthSchemes)]
    public class TempMointorController : Controller
    {
        public const string CommRoomName = "Communication_Room";
        public const string UpsRoomName = "UPS_Room";
        public const string ServRoomName = "Server_Room";

        private readonly IDeviceDataService _deviceDataService;
        public TempMointorController(IDeviceDataService deviceDataService)
        {
            _deviceDataService = deviceDataService;
        }

        [HttpGet("{tag}/{start_date}/{end_date}")]
        public async Task<IEnumerable<double>> Index(string tag, string start_date, string end_date)
        {
            // https://localhost:44390/TempMointor/Server_Room|Temperature/2018-06-18-00-00-00/2018-06-28-00-00-00
            IEnumerable<double> res = new List<double>();
            DateTime startDate = DateTime.ParseExact(start_date, "yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(end_date, "yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
            try
            {
                res = await _deviceDataService.GetHistDeviceData(tag, startDate, endDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return res;
        }

        [HttpGet("GetMeasurementsTable")]
        public async Task<IEnumerable<IEnumerable<string>>> GetMeasurementsTable()
        {
            // https://localhost:44390/api/TempMointor/GetMeasurementsTable
            List<List<string>> roomNamesTable = new List<List<string>>() {
                new List<string>() { CommRoomName },
                new List<string>() { ServRoomName },
                new List<string>() { UpsRoomName }
            };
            roomNamesTable.Insert(0, new List<string>() { "id" });
            return await Task.FromResult(roomNamesTable);
        }

        [HttpGet("GetMeasurements")]
        public async Task<IEnumerable<string>> GetMeasurements()
        {
            // https://localhost:44390/api/TempMointor/getmeasurements
            List<string> roomNames = new List<string>() { CommRoomName, ServRoomName, UpsRoomName };
            return await Task.FromResult(roomNames);
        }
    }
}