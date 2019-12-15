using System;
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
    }
}