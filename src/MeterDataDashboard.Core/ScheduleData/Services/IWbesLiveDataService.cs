using System;
using System.Threading.Tasks;

namespace MeterDataDashboard.Core.ScheduleData.Services
{
    public interface IWbesLiveDataService
    {
        Task<IsgsSchedulesDTO> GetIsgsThermalDownMarginsForDates(DateTime fromDate, DateTime toDate);
        Task<IsgsSchedulesDTO> GetIsgsThermalUpMarginsForDates(DateTime fromDate, DateTime toDate);
        Task<IsgsSchedulesDTO> GetIsgsRrasForDates(DateTime fromDate, DateTime toDate);
        Task<IsgsSchedulesDTO> GetIsgsScedForDates(DateTime fromDate, DateTime toDate);
    }
}
