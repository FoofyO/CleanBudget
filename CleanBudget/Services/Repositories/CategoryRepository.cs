using System;
using CleanBudget.Models;
using System.Collections.Generic;
using CleanBudget.Database;
using System.Linq;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;

namespace CleanBudget.Services.Repositories
{
    public class CategoryRepository : IRepository<Category>
    {
        public void Create(Category item)
        {
            using (var db = new BudgetContext())
            {
                db.Categories.Add(item);
                db.SaveChanges();
            }
        }

        public void Delete(Category item)
        {
            using (var db = new BudgetContext())
            {
                db.Categories.Remove(item);
                db.SaveChanges();
            }
        }

        public IEnumerable<Category> GetAll()
        {
            using (var db = new BudgetContext())
            {
                return db.Categories.Include(c => c.Currency).ToList();
            }
        }

        public Category GetById(Guid id)
        {
            using (var db = new BudgetContext())
            {
                return db.Categories.Include(c => c.Currency).FirstOrDefault(c => c.Id.Equals(id));
            }
        }

        public void Update(Category item)
        {
            using (var db = new BudgetContext())
            {
                var category = db.Categories.FirstOrDefault(c => c.Id.Equals(item.Id));

                if (category != null)
                {
                    category.Icon = item.Icon;
                    category.Color = item.Color;
                    category.Title = item.Title;
                    category.Consumption = item.Consumption;
                    category.CurrencyId = item.CurrencyId;
                    db.Categories.Update(category);
                    db.SaveChangesAsync();
                }
            }
        }

        public void UpdateQueue(Guid id, int queue)
        {
            using (var db = new BudgetContext())
            {
                var category = db.Categories.FirstOrDefault(c => c.Id.Equals(id));

                if (category != null)
                {
                    category.Queue = queue;
                    db.Categories.Update(category);
                    db.SaveChanges();
                }
            }
        }

        public void UpdateConsumption(Guid id, double consumption)
        {
            using (var db = new BudgetContext())
            {
                var category = db.Categories.FirstOrDefault(c => c.Id.Equals(id));

                if (category != null)
                {
                    category.Consumption = consumption;
                    db.Categories.Update(category);
                    db.SaveChanges();
                }
            }
        }

        public IEnumerable<Category> GetAccountCategories(Guid id)
        {
            using (var db = new BudgetContext())
            {
                return db.Categories.Include(c => c.Currency)
                                    .Where(c => c.AccountId.Equals(id))
                                    .OrderBy(c => c.Queue)
                                    .ToList();
            }
        }
    }
}
