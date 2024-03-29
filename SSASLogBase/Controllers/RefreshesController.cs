﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSASLogBase.Data;
using SSASLogBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SSASLogBase.Controllers
{
    [Authorize("Skarp Rockstars")]
    public class RefreshesController : Controller
    {
        private readonly DataContext _context;

        public RefreshesController(DataContext context)
        {
            _context = context;
        }

        /// <summary> 
        /// GET: Refreshes
        /// </summary>
        /// <param name="d">Database</param>
        /// <param name="p">Page number</param>
        /// <returns></returns>
        public async Task<IActionResult> Index( [Optional] Guid d, int p = 1, int pl = 10 )
        {
            int pageSize = pl;
            List<Refresh> refreshes = new List<Refresh>();

            ViewBag.page = p;
            ViewBag.pageLength = pl;

            if (d != Guid.Parse("00000000-0000-0000-0000-000000000000") )
            {
                refreshes =  await _context.Refreshes
                    .Where(r => r.Database.ID == d)
                    .OrderByDescending(r => r.StartTime)
                    .Skip((p - 1) * pageSize)
                    .Take(pageSize)
                    .Include("Database")
                    .Include("Database.SSASServer")
                    .Include("Messages")
                    .ToListAsync();

                decimal numPages = (decimal) await _context.Refreshes.Where(r => r.Database.ID == d).CountAsync() / pageSize;
                ViewBag.numPages = Math.Floor( numPages );
                if (numPages != ViewBag.numPages) ViewBag.numPages += 1;
                ViewBag.filteredDatabaseName = refreshes.FirstOrDefault().Database.SSASServer.Name + " - " + refreshes.FirstOrDefault().Database.Name;
                ViewBag.filteredDatabaseID = refreshes.FirstOrDefault().Database.ID;
            }
            else
            {
                refreshes = await _context.Refreshes
                    .OrderByDescending(r => r.StartTime)
                    .Skip((p - 1) * pageSize)
                    .Take(pageSize)
                    .Include("Database")
                    .Include("Database.SSASServer")
                    .Include("Messages")
                    .ToListAsync();

                decimal numPages = (decimal) await _context.Refreshes.CountAsync() / pageSize;
                ViewBag.numPages = Math.Floor(numPages);
                if (numPages != ViewBag.numPages) ViewBag.numPages += 1;
            }


            return View( refreshes );
        }

        // GET: Refreshes/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            if ( id.ToString() == "00000000-0000-0000-0000-000000000000" )
            {
                return BadRequest();
            }

            Refresh refresh = await _context.Refreshes
                .Include("Database")
                .Include("Database.SSASServer")
                .Include("Messages")
                .Include("Messages.Location.SourceObject")
                .FirstOrDefaultAsync(l => l.ID == id);

            if (refresh == null) return new NotFoundResult();

            return View(refresh);
        }

        // POST: Refreshes/Create
        [AllowAnonymous] // Remove this if we can successfully authenticate with Azure AD from a Logic App HTTP POST action.
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Refresh refresh, Guid id, string server, string database)
        {
            if (ModelState.IsValid 
                && id.ToString() != "00000000-0000-0000-0000-000000000000" 
                && server != null 
                && database != null)
            {
                if ( _context.Refreshes.Any(r => r.ID == id) )
                {
                    ViewBag.Message = "That refresh is logged already.";
                    return View("PlainFeedback");
                }

                refresh.ID = id;
                SSASServer _server = _context.Servers.Include("SSASDatabases").FirstOrDefault(s => s.Name == server);
                if ( _server == null )
                {
                    _server = new SSASServer()
                    {
                        Name = server
                    };
                    refresh.Database = new SSASDatabase()
                    {
                        Name = database,
                        SSASServer = _server
                    };
                }
                else
                {
                    SSASDatabase _database = _server.SSASDatabases.FirstOrDefault(d => d.Name == database);
                    if ( _database == null)
                    {
                        _database = new SSASDatabase()
                        {
                            Name = database,
                            SSASServer = _server
                        };
                    }
                    refresh.Database = _database;
                }

                _context.Add(refresh);
                await _context.SaveChangesAsync();
            }
            else
            {
                return new BadRequestResult();
            }

            return new AcceptedResult();
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

        //private bool RefreshExists(Guid id)
        //{
        //    return _context.Refreshes.Any(e => e.ID == id);
        //}
    }
}
