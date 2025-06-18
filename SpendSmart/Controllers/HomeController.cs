using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Models;

namespace SpendSmart.Controllers
{
    [Authorize] 
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _context;

        public HomeController(ILogger<HomeController> logger, AppDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        private string? Username => User.Identity?.Name;
        private string? UserId => User.FindFirst("UserId")?.Value;
        private string? Email => User.FindFirst(ClaimTypes.Email)?.Value;

        public IActionResult Index()
        {
            ViewBag.Username = Username;
            ViewBag.Email = Email;

            var allexpenses = _context.Expenses
                .Where(e => e.UserId == Username)
                .ToList();

            var allbudgets = _context.Budgets
                .Where(b => b.UserId == Username)
                .ToList();

            ViewBag.Expenses = allexpenses.Sum(e => e.Value);
            ViewBag.Count = allexpenses.Count;
            ViewBag.Budget = allbudgets.Sum(b => b.Value);
            ViewBag.TotalNatira = ViewBag.Budget - ViewBag.Expenses;
            ViewData["Breadcrumbs"] = "Dashboard";

            var data = _context.Expenses
                .GroupBy(e => e.Description)
                .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Value) })
                .ToList();

            ViewBag.categories = data.Select(d => d.Category).ToList();
            ViewBag.EXP = data.Select(d => d.Total).ToList();

            return View();
        }

        public IActionResult Expenses()
        {
            var expenses = _context.Expenses
                .Where(e => e.UserId == Username)
                .ToList();

            ViewBag.Expenses = expenses.Sum(e => e.Value);
            ViewBag.Count = expenses.Count;
            ViewBag.Username = Username;

            return View(expenses);
        }

        public IActionResult Budget()
        {
            var budgets = _context.Budgets
                .Where(b => b.UserId == Username)
                .ToList();

            ViewBag.Budget = budgets.Sum(b => b.Value);
            ViewBag.Count = budgets.Count;
            ViewBag.UserId = Username;

            return View(budgets);
        }

        public IActionResult CreateEditExpense(int? id)
        {
            var model = id.HasValue
                ? _context.Expenses.SingleOrDefault(e => e.Id == id)
                : new Expense { UserId = Username };

            return View(model);
        }

        [HttpPost]
        public IActionResult CreateEditExpenseForm(Expense model)
        {
            if (model.Id == 0)
                _context.Expenses.Add(model);
            else
                _context.Expenses.Update(model);

            _context.SaveChanges();
            return RedirectToAction("Expenses");
        }

        [HttpPost]
        public IActionResult DeleteExpense(int id)
        {
            var item = _context.Expenses.FirstOrDefault(e => e.Id == id);
            if (item == null) return NotFound();

            _context.Expenses.Remove(item);
            _context.SaveChanges();

            return RedirectToAction("Expenses");
        }

        public IActionResult BudgetCrud(int? id)
        {
            var model = id.HasValue
                ? _context.Budgets.SingleOrDefault(b => b.Id == id)
                : new Budget { UserId = Username };

            return View(model);
        }

        public IActionResult CreateEditBudgetForm(Budget model)
        {
            if (model.Id == 0)
                _context.Budgets.Add(model);
            else
                _context.Budgets.Update(model);

            _context.SaveChanges();
            return RedirectToAction("Budget");
        }

        public IActionResult DeleteBudget(int id)
        {
            var budget = _context.Budgets.SingleOrDefault(b => b.Id == id);
            if (budget != null)
            {
                _context.Budgets.Remove(budget);
                _context.SaveChanges();
            }

            return RedirectToAction("Budget");
        }

        public IActionResult Privacy() => View();

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
