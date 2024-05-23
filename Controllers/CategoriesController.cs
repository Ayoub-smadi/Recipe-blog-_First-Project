using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recipe__MVCProject.Models;

namespace Recipe__MVCProject.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _WebHostEnvironment;


        public CategoriesController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _WebHostEnvironment = webHostEnvironment;

        }



        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return _context.Categories != null ?
                        View(await _context.Categories.ToListAsync()) :
                        Problem("Entity set 'ModelContext.Categories'  is null.");
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Categoryid == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Categoryid,Categoryname,ImageFile")] Category category)
        {
            if (category.ImageFile != null)
            {
                string wwwrootpath = _WebHostEnvironment.WebRootPath;
                string fileName = Path.GetFileName(category.ImageFile.FileName);  // Get the actual file name
                string imageName = Guid.NewGuid().ToString() + "_" + fileName;  // Append the GUID to the original file name
                string fullpath = Path.Combine(wwwrootpath + "/Images/", imageName);

                using (var filestream = new FileStream(fullpath, FileMode.Create))
                {
                    await category.ImageFile.CopyToAsync(filestream);
                }

                category.Categoryimage = imageName;
            }

            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Categoryid,Categoryname,ImageFile")] Category category)
        {
            if (id != category.Categoryid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (category.ImageFile != null)
                {
                    string wwwrootpath = _WebHostEnvironment.WebRootPath;
                    string fileName = Path.GetFileName(category.ImageFile.FileName);  // Get the actual file name
                    string imageName = Guid.NewGuid().ToString() + "_" + fileName;  // Append the GUID to the original file name
                    string fullpath = Path.Combine(wwwrootpath + "/Images/", imageName);

                    // Delete the old image file if it exists
                    if (!string.IsNullOrEmpty(category.Categoryimage))
                    {
                        string oldImagePath = Path.Combine(wwwrootpath, "Images", category.Categoryimage);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var filestream = new FileStream(fullpath, FileMode.Create))
                    {
                        await category.ImageFile.CopyToAsync(filestream);
                    }

                    category.Categoryimage = imageName;
                }

                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Categoryid))
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
            return View(category);
        }


        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Categoryid == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ModelContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(decimal id)
        {
            return (_context.Categories?.Any(e => e.Categoryid == id)).GetValueOrDefault();
        }
    }
}