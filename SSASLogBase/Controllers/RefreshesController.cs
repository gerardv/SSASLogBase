using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSASLogBase.Data;
using SSASLogBase.Models;

namespace SSASLogBase.Controllers
{
    public class RefreshesController : Controller
    {
        private readonly DataContext _context;

        public RefreshesController(DataContext context)
        {
            _context = context;
        }

        // GET: Refreshes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Refreshes.ToListAsync());
        }

        // GET: Refreshes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var refresh = await _context.Refreshes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (refresh == null)
            {
                return NotFound();
            }

            return View(refresh);
        }

        // POST: Refreshes/Create
        [HttpPost]
        public async Task<int> Create([FromBody] Refresh refresh, Guid id, string server, string database)
        {
            if (ModelState.IsValid 
                && id.ToString() != "00000000-0000-0000-0000-000000000000" 
                && server != null 
                && database != null)
            {
                refresh.ID = id;
                refresh.Database = new SSASDatabase()
                {
                    Name = database,
                    SSASServer = new SSASServer()
                    {
                        Name = server
                    }
                };
                _context.Add(refresh);
                await _context.SaveChangesAsync();

                return 1;
            }

            return 0;
        }

        // GET: Refreshes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var refresh = await _context.Refreshes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (refresh == null)
            {
                return NotFound();
            }

            return View(refresh);
        }

        // POST: Refreshes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var refresh = await _context.Refreshes.FindAsync(id);
            _context.Refreshes.Remove(refresh);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RefreshExists(Guid id)
        {
            return _context.Refreshes.Any(e => e.ID == id);
        }
    }
}
