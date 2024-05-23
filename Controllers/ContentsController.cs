using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recipe__MVCProject.Models;

namespace Recipe__MVCProject.Controllers
{
    public class ContentsController : Controller
    {
        private readonly ModelContext _context;

        public ContentsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Contents
        public async Task<IActionResult> Index()
        {
              return _context.Contents != null ? 
                          View(await _context.Contents.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Contents'  is null.");
        }

        // GET: Contents/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Contents == null)
            {
                return NotFound();
            }

            var content = await _context.Contents
                .FirstOrDefaultAsync(m => m.Contentid == id);
            if (content == null)
            {
                return NotFound();
            }

            return View(content);
        }

        // GET: Contents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Contentid,Name,Key,Value")] Content content)
        {
            if (ModelState.IsValid)
            {
                _context.Add(content);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(content);
        }

        // GET: Contents/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Contents == null)
            {
                return NotFound();
            }

            var content = await _context.Contents.FindAsync(id);
            if (content == null)
            {
                return NotFound();
            }
            return View(content);
        }

        // POST: Contents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Contentid,Name,Key,Value")] Content content)
        {
            if (id != content.Contentid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(content);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContentExists(content.Contentid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(content);
        }

        // GET: Contents/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Contents == null)
            {
                return NotFound();
            }

            var content = await _context.Contents
                .FirstOrDefaultAsync(m => m.Contentid == id);
            if (content == null)
            {
                return NotFound();
            }

            return View(content);
        }

        // POST: Contents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Contents == null)
            {
                return Problem("Entity set 'ModelContext.Contents'  is null.");
            }
            var content = await _context.Contents.FindAsync(id);
            if (content != null)
            {
                _context.Contents.Remove(content);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContentExists(decimal id)
        {
          return (_context.Contents?.Any(e => e.Contentid == id)).GetValueOrDefault();
        }
    }
}
