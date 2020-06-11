using System;
using System.Collections.Generic;
using System.Text;

namespace MeterDataDashboard.Core.ScadaNodes
{
    public class NodesPingLiveStatusDTO
    {

        public string NodeName { get; set; }
        public string NodeIp { get; set; }
        public DateTime StatusTime { get; set; }
        public int Status { get; set; }
    }
}
