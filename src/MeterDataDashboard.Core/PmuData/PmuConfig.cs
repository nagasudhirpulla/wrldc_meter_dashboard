using System;
using System.Collections.Generic;
using System.Text;

namespace MeterDataDashboard.Core.PmuData
{
    public class PmuConfig
    {
        public string AdapterPath { get; set; } = @".\adapter.exe";
        public string Host { get; set; } = "172.16.183.131";
        public int Port { get; set; } = 24721;
        public string Path { get; set; } = "/eterra-ws/HistoricalDataProvider";
        public string Username { get; set; } = "pdcAdmin";
        public string Password { get; set; } = "p@ssw0rd";
        public int RefMeasId { get; set; } = 2127;
        public int DataRate { get; set; } = 25;
    }
}
