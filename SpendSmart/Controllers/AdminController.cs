using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpendSmart.Models;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
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

            return View();
        }
    }
}
