using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using SpendSmart.Models;
namespace SpendSmart.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDBContext _context;

        // Constructor to inject DbContext
        public AccountController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]

        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("userrole", user.userrole ?? "User");

                // Optional: redirect based on role
                if (user.userrole == "Admin")
                    ViewBag.Username = user.Username;
                ViewBag.Email = user.Email;
                ViewBag.UserRole = user.userrole; // Default to "User" if null
                ViewBag.UserId = user.Id.ToString(); // Store Us

                return RedirectToAction("Index", "Admin");




               
                ViewBag.Username = user.Username;
                ViewBag.Email = user.Email;
                ViewBag.UserRole = user.userrole ?? "User"; // Default to "User" if null
                ViewBag.UserId = user.Id.ToString(); // Store UserId in ViewBag for later use
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Username = username;
            ViewBag.Error = "Invalid login credentials.";
            return View();
        }
       

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Add new user to the database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Registered successfully. Please log in.";
                return RedirectToAction("Login");
            }

            // Return to the Register view with validation errors
            return View(user);
        }

        // Logout action to clear session and redirect to login
        public IActionResult Logout()
        {
            // Clear the session data
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
