﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeterDataDashboard.Web.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        public IActionResult Meter()
        {
            return View();
        }

        public IActionResult Scada()
        {
            return View();
        }

        public IActionResult WbesArchive()
        {
            return View();
        }

        public IActionResult AllData()
        {
            return View();
        }
        public IActionResult DemandFreq()
        {
            return View();
        }
        public IActionResult StateDemands()
        {
            return View();
        }

        public IActionResult DownMargins()
        {
            return View();
        }

        public IActionResult PmuFrequency()
        {
            return View();
        }
    }
}