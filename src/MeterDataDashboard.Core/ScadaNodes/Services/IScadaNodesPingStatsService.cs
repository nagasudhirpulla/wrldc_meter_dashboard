using System;
using System.Collections.Generic;
using System.Text;

namespace MeterDataDashboard.Core.ScadaNodes.Services
{
    public interface IScadaNodesPingStatsService
    {
        IEnumerable<NodesPingLiveStatusDTO> FetchNodesPingLiveStatus();
    }
}
