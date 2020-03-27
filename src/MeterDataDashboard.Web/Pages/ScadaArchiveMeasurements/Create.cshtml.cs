using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MeterDataDashboard.Core.Entities;
using MeterDataDashboard.Infra.Persistence;
using Microsoft.AspNetCore.Authorization;
using MeterDataDashboard.Infra.Identity;

namespace MeterDataDashboard.Web.Pages.ScadaArchiveMeasurements
{
    [Authorize(Roles = SecurityConstants.AdminRoleString)]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ScadaArchiveMeasurement ScadaArchiveMeasurement { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ScadaArchiveMeasurements.Add(ScadaArchiveMeasurement);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
