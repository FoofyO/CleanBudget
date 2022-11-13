using System;
using System.Linq;
using CleanBudget.Models;
using CleanBudget.Database;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Windows;

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

        public IEnumerable<Account> GetAll()
        {
            using (var db = new BudgetContext())
            {
                return db.Accounts.Include(a => a.User)
                                  .Include(a => a.Currency)
                                  .ToList();
            }
        }

        public Account GetById(Guid id)
        {
            using (var db = new BudgetContext())
            {
                return db.Accounts.Include(a => a.User)
                                  .Include(a => a.Currency)
                                  .FirstOrDefault(u => u.Id.Equals(id));
            }
        }

        public int GetCardsCount(Guid id)
        {
            using (var db = new BudgetContext())
            {
                return db.Accounts.Include(a => a.Cards)
                                  .FirstOrDefault(u => u.Id.Equals(id)).Cards.Count;
            }
        }

        public void Update(Account item)
        {
            using (var db = new BudgetContext())
            {
                Account account = db.Accounts.FirstOrDefault(a => a.Id.Equals(item.Id));

                if (account != null)
                {
                    account.CurrencyId = item.CurrencyId;
                    db.Accounts.Update(account);
                    db.SaveChangesAsync();
                }
            }
        }

        public void UpdateCurrency(Guid id, Currency item)
        {
            using (var db = new BudgetContext())
            {
                Account account = db.Accounts.FirstOrDefault(a => a.Id.Equals(id));

                if (account != null)
                {
                    account.CurrencyId = item.Id;
                    db.Accounts.Update(account);
                    db.SaveChangesAsync();
                }
            }
        }
    }
}
