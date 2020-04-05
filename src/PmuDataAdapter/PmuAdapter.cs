using InStep.eDNA.EzDNAApiNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMUDataAdapter
{
    public class PmuAdapter
    {

        public PmuAdapter()
        {

        }
        /// UNIX time epoch
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public double ToMillisSinceUnixEpoch(DateTime time)
        {
            return time.ToUniversalTime().Subtract(UnixEpoch).TotalMilliseconds;
        }

        public string FetchData(string measId, DateTime startTime, DateTime endTime, string filePath)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
        public List<double> FetchPmuData(string measId, DateTime startTime, DateTime endTime)
        {
            List<double> res = new List<double>();
            int nret = 0;
            try
            {
                uint s = 0;
                double dval = 0;
                DateTime timestamp = DateTime.Now;
                string status = "";
                //history request initiation
                nret = History.DnaGetHistRaw(measId, startTime, endTime, out s);

                while (nret == 0)
                {
                    nret = History.DnaGetNextHist(s, out dval, out timestamp, out status);
                    if (status != null)
                    {
                        res.Add(ToMillisSinceUnixEpoch(timestamp));
                        res.Add(dval);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while fetching history results " + ex.Message);
                res = new List<double>();
            }
            return res;
        }
    }
}