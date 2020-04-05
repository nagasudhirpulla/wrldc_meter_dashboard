using MeterDataDashboard.Core.PmuData.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using MeterDataDashboard.Core.PmuData;
using System.Threading.Tasks;
using PMUDataAdapter;

namespace MeterDataDashboard.Infra.Services
{
    public class PMUHistDataService : IPMUHistDataService
    {
        private PmuAdapter PmuAdapter_ { get; set; }

        public PMUHistDataService(PmuConfig pmuConfig)
        {
            PmuAdapter_ = new PmuAdapter();
        }

        public async Task<List<double>> FetchData(int measId, DateTime startTime, DateTime endTime)
        {
            List<double> res = await PmuAdapter_.FetchData(measId, startTime, endTime);
            return res;
        }
    }
}
