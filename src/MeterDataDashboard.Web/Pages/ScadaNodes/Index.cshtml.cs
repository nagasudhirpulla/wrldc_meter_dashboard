using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MeterDataDashboard.Core.Entities;
using MeterDataDashboard.Infra.Persistence;
using Microsoft.AspNetCore.Authorization;

namespace MeterDataDashboard.Web.Pages.ScadaNodes
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly MeterDataDashboard.Infra.Persistence.AppDbContext _context;

        public IndexModel(MeterDataDashboard.Infra.Persistence.AppDbContext context)
        {
            _context = context;
        }

        public IList<ScadaNode> ScadaNode { get;set; }

        public async Task OnGetAsync()
        {
            ScadaNode = await _context.ScadaNodes.ToListAsync();
        }
    }
}
