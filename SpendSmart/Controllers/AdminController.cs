using Microsoft.AspNetCore.Mvc;
using SpendSmart.Models;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;


namespace SpendSmart.Controllers
{
    [Authorize]

    public class AdminController : Controller
    {
        private readonly AppDBContext _context;

        public AdminController(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Get user ID from claims
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); // fallback protection
            }

            // Group expenses by month for the current user
            var groupedData = _context.Expenses
    .Where(e => e.UserId == userId)
    .GroupBy(e => e.DateCreated.Month)
    .Select(g => new
    {
        MonthNumber = g.Key,
        Total = g.Sum(e => e.Value)
    })
    .ToList()
    .OrderBy(g => g.MonthNumber) // safer than parsing strings
    .Select(g => new
    {
        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.MonthNumber),
        Total = g.Total
    })
    .ToList();


            // Pass data to view
            ViewBag.Months = groupedData.Select(g => g.Month).ToList();
            ViewBag.MonthlyExpenses = groupedData.Select(g => g.Total).ToList();
            var users = _context.Users.ToList(); // Ensure this line fetches your users
            return View(users);


        }

        [HttpPost]
        public async Task<IActionResult> CreateEditUserForm(User user)
        {
            if (!ModelState.IsValid)
            {
                // Optionally return with error message
                return RedirectToAction("Users");
            }

            // If Id is 0, treat as new user
            if (user.Id != 0)
            {
                user.DateCreated = DateTime.Now;
                user.DateUpdated = DateTime.Now;
                _context.Users.Add(user);
            }
            else
            {
                var existingUser = await _context.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                // Update fields
                existingUser.Username = user.Username;
                existingUser.Email = user.Email;
                existingUser.Password = user.Password;
                existingUser.userrole = user.userrole;
                existingUser.DateUpdated = user.DateUpdated;
                existingUser.DateCreated = user.DateCreated;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Admin"); // Assumes your user list view is "Users"
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Users"); // Assumes your user list view is named "Users"
        }


    }
}
