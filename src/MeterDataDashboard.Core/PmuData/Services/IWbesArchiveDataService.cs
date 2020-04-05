using MeterDataDashboard.Core.ScheduleData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeterDataDashboard.Core.PmuData.Services
{
    public interface IPMUHistDataService
    {
        Task<List<double>> FetchData(int measId, DateTime startTime, DateTime endTime);
    }
}
