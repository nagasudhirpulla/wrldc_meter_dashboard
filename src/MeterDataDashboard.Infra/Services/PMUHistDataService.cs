using MeterDataDashboard.Core.PmuData.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using MeterDataDashboard.Core.PmuData;
using System.Threading.Tasks;
using MeterDataDashboard.Application;
using PMUDataAdapter;
using System.Linq;

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
            PmuAdapter adapter = new PmuAdapter();
            try
            {
                res = $"[{adapter.FetchData(measId, startTime, endTime, _configuration["pmuAdapterPath"])}]";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while fetching history results " + ex.Message);
                res = "[]";
            }
            return res;
        }
    }
}
