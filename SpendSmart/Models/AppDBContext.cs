using Microsoft.EntityFrameworkCore;

namespace SpendSmart.Models
{
    public class AppDBContext : DbContext
    {

        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {

        }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }  

        public class HomeViewModel
        {
            public string Username { get; set; }
            public string Email { get; set; }
        }

        

    }   


        
}
