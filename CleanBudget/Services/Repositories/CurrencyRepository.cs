using System;
using System.Linq;
using CleanBudget.Models;
using CleanBudget.Database;
using System.Collections.Generic;

namespace CleanBudget.Services.Repositories
{
    public class CurrencyRepository : IRepository<Currency>
    {
        public void Create(Currency item)
        {
            using (var db = new BudgetContext())
            {
                db.Currencies.Add(item);
                db.SaveChanges();
            }
        }

        public void Delete(Currency item)
        {
            using (var db = new BudgetContext())
            {
                db.Currencies.Remove(item);
                db.SaveChanges();
            }
        }

        public IEnumerable<Currency> GetAll()
        {
            using (var db = new BudgetContext())
            {
                return db.Currencies.OrderBy(c => c.FullName).ToList();
            }
        }

        public Currency GetById(Guid id)
        {
            using (var db = new BudgetContext())
            {
                return db.Currencies.FirstOrDefault(c => c.Id.Equals(id));
            }
        }

        public void Update(Currency item)
        {
            using (var db = new BudgetContext())
            {
                Currency currency = db.Currencies.FirstOrDefault(c => c.Id.Equals(item.Id));

                if (currency != null)
                {
                    currency.FullName = item.FullName;
                    currency.ShortName = item.ShortName;
                    db.Currencies.Update(currency);
                    db.SaveChangesAsync();
                }
            }
        }

        public Currency GetDollar()
        {
            using (var db = new BudgetContext())
            {
                return db.Currencies.FirstOrDefault(c => c.ShortName.Equals("USD"));
            }
        }
    }
}
