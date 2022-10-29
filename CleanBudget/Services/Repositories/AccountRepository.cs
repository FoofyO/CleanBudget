using System;
using System.Linq;
using CleanBudget.Models;
using CleanBudget.Database;
using System.Collections.Generic;

namespace CleanBudget.Services.Repositories
{
    public class AccountRepository : IRepository<Account>
    {
        public void Create(Account item)
        {
            using (var db = new BudgetContext())
            {
                db.Accounts.Add(item);
                db.SaveChanges();
            }
        }

        public void Delete(Account item)
        {
            using (var db = new BudgetContext())
            {
                db.Accounts.Remove(item);
                db.SaveChanges();
            }
        }

        public ICollection<Account> GetAll()
        {
            using (var db = new BudgetContext())
            {
                return db.Accounts.ToList();
            }
        }

        public Account GetById(string id)
        {
            using (var db = new BudgetContext())
            {
                return GetAll().FirstOrDefault(u => u.Id.Equals(id));
            }
        }

        public void Update(Account item)
        {
            using (var db = new BudgetContext())
            {
                Account account = GetAll().FirstOrDefault(a => a.Id.Equals(item.Id));

                if (account != null)
                {
                    db.Update(account);
                    db.SaveChangesAsync();
                }
            }
        }
    }
}
