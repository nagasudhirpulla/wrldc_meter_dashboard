using System;
using System.Collections.Generic;
using System.Text;

namespace MeterDataDashboard.Core.TempHumidity
{
    public class DeviceDataDTO
    {
        public double? Tempurature { get; set; }
        public double? Humidity { get; set; }
        public string DeviceName { get; set; }
    }
}
