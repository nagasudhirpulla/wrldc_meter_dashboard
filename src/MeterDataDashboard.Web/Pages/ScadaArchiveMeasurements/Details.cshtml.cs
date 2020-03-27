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
using MeterDataDashboard.Infra.Identity;

namespace MeterDataDashboard.Web.Pages.ScadaArchiveMeasurements
{
    [Authorize(Roles = SecurityConstants.AdminRoleString)]
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public ScadaArchiveMeasurement ScadaArchiveMeasurement { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ScadaArchiveMeasurement = await _context.ScadaArchiveMeasurements.FirstOrDefaultAsync(m => m.Id == id);

            if (ScadaArchiveMeasurement == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
