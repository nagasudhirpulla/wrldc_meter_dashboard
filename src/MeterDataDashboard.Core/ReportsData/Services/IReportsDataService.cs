using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MeterDataDashboard.Core.ReportsData.Services
{
    public interface IReportsDataService
    {
        Task<List<PspMeasurement>> GetAllMeasurements();
        Task<List<double>> GetMeasurementData(string measLabel, DateTime startTime, DateTime endTime);
    }
}
