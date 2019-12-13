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
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ScadaArchiveMeasurement = await _context.ScadaArchiveMeasurements.FindAsync(id);

            if (ScadaArchiveMeasurement != null)
            {
                _context.ScadaArchiveMeasurements.Remove(ScadaArchiveMeasurement);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
