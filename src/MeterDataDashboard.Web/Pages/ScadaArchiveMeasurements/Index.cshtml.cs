using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MeterDataDashboard.Core.Entities;
using MeterDataDashboard.Infra.Persistence;

namespace MeterDataDashboard.Web.Pages.ScadaArchiveMeasurements
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<ScadaArchiveMeasurement> ScadaArchiveMeasurement { get; set; }

        public async Task OnGetAsync()
        {
            ScadaArchiveMeasurement = await _context.ScadaArchiveMeasurements.ToListAsync();
        }
    }
}
