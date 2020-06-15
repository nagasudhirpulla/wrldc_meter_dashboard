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
using MeterDataDashboard.Core.ScadaNodes.Services;

namespace MeterDataDashboard.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Startup.ApiAuthSchemes)]
    public class PingStatusController : ControllerBase
    {
        private readonly IScadaNodesPingStatsService _pingStatusService;

        public PingStatusController(IScadaNodesPingStatsService pingStatusService)
        {
            _pingStatusService = pingStatusService;
        }

        [HttpGet("GetMeasurementsTable")]
        public async Task<IEnumerable<IEnumerable<string>>> GetMeasurementsTable()
        {
            // https://localhost:44390/api/pingStatus/GetMeasurementsTable
            List<List<string>> nodeNamesTable = _pingStatusService.FetchNodesPingLiveStatus().Select(s => new List<string>() { s.NodeName }).ToList();
            nodeNamesTable.Insert(0, new List<string>() { "id" });
            return await Task.FromResult(nodeNamesTable);
        }

        [HttpGet("GetMeasurements")]
        public async Task<IEnumerable<string>> GetMeasurements()
        {
            // https://localhost:44390/api/pingStatus/getmeasurements
            List<string> nodeNames = _pingStatusService.FetchNodesPingLiveStatus().Select(s => s.NodeName).ToList();
            return await Task.FromResult(nodeNames);
        }

        [HttpGet("{tag}/{start_date}/{end_date}")]
        public IEnumerable<double> Index(string tag, string start_date, string end_date)
        {
            // https://localhost:44390/pingStatus/fictdata/name/2018-05-01-00-00-00/2018-05-10-00-00-00
            IEnumerable<double> res = new List<double>();
            DateTime startDate = DateTime.ParseExact(start_date, "yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(end_date, "yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
            try
            {
                res = _pingStatusService.FetchNodePingHist(tag, startDate, endDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return res;
        }

    }
}