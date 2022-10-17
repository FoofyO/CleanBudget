using CleanBudget.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanBudget.Database
{
    public class BudgetContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public BudgetContext() : base("SqlConnection")
        {

        }
    }
}
