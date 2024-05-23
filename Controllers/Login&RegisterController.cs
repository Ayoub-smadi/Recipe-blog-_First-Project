using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipe__MVCProject.Models;

namespace Recipe__MVCProject.Controllers
{
    public class Login_RegisterController : Controller
    {
        private readonly ModelContext _context;

        public Login_RegisterController(ModelContext context)
        {
            _context = context;
        }

        public IActionResult LogIn()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

       


        [HttpPost]
        public async Task<IActionResult> LogIn(User userModel)
        {
            var log = await _context.Users
                                    .SingleOrDefaultAsync(x => x.Username == userModel.Username && x.Password == userModel.Password);

            if (log != null)
            {
                HttpContext.Session.SetInt32("UserID", (int)log.Userid);
                HttpContext.Session.SetInt32("RoleID", (int)log.Roleid);

                switch (log.Roleid)
                {
                    case 1:  // Assuming '1' is the RoleId for Admins
                        return RedirectToAction("Index", "Admin");

                    case 2:  // Assuming '2' is the RoleId for regular Users
                        return RedirectToAction("Index", "Home");

                    case 3:  // Assuming '3' is the RoleId for Chefs
                        return RedirectToAction("Index", "Home");  // Adjust controller and action names as necessary

                    default:
                        // Optionally handle unexpected roles or add a default case
                        return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                // Optionally add error handling for failed login attempts
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(userModel);
            }
        }

    

        [HttpPost]
        public IActionResult Register(User user, string Email, string userName, string Password, string roleType)
        {
            if (ModelState.IsValid)
            {
                user.Email = Email;  // Make sure to capture the email
                user.Username = userName;
                user.Password = Password;

                // Set RoleID based on the role type selected in the form
                if (roleType == "Register as Chef")
                {
                    user.Roleid = 3; // Chef
                }
                else if (roleType == "Register as User")
                {
                    user.Roleid = 2; // User
                }
                else
                {
                    // Handle unexpected roleType or default case
                    user.Roleid = 2; // Default to User if no proper roleType is provided
                }

                _context.Add(user);
                _context.SaveChanges();
            }

            return View();
        }

        // Logout action
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}