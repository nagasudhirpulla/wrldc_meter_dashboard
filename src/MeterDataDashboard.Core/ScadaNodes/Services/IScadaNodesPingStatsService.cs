using System;
using System.Collections.Generic;
using System.Text;

namespace MeterDataDashboard.Core.ScadaNodes.Services
{
    public interface IScadaNodesPingStatsService
    {
        IEnumerable<NodesPingLiveStatusDTO> FetchNodesPingLiveStatus();

        IEnumerable<double> FetchNodePingHist(string nodeName, DateTime startTime, DateTime endTime);
    }
}
