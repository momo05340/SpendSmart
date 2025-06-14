using Microsoft.AspNetCore.Mvc;
using SpendSmart.Models;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;


namespace SpendSmart.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        // Injecting the AppDBContext to interact with the database
        private readonly AppDBContext _context; 
        public HomeController(ILogger<HomeController> logger,AppDBContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.Email = HttpContext.Session.GetString("Email");
            var username = HttpContext.Session.GetString("Username");

            //var allexpenses = _context.Expenses.ToList();
           // var allbudgets = _context.Budgets.ToList();

            //Filter expes and budgets by the current user
            var allexpenses = _context.Expenses
               .Where(e => e.UserId == username) // or e.Username, depending on your model
               .ToList();

            var allbudgets = _context.Budgets
               .Where(b => b.UserId == username) // Adjust property name if needed
               .ToList();



            // Calculate total expenses and count of products
            var countprod = allexpenses.Count;
            var totalExpenses = allexpenses.Sum(expense => expense.Value);
            var totalBudgets = allbudgets.Sum(budget => budget.Value);  

            var totalnatira = totalBudgets - totalExpenses;



            ViewBag.Expenses = totalExpenses;
            ViewBag.Count = countprod;
            ViewBag.Budget = totalBudgets;
            ViewBag.TotalNatira = totalnatira; // Total remaining after expenses
            ViewData["Expenses"] = totalExpenses;
            ViewData["Breadcrumbs"] = "Dashboard";


            var categories = _context.Expenses
            .Select(e => e.Description)         // Adjust to your actual column
            .Distinct()
            .OrderBy(c => c)
            .ToList();

            ViewBag.Category = categories;

            var exp = _context.Expenses
            .Select(e => e.Value)         // Adjust to your actual column
            .Distinct()
            .OrderBy(c => c)
            .ToList();

            ViewBag.EXP = exp;

            var data = _context.Expenses
                .GroupBy(e => e.Description)
                .Select(g => new {
                 Category = g.Key,
                Total = g.Sum(e => e.Value)
    })
    .ToList();

            ViewBag.categories = data.Select(d => d.Category).ToList();
            ViewBag.EXP = data.Select(d => d.Total).ToList();

            return View();
          
        }


        //public IActionResult Expenses()
        //{
        //    var allexpenses = _context.Expenses.ToList();
        //    // Calculate total expenses and count of products
        //    var countprod = allexpenses.Count;  
        //    var totalExpenses = allexpenses.Sum(expense => expense.Value);

        //    ViewBag.Expenses = totalExpenses;
        //    ViewBag.Count = countprod;
        //    ViewData["Expenses"] = totalExpenses;
        //    ViewBag.username = HttpContext.Session.GetString("Username");

        //    return View(allexpenses);
        //}
        public IActionResult Expenses()
        {
            // 1. Get username from session
            var username = HttpContext.Session.GetString("Username");

            // 2. Filter only expenses belonging to that user
            var allexpenses = _context.Expenses
                .Where(e => e.UserId == username) // or e.Username, depending on your model
                .ToList();

            // 3. Calculate based on filtered data
            var countprod = allexpenses.Count;
            var totalExpenses = allexpenses.Sum(expense => expense.Value);

            // 4. Pass data to the view
            ViewBag.Expenses = totalExpenses;
            ViewBag.Count = countprod;
            ViewBag.Username = username;

            return View(allexpenses);
        }




        public IActionResult Budget()
        {


            // Get the logged-in username from session
            var currentUser = HttpContext.Session.GetString("Username");
            ViewBag.UserId = currentUser;

            // Filter only the budgets that belong to the current user
            var allexpenses = _context.Budgets
                .Where(b => b.UserId == currentUser) // Adjust property name if needed
                .ToList();

            // Calculate total budget and count of items for the current user
            var countprod = allexpenses.Count;
            var totalExpenses = allexpenses.Sum(expense => expense.Value);

            // Pass data to view
            ViewBag.Budget = totalExpenses;
            ViewBag.Count = countprod;

            return View(allexpenses);
        }
        //public IActionResult Budget()
        //{
        //    ViewBag.UserId = HttpContext.Session.GetString("Username");

        //    var allexpenses = _context.Budgets.ToList();

        //    // Calculate total expenses and count of products
        //    var countprod = allexpenses.Count;
        //    var totalExpenses = allexpenses.Sum(expense => expense.Value);

        //    ViewBag.Budget = totalExpenses;
        //    ViewBag.Count = countprod;
        //    ViewBag.UserId = HttpContext.Session.GetString("Username");

        //    return View(allexpenses);
        //}
        [HttpPost]
        public IActionResult DeleteExpense(int id)
        {
            
            var expense = _context.Expenses.FirstOrDefault(e => e.Id == id);

            if (expense == null)
            {
                
                return NotFound();
            }

            _context.Expenses.Remove(expense);
            _context.SaveChanges();

           
            return RedirectToAction("Expenses");
        }

        public IActionResult CreateEditExpense(int? id)
        {
            

            if(id != null)
            {
                //editing an existing expense
                var expenseInDb = _context.Expenses.SingleOrDefault(expense => expense.Id == id);
                ViewBag.UserId = HttpContext.Session.GetString("Username");
                return View(expenseInDb);
            }
            else
            {
                ViewBag.UserId = HttpContext.Session.GetString("Username");
            }
                return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult CreateEditExpenseForm(Expense model)
        {
           

            if (model.Id == 0)
            {
                //Create
                _context.Expenses.Add(model);
            }
            else
            {
                //Editing
                _context.Expenses.Update(model);

            }

            //_context.Expenses.Add(model);

            _context.SaveChanges();

            return RedirectToAction("Expenses");
        }

        public IActionResult BudgetCrud(int? id)
        {


            if (id != null)
            {
                //editing an existing expense
                var BudgetInDb = _context.Budgets.SingleOrDefault(Budget => Budget.Id == id);
                return View(BudgetInDb);
            }
            else
            {
                ViewBag.Username = HttpContext.Session.GetString("Username");


            }
            return View();
        }

        public IActionResult CreateEditBudgetForm(Budget model1)
        {
            if (model1.Id == 0)
            {
                //Create
                _context.Budgets.Add(model1);
            }
            else
            {
                //Editing
                _context.Budgets.Update(model1);

            }

            //_context.Expenses.Add(model);

            _context.SaveChanges();

            return RedirectToAction("Budget");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]



        public IActionResult DeleteBudget(int id)
        {
           
            // Find the expense by id   
            var expenseInDb = _context.Budgets.SingleOrDefault(expense => expense.Id == id);

            _context.Budgets.Remove(expenseInDb);
            _context.SaveChanges();

            return RedirectToAction("Budget");
        }







        public IActionResult CreateEditBudget(int? id)
        {


            if (id != null)
            {
                //editing an existing expense
                var expenseInDb = _context.Budgets.SingleOrDefault(expense => expense.Id == id);
                ViewBag.UserId = HttpContext.Session.GetString("Username");
                return View(expenseInDb);
            }
            else
            {
                ViewBag.UserId = HttpContext.Session.GetString("Username");
            }
            return View();
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
