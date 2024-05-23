using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recipe__MVCProject.Models;

namespace Recipe__MVCProject.Controllers
{
    public class RecipesController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RecipesController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Recipes/Search
        [HttpGet]
        public async Task<IActionResult> Search(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                return BadRequest("Start date and end date are required.");
            }

            DateTime startOfDay = startDate.Value.Date;
            DateTime endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);

            var recipes = await _context.Recipes
                .Where(r => r.Addedtime >= startOfDay && r.Addedtime <= endOfDay)
                .Include(r => r.Category)
                .Include(r => r.User)
                .ToListAsync();

            var urlHelper = Url;
            var builder = new StringBuilder();
            foreach (var item in recipes)
            {
                var editLink = urlHelper.Action("Edit", "Recipes", new { id = item.Recipeid });
                var detailsLink = urlHelper.Action("Details", "Recipes", new { id = item.Recipeid });
                var deleteLink = urlHelper.Action("Delete", "Recipes", new { id = item.Recipeid });

                builder.AppendLine("<tr class='recipe-item'>");
                builder.AppendLine($"<td>{item.Name}</td>");
                builder.AppendLine($"<td>{item.Description}</td>");
                builder.AppendLine($"<td>{item.Ingredients}</td>");
                builder.AppendLine($"<td>{item.Instructions}</td>");
                string formattedDate = item.Addedtime.HasValue ? item.Addedtime.Value.ToString("MM-dd-yyyy") : "N/A";
                builder.AppendLine($"<td>{formattedDate}</td>");
                builder.AppendLine($"<td><img src='~/RecipeImages/{item.Mainimage}' style='border-radius:20%;' width='100' height='100' /></td>");
                builder.AppendLine($"<td>{item.Price}</td>");
                builder.AppendLine($"<td>{item.Category.Categoryid}</td>");
                builder.AppendLine($"<td>{item.User.Userid}</td>");
                builder.AppendLine($"<td><a href='{editLink}'>Edit</a> | <a href='{detailsLink}'>Details</a> | <a href='{deleteLink}'>Delete</a></td>");
                builder.AppendLine("</tr>");
            }
            return Content(builder.ToString(), "text/html");
        }

        // GET: Recipes
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Recipes.Include(r => r.Category).Include(r => r.User);
            return View(await modelContext.ToListAsync());
        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Recipeid == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // GET: Recipes/Create
        public IActionResult Create()
        {
            ViewData["Categoryid"] = new SelectList(_context.Categories, "Categoryid", "Categoryid");
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid");
            return View();
        }

        // POST: Recipes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Recipeid,Userid,Name,Description,Ingredients,Instructions,Categoryid,Addedtime,ImageFile,Price")] Recipe recipe)
        {
            if (recipe.ImageFile != null)
            {
                string wwwrootpath = _webHostEnvironment.WebRootPath;
                string fileName = Path.GetFileName(recipe.ImageFile.FileName);
                string imageName = Guid.NewGuid().ToString() + "_" + fileName;
                string fullpath = Path.Combine(wwwrootpath + "/RecipeImages/", imageName);

                using (var filestream = new FileStream(fullpath, FileMode.Create))
                {
                    await recipe.ImageFile.CopyToAsync(filestream);
                }

                recipe.Mainimage = imageName;
            }
            if (ModelState.IsValid)
            {
                _context.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Categoryid"] = new SelectList(_context.Categories, "Categoryid", "Categoryid", recipe.Categoryid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", recipe.Userid);
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Recipeid,Userid,Name,Description,Ingredients,Instructions,Categoryid,Addedtime,ImageFile,Price")] Recipe recipe)
        {
            if (id != recipe.Recipeid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (recipe.ImageFile != null)
                    {
                        string wwwrootpath = _webHostEnvironment.WebRootPath;
                        string fileName = Path.GetFileName(recipe.ImageFile.FileName);
                        string imageName = Guid.NewGuid().ToString() + "_" + fileName;
                        string fullpath = Path.Combine(wwwrootpath + "/RecipeImages/", imageName);

                        if (!string.IsNullOrEmpty(recipe.Mainimage))
                        {
                            string oldImagePath = Path.Combine(wwwrootpath, "RecipeImages", recipe.Mainimage);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        using (var filestream = new FileStream(fullpath, FileMode.Create))
                        {
                            await recipe.ImageFile.CopyToAsync(filestream);
                        }

                        recipe.Mainimage = imageName;
                    }

                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.Recipeid))
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
            ViewData["Categoryid"] = new SelectList(_context.Categories, "Categoryid", "Categoryid", recipe.Categoryid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", recipe.Userid);
            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(decimal id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                if (!string.IsNullOrEmpty(recipe.Mainimage))
                {
                    string wwwrootpath = _webHostEnvironment.WebRootPath;
                    string imagePath = Path.Combine(wwwrootpath, "RecipeImages", recipe.Mainimage);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(decimal id)
        {
            return _context.Recipes.Any(e => e.Recipeid == id);
        }
    }
}