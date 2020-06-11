using System.Collections.Generic;
using MeterDataDashboard.Core.ScadaNodes;
using MeterDataDashboard.Core.ScadaNodes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MeterDataDashboard.Web.Pages.ScadaNodes
{
    [Authorize]
    public class LiveStatusModel : PageModel
    {
        private readonly IScadaNodesPingStatsService _scadaNodesService;

        public LiveStatusModel(IScadaNodesPingStatsService scadaNodesService)
        {
            _scadaNodesService = scadaNodesService;
        }

        public IEnumerable<NodesPingLiveStatusDTO> LiveNodesStatus_ { get; set; }

        public void OnGet()
        {
            // get the live nodes ping status
            LiveNodesStatus_ = _scadaNodesService.FetchNodesPingLiveStatus();
        }
    }
}