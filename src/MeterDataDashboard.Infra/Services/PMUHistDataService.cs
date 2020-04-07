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
        private readonly PmuConfig PmuConfig_;
        public PMUHistDataService(PmuConfig pmuConfig)
        {
            PmuConfig_ = pmuConfig;
        }

        public string FetchData(string measId, DateTime startTime, DateTime endTime)
        {
            string res;
            List<string> args = new List<string>();
            // add meas Id
            args.AddRange(new List<string>() { "--meas_id", measId });
            args.AddRange(new List<string>() { "--from_time", startTime.ToString("yyyy_MM_dd_HH_mm_ss") });
            args.AddRange(new List<string>() { "--to_time", endTime.ToString("yyyy_MM_dd_HH_mm_ss") });
            args.AddRange(new List<string>() { "--host", PmuConfig_.Host });
            args.AddRange(new List<string>() { "--port", $"{PmuConfig_.Port}" });
            args.AddRange(new List<string>() { "--path", PmuConfig_.Path });
            args.AddRange(new List<string>() { "--username", PmuConfig_.Username });
            args.AddRange(new List<string>() { "--password", PmuConfig_.Password });
            args.AddRange(new List<string>() { "--ref_meas_id", $"{PmuConfig_.RefMeasId}" });
            args.AddRange(new List<string>() { "--data_rate", $"{PmuConfig_.DataRate}" });
            try
            {
                // spawn process to get PMU data
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = PmuConfig_.AdapterPath,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Arguments = String.Join(" ", args)
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
