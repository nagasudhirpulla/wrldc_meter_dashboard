using MeterDataDashboard.Core.PmuData.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using MeterDataDashboard.Core.PmuData;
using System.Threading.Tasks;
using MeterDataDashboard.Application;
using System.Linq;
using System.Diagnostics;

namespace MeterDataDashboard.Infra.Services
{
    public class PMUHistDataService : IPMUHistDataService
    {
        private readonly IConfiguration _configuration;
        public PMUHistDataService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string FetchData(string measId, DateTime startTime, DateTime endTime)
        {
            string res;
            try
            {
                // spawn process to get PMU data
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _configuration["pmuAdapterPath"],
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };
                process.Start();
                res = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                res = "";
            }
            return $"[{res}]";
        }
    }
}
