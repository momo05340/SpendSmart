using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpendSmart.Models;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
namespace SpendSmart.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDBContext _context;
        public DashboardController(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Example: Assume you have an Expenses table with a Date and Amount field
            var groupedData = _context.Expenses
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
