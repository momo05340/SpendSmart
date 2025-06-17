using Microsoft.AspNetCore.Mvc;
using SpendSmart.Models;

namespace SpendSmart.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        // Injecting the AppDBContext to interact with the database
        private readonly AppDBContext _context;

        public AdminController(ILogger<AdminController> logger, AppDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
             ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.UserRole = HttpContext.Session.GetString("userrole");// Default to "User" if null
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("userrole");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(role) || role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var roles = _context.Roles.ToList();
            var users = _context.Users.ToList();

            var model = Tuple.Create(roles, users);
            return View(model);


        }


        [HttpGet]
        public IActionResult AdminPanel()

        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole) || userRole != "Admin")
            {
                return RedirectToAction("Index", "Admin"); // Or show unauthorized page
            }

            var roles = _context.Roles.ToList();
            var users = _context.Users.ToList();

            var model = Tuple.Create(roles, users);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(Role role)
        {
            if (ModelState.IsValid)
            {
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AdminPanel");
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AdminPanel");
        }
    }
}
