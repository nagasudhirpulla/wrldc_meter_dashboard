using MeterDataDashboard.Application;
using MeterDataDashboard.Core.TempHumidity;
using MeterDataDashboard.Core.TempHumidity.Services;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeterDataDashboard.Infra.Services.TempHumidity
{
    public partial class DeviceDataService : IDeviceDataService
    {
        private readonly string _mysqlConnStr;
        public DeviceDataService(IConfiguration configuration)
        {
            _mysqlConnStr = configuration["ConnectionStrings:TempMonitorConnection"];
        }

        public async Task<List<double>> GetHistDeviceData(string measTag, DateTime startTime, DateTime endTime)
        {
            List<double> res = new List<double>();
            List<string> measSegs = measTag.Split('|').ToList();
            // device name can be like Server Room|Temperature or UPS Room|Humidity
            if (measSegs.Count != 2)
            {
                return new List<double>();
            }

            string deviceName = measSegs[0];
            deviceName = deviceName.Replace("_", " ");
            string measType = measSegs[1];
            string tempMeasType = "Temperature";
            string humMeasType = "Humidity";

            if (!new List<string>() { tempMeasType, humMeasType }.Any(s => measType.ToLower().Equals(s.ToLower())))
            {
                return new List<double>();
            }

            using var connection = new MySqlConnection(_mysqlConnStr);
            await connection.OpenAsync();

            // field1 is temp, field2 is humidity
            string valColName = (measType == tempMeasType) ? "field1" : "field2";
            using var command = new MySqlCommand($@"select time, {valColName} FROM wrldc_temperature.data where 
                                                    name=@name and date between @startTime and @endTime;", connection);

            command.Parameters.AddWithValue("@name", deviceName);
            command.Parameters.AddWithValue("@startTime", startTime.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@endTime", endTime.ToString("yyyy-MM-dd HH:mm:ss"));

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                try
                {
                    var timeStr = reader.GetString(0);
                    double timeEpochMs = long.Parse(timeStr) * 1000;
                    double val = double.Parse(reader.GetString(1));
                    if (val != 0)
                    {
                        res.Add(timeEpochMs);
                        res.Add(val);
                    }
                }
                catch (Exception ex)
                {
                    // Console.WriteLine($"Error fetching device history data : {ex.Message}");
                }
                // do something with 'value'
            }
            return res;
        }

        public async Task<List<DeviceDataDTO>> GetRealTimeDevicesData()
        {
            List<DeviceDataDTO> res = new List<DeviceDataDTO>();
            //List<(string name, string ip, int port)> devices = new List<(string name, string ip, int port)>()
            //{
            //    ("Server Room", "10.2.100.72", 4567),
            //    ("Communication Room", "10.2.100.73", 4567),
            //    ("UPS Room", "10.2.100.74", 4567)
            //};
            List<(string name, string ip, int port)> devices = new List<(string name, string ip, int port)>();
            foreach ((string name, string ip, int port) in devices)
            {
                double? devTemp = DeviceDataFetcher.FetchData(ip, DataType.Temperature, port);
                double? devHum = DeviceDataFetcher.FetchData(ip, DataType.Humidity, port);
                res.Add(new DeviceDataDTO() { Humidity = devHum, Tempurature = devTemp, DeviceName = name });
            }
            return await Task.FromResult(res);
        }
    }
}
