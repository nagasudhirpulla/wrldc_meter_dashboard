using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MeterDataDashboard.Core.Entities;
using MeterDataDashboard.Infra.Persistence;
using Microsoft.AspNetCore.Authorization;
using MeterDataDashboard.Infra.Identity;

namespace MeterDataDashboard.Web.Pages.ScadaNodes
{
    [Authorize(Roles = SecurityConstants.AdminRoleString)]
    public class EditModel : PageModel
    {
        private readonly MeterDataDashboard.Infra.Persistence.AppDbContext _context;

        public EditModel(MeterDataDashboard.Infra.Persistence.AppDbContext context)
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

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(ScadaNode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScadaNodeExists(ScadaNode.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ScadaNodeExists(int id)
        {
            return _context.ScadaNodes.Any(e => e.Id == id);
        }
    }
}
