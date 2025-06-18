using Microsoft.AspNetCore.Mvc;
using SpendSmart.Models;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SpendSmart.Controllers
{
    [Authorize(AuthenticationSchemes = "MyCookieScheme")]
   
    public class Dashboard1Controller : Controller
    {
        private readonly AppDBContext _context;

        public Dashboard1Controller(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var groupedData = _context.Expenses
                .Where(e => e.UserId == userId)
                .GroupBy(e => e.DateCreated.Month)
                .Select(g => new
                {
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                    Total = g.Sum(e => e.Value)
                })
                .OrderBy(g => DateTime.ParseExact(g.Month, "MMMM", CultureInfo.CurrentCulture))
                .ToList();

            ViewBag.Months = groupedData.Select(g => g.Month).ToList();
            ViewBag.MonthlyExpenses = groupedData.Select(g => g.Total).ToList();

            return View();
        }
    }
}
