using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recipe__MVCProject.Models;

namespace Recipe__MVCProject.Controllers
{
    public class RecipesalesController : Controller
    {
        private readonly ModelContext _context;

        public RecipesalesController(ModelContext context)
        {
            _context = context;
        }

        // GET: Recipesales
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Recipesales.Include(r => r.Recipe).Include(r => r.User);
            return View(await modelContext.ToListAsync());
        }


        // GET: Recipesales/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Recipesales == null)
            {
                return NotFound();
            }

            var recipesale = await _context.Recipesales
                .Include(r => r.Recipe)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Saleid == id);
            if (recipesale == null)
            {
                return NotFound();
            }

            return View(recipesale);
        }

        // GET: Recipesales/Create
        public IActionResult Create()
        {
            ViewData["Recipeid"] = new SelectList(_context.Recipes, "Recipeid", "Recipeid");
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid");
            return View();
        }

        // POST: Recipesales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Saleid,Userid,Recipeid,Saledate,Amount")] Recipesale recipesale)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recipesale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Recipeid"] = new SelectList(_context.Recipes, "Recipeid", "Recipeid", recipesale.Recipeid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", recipesale.Userid);
            return View(recipesale);
        }

        // GET: Recipesales/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Recipesales == null)
            {
                return NotFound();
            }

            var recipesale = await _context.Recipesales.FindAsync(id);
            if (recipesale == null)
            {
                return NotFound();
            }
            ViewData["Recipeid"] = new SelectList(_context.Recipes, "Recipeid", "Recipeid", recipesale.Recipeid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", recipesale.Userid);
            return View(recipesale);
        }

        // POST: Recipesales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Saleid,Userid,Recipeid,Saledate,Amount")] Recipesale recipesale)
        {
            if (id != recipesale.Saleid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipesale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipesaleExists(recipesale.Saleid))
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
            ViewData["Recipeid"] = new SelectList(_context.Recipes, "Recipeid", "Recipeid", recipesale.Recipeid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", recipesale.Userid);
            return View(recipesale);
        }

        // GET: Recipesales/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Recipesales == null)
            {
                return NotFound();
            }

            var recipesale = await _context.Recipesales
                .Include(r => r.Recipe)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Saleid == id);
            if (recipesale == null)
            {
                return NotFound();
            }

            return View(recipesale);
        }

        // POST: Recipesales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Recipesales == null)
            {
                return Problem("Entity set 'ModelContext.Recipesales'  is null.");
            }
            var recipesale = await _context.Recipesales.FindAsync(id);
            if (recipesale != null)
            {
                _context.Recipesales.Remove(recipesale);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipesaleExists(decimal id)
        {
            return (_context.Recipesales?.Any(e => e.Saleid == id)).GetValueOrDefault();
        }
        [HttpGet]
        public IActionResult SalesReport()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SalesReport(DateTime startDate, DateTime endDate, string reportType)
        {
            if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
            {
                return BadRequest("Start date and end date are required.");
            }

            if (reportType == "Monthly")
            {
                return RedirectToAction("MonthlyReport", new { startDate, endDate });
            }
            else if (reportType == "Annual")
            {
                return RedirectToAction("AnnualReport", new { startDate, endDate });
            }

            return BadRequest("Invalid report type.");
        }

        // GET: Recipesales/MonthlyReport
        public async Task<IActionResult> MonthlyReport(DateTime startDate, DateTime endDate)
        {
            var sales = await _context.Recipesales
                .Include(s => s.Recipe) 
                .Include(s => s.User)   
                .Where(r => r.Saledate >= startDate && r.Saledate <= endDate)
                .GroupBy(r => new { r.Saledate.Value.Year, r.Saledate.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalAmount = g.Sum(x => x.Amount),
                    Sales = g.ToList()
                })
                .ToListAsync();

            if (sales == null || sales.Count == 0)
            {
                ViewBag.Message = "No sales data found for the specified period.";
            }

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(sales);
        }

        // GET: Recipesales/AnnualReport
        public async Task<IActionResult> AnnualReport(DateTime startDate, DateTime endDate)
        {
            var sales = await _context.Recipesales
                .Include(r => r.Recipe) 
                .Include(r => r.User)  
                .Where(r => r.Saledate >= startDate && r.Saledate <= endDate)
                .GroupBy(r => r.Saledate.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    TotalAmount = g.Sum(x => x.Amount),
                    Sales = g.ToList()
                })
                .ToListAsync();

            if (sales == null || sales.Count == 0)
            {
                ViewBag.Message = "No sales data found for the specified period.";
            }

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(sales);
        }

        public ActionResult GenerateReport()
        {
            DateTime startDate = DateTime.Now.AddDays(-30); 
            DateTime endDate = DateTime.Now; 

            var sales = _context.Recipesales
                .Include(r => r.Recipe) 
                .Include(r => r.User)   
                .Where(r => r.Saledate >= startDate && r.Saledate <= endDate)
                .ToList();

            MemoryStream memoryStream = new MemoryStream();
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

         
            Paragraph title = new Paragraph("Sales Report");
            title.Alignment = Element.ALIGN_CENTER;
            document.Add(title);

           
            PdfPTable table = new PdfPTable(4);
            table.WidthPercentage = 100;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 10f;

            PdfPCell cell = new PdfPCell(new Phrase("Sales Data"));
            cell.Colspan = 4;
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);

            table.AddCell("Sale Date");
            table.AddCell("Amount");
            table.AddCell("Recipe");
            table.AddCell("User");

            foreach (var sale in sales)
            {
                table.AddCell(sale.Saledate.ToString());
                table.AddCell(sale.Amount.ToString());
                table.AddCell(sale.Recipe?.Name ?? "N/A"); 
                table.AddCell(sale.User?.Username ?? "N/A");
            }

            document.Add(table);

            document.Close();
            writer.Close();

            return File(memoryStream.ToArray(), "application/pdf", "SalesReport.pdf");
        }


        public async Task<IActionResult> SalesChart(DateTime startDate, DateTime endDate)
        {
            var sales = await _context.Recipesales
                .Where(r => r.Saledate >= startDate && r.Saledate <= endDate)
                .GroupBy(r => new { r.Saledate.Value.Year, r.Saledate.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalAmount = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            if (sales == null || sales.Count == 0)
            {
                ViewBag.Message = "No sales data found for the specified period.";
            }

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(sales);
        }



    }
}
