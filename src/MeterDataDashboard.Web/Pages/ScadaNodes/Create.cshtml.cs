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

namespace MeterDataDashboard.Web.Pages.ScadaNodes
{
    [Authorize(Roles = SecurityConstants.AdminRoleString)]
    public class CreateModel : PageModel
    {
        private readonly MeterDataDashboard.Infra.Persistence.AppDbContext _context;

        public CreateModel(MeterDataDashboard.Infra.Persistence.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ScadaNode ScadaNode { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ScadaNodes.Add(ScadaNode);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
