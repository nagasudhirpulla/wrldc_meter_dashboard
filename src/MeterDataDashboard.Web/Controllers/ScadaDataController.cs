﻿using System;
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
    [Authorize]
    public class ScadaDataController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IScadaDataService _scadaDataService;

        public ScadaDataController(AppDbContext appDbContext, IScadaDataService scadaDataService)
        {
            _appDbContext = appDbContext;
            _scadaDataService = scadaDataService;
        }

        [HttpGet("GetMeasurements")]
        public async Task<IEnumerable<ScadaArchiveMeasurement>> GetMeasurements()
        {
            // https://localhost:44390/api/fictdata/getmeasurements
            List<ScadaArchiveMeasurement> scadaMeasurements = await _appDbContext.ScadaArchiveMeasurements.ToListAsync();
            return scadaMeasurements;
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