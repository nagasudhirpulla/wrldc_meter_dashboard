using MeterDataDashboard.Core.ScheduleData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeterDataDashboard.Core.PmuData.Services
{
    public interface IPMUHistDataService
    {
        string FetchData(string measId, DateTime startTime, DateTime endTime);
    }
}
