using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipe__MVCProject.Models;

namespace Recipe__MVCProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly ModelContext _context;
        public AdminController(ModelContext context)
        {
            _context = context;
            
        }



        public IActionResult Index()
        {
            // Retrieve data for ViewBag
            ViewBag.Categories = _context.Categories;
            ViewBag.Recipes = _context.Recipes.ToList();
            ViewBag.Payments = _context.Payments;
            ViewBag.registereduserCount = _context.Users.Where(x => x.Roleid == 2).Count();
            ViewBag.registerchefCount = _context.Users.Where(x => x.Roleid == 3).Count();
            ViewBag.recipycount = _context.Recipes.Count();
          

            // Retrieve user ID from session
            var id = HttpContext.Session.GetInt32("UserID");
            if (id == null)
            {
                // Handle case where user ID is not found in session (e.g., redirect to login)
                return RedirectToAction("LogIn", "Login_Register");
            }

            // Retrieve user from database
            var user = _context.Users.SingleOrDefault(x => x.Userid == id);
            if (user == null)
            {
                // Handle case where user is not found (e.g., redirect to error page)
                return RedirectToAction("Error", "Home");
            }

            // Pass user model to view
            return View(user);
        }


        public async Task<IActionResult> Approve(decimal id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            recipe.Status = Recipe.RecipeStatus.Approved;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reject(decimal id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            recipe.Status = Recipe.RecipeStatus.Rejected;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
