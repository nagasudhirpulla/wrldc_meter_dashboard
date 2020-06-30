using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MeterDataDashboard.Core.TempHumidity.Services
{
    public interface IDeviceDataService
    {
        Task<List<DeviceDataDTO>> GetRealTimeDevicesData();
        Task<List<double>> GetHistDeviceData(string measTag, DateTime startTime, DateTime endTime);
    }
}
