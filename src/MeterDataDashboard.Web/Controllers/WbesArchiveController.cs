using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeterDataDashboard.Core.Entities;
using MeterDataDashboard.Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using MeterDataDashboard.Core.ScadaData.Services;

namespace MeterDataDashboard.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Startup.ApiAuthSchemes)]
    public class WbesArchiveController : ControllerBase
    {
        private readonly IWbesArchiveDataService _wbesArchDataService;

        public WbesArchiveController(IWbesArchiveDataService wbesArchDataService)
        {
            _wbesArchDataService = wbesArchDataService;
        }

        [HttpGet("GetUtilities")]
        public IEnumerable<string> GetUtilities()
        {
            IEnumerable<string> utilNames = _wbesArchDataService.FetchSchUtils();
            return utilNames;
        }

        [HttpGet("GetSchTypes")]
        public IEnumerable<object> GetSchTypes()
        {
            List<object> schTypes = new List<object>() {
                new { t = "Total", v= "Total"},
                new { t = "ISGS", v= "ISGS"},
                new { t = "Onbar Installed Capacity", v= "icOnBar"},
                new { t = "Onbar DC For Schedule", v= "onBarForSch"},
                new { t = "Onbar DC By Seller", v= "sellOnBar"},
                new { t = "Offbar DC", v= "offBar"},
                new { t = "LTA", v= "LTA"},
                new { t = "MTOA", v= "MTOA"},
                new { t = "STOA", v= "STOA"},
                new { t = "URS", v= "URS"},
                new { t = "IEX", v= "IEX"},
                new { t = "PXI", v= "PXI"},
                new { t = "RRAS", v= "RRAS"},
                new { t = "SCED", v= "SCED"},
                new { t = "Ramp Up", v= "rampUp"},
                new { t = "Ramp Down", v= "rampDn"},
                new { t = "AGC", v= "AGC"}
            };
            return schTypes;
        }

        [HttpGet("{utilName}/{schType}/{start_date}/{end_date}")]
        public IEnumerable<double> Index(string utilName, string schType, string start_date, string end_date)
        {
            // https://localhost:44390/api/WbesArchive/CGPL/Total/2019-12-01/2019-12-10
            IEnumerable<double> res = new List<double>();
            DateTime startDate = DateTime.ParseExact(start_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(end_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            try
            {
                res = _wbesArchDataService.FetchData(utilName, schType, startDate, endDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return res;
        }
    }
}