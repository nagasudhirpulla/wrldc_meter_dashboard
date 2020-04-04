using System;
using System.Threading.Tasks;

namespace MeterDataDashboard.Core.ScheduleData.Services
{
    public interface IWbesLiveDataService
    {
        Task<IsgsDownMarginsDTO> GetIsgsThermalDownMarginsForDates(DateTime fromDate, DateTime toDate);
    }
}
