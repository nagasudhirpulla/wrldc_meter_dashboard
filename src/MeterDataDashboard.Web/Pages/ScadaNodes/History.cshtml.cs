﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MeterDataDashboard.Web.Pages.ScadaNodes
{
    [Authorize]
    public class HistoryModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}