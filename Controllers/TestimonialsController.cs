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
    public class TestimonialsController : Controller
    {
        private readonly ModelContext _context;

        public TestimonialsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Testimonials
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Testimonials.Include(t => t.Recipe).Include(t => t.User);
            return View(await modelContext.ToListAsync());
        }

        // GET: Testimonials/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Testimonials == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials
                .Include(t => t.Recipe)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Testimonialid == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }
        // GET: Testimonials/Create
        public IActionResult Create()
        {
            // استرجع قيمة UserId من الجلسة وضعها في ViewBag.userid
            ViewBag.userid = HttpContext.Session.GetInt32("UserID");

            // قم بتحميل قائمة الوصفات وأعرضها في العرض
            ViewData["Recipeid"] = new SelectList(_context.Recipes, "Recipeid", "Name");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Testimonialcontent,Recipeid,Userid,Testimonialdate")] Testimonial testimonial)
        {
            if (ModelState.IsValid)
            {
                // استرجع قيمة UserId من الجلسة وضعها في UserId
                testimonial.Userid = HttpContext.Session.GetInt32("UserID").Value;

                // إذا لم يتم تعيين Testimonialdate من النموذج، قم بتعيينه إلى التاريخ الحالي
                if (testimonial.Testimonialdate == null)
                {
                    testimonial.Testimonialdate = DateTime.Now;
                }

                // جلب الوصفة من قاعدة البيانات باستخدام معرف الوصفة
                var recipe = await _context.Recipes.FindAsync(testimonial.Recipeid);
                if (recipe != null)
                {
                    // تعيين الوصفة في الشهادة
                    testimonial.Recipe = recipe;
                }

                // جلب المستخدم من قاعدة البيانات باستخدام معرف المستخدم
                var user = await _context.Users.FindAsync(testimonial.Userid);
                if (user != null)
                {
                    // تعيين اسم المستخدم في الشهادة
                    testimonial.Userid = user.Userid;
                }

                // إضافة الشهادة الجديدة إلى قاعدة البيانات
                _context.Add(testimonial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // إذا كان هناك أخطاء في النموذج، أعد عرض النموذج مع البيانات المطلوبة
            ViewData["Recipeid"] = new SelectList(_context.Recipes, "Recipeid", "Name", testimonial.Recipeid);
            return View(testimonial);
        }




        // GET: Testimonials/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Testimonials == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }
            ViewData["Recipeid"] = new SelectList(_context.Recipes, "Recipeid", "Recipeid", testimonial.Recipeid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", testimonial.Userid);
            return View(testimonial);
        }

        // POST: Testimonials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Testimonialid,Userid,Recipeid,Testimonialcontent,Testimonialdate")] Testimonial testimonial)
        {
            if (id != testimonial.Testimonialid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(testimonial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestimonialExists(testimonial.Testimonialid))
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
            ViewData["Recipeid"] = new SelectList(_context.Recipes, "Recipeid", "Recipeid", testimonial.Recipeid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", testimonial.Userid);
            return View(testimonial);
        }

        // GET: Testimonials/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Testimonials == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials
                .Include(t => t.Recipe)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Testimonialid == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        // POST: Testimonials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Testimonials == null)
            {
                return Problem("Entity set 'ModelContext.Testimonials'  is null.");
            }
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial != null)
            {
                _context.Testimonials.Remove(testimonial);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestimonialExists(decimal id)
        {
          return (_context.Testimonials?.Any(e => e.Testimonialid == id)).GetValueOrDefault();
        }
    }
}
