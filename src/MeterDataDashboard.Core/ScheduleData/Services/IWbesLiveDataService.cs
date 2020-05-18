using System;
using System.Threading.Tasks;

namespace MeterDataDashboard.Core.ScheduleData.Services
{
    public interface IWbesLiveDataService
    {
        Task<IsgsMarginsDTO> GetIsgsThermalDownMarginsForDates(DateTime fromDate, DateTime toDate);
        Task<IsgsMarginsDTO> GetIsgsThermalUpMarginsForDates(DateTime fromDate, DateTime toDate);
    }
}
