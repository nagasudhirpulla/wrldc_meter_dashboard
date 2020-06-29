﻿using MeterDataDashboard.Core.TempHumidity;
using MeterDataDashboard.Core.TempHumidity.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeterDataDashboard.Infra.Services.TempHumidity
{
    public partial class DeviceDataService : IDeviceDataService
    {
        public async Task<List<DeviceDataDTO>> GetRealTimeDevicesData()
        {
            List<DeviceDataDTO> res = new List<DeviceDataDTO>();
            List<(string name, string ip, int port)> devices = new List<(string name, string ip, int port)>()
            {
                ("Server Room", "10.2.100.72", 4567),
                ("Communication Room", "10.2.100.73", 4567),
                ("UPS Room", "10.2.100.74", 4567)
            };
            foreach (var dev in devices)
            {
                double? devTemp = DeviceDataFetcher.FetchData(dev.ip, DataType.Temperature, dev.port);
                double? devHum = DeviceDataFetcher.FetchData(dev.ip, DataType.Humidity, dev.port);
                res.Add(new DeviceDataDTO() { Humidity = devHum, Tempurature = devTemp, DeviceName = dev.name });
            }
            return await Task.FromResult(res);
        }
    }
}