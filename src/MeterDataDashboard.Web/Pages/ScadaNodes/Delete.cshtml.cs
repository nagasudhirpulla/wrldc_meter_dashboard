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

namespace MeterDataDashboard.Web.Pages.ScadaNodes
{
    [Authorize(Roles = SecurityConstants.AdminRoleString)]
    public class DeleteModel : PageModel
    {
        private readonly MeterDataDashboard.Infra.Persistence.AppDbContext _context;

        public DeleteModel(MeterDataDashboard.Infra.Persistence.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ScadaNode ScadaNode { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ScadaNode = await _context.ScadaNodes.FirstOrDefaultAsync(m => m.Id == id);

            if (ScadaNode == null)
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

            ScadaNode = await _context.ScadaNodes.FindAsync(id);

            if (ScadaNode != null)
            {
                _context.ScadaNodes.Remove(ScadaNode);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
