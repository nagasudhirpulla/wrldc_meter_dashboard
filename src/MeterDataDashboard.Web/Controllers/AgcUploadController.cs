using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeterDataDashboard.Infra.Identity;
using MeterDataDashboard.Web.Models;
using MeterDataDashboard.Web.Extensions;
using Microsoft.AspNetCore.Http;
using System.IO;
using MeterDataDashboard.Core.ScheduleData;
using MeterDataDashboard.Core.ScadaData.Services;

namespace MeterDataDashboard.Web.Controllers
{
    [Authorize(Roles = SecurityConstants.AdminRoleString)]
    public class AgcUploadController : Controller
    {
        // upload multiple files in asp.net core - https://www.talkingdotnet.com/uploading-multiple-files-asp-net-core-razor-pages/

        public readonly IAgcFileUtilsService _agcFileUtilsService;
        public readonly IWbesArchiveDataService _wbesArchiveDataService;
        public AgcUploadController(IAgcFileUtilsService agcFileUtilsService, IWbesArchiveDataService wbesArchiveDataService)
        {
            _agcFileUtilsService = agcFileUtilsService;
            _wbesArchiveDataService = wbesArchiveDataService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Index(AgcUploadVm vm)
        {
            // read csv data from posted file
            if (vm.DataFiles != null && vm.DataFiles.Count > 0)
            {
                foreach (IFormFile dataFile in vm.DataFiles)
                {
                    if (dataFile.Length > 0)
                    {
                        try
                        {
                            string fileExtension = Path.GetExtension(dataFile.FileName);
                            //Validate uploaded file and return error.
                            if (fileExtension != ".csv")
                            {
                                // donot process this file
                                continue;
                            }
                            List<UtilSchRow> rows = new List<UtilSchRow>();
                            // https://stackoverflow.com/questions/40045147/how-to-read-into-memory-the-lines-of-a-text-file-from-an-iformfile-in-asp-net-co/40045456
                            using (StreamReader sreader = new StreamReader(dataFile.OpenReadStream()))
                            {
                                rows = _agcFileUtilsService.ParseAgcCsv(sreader);
                                _wbesArchiveDataService.PushSchRowsToArchive(rows);
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
            }
            return View(vm).WithSuccess("Files uploading done");
        }
    }
}