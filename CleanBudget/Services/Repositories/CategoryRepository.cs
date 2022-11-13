using System;
using CleanBudget.Models;
using System.Collections.Generic;
using CleanBudget.Database;
using System.Linq;

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
                return db.Categories.ToList();
            }
        }

        public Category GetById(Guid id)
        {
            using (var db = new BudgetContext())
            {
                return db.Categories.FirstOrDefault(u => u.Id.Equals(id));
            }
        }

        public void Update(Category item)
        {
            using (var db = new BudgetContext())
            {
                Category category = db.Categories.FirstOrDefault(a => a.Id.Equals(item.Id));

                if (category != null)
                {
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

        public void UpdateBalance(Guid id, double balance)
        {
            using (var db = new BudgetContext())
            {
                var category = db.Categories.FirstOrDefault(c => c.Id.Equals(id));

                if (category != null)
                {
                    category.Balance = balance;
                    db.Categories.Update(category);
                    db.SaveChanges();
                }
            }
        }

        public IEnumerable<Category> GetAccountCategories(Guid id)
        {
            using (var db = new BudgetContext())
            {
                return db.Categories.Where(c => c.AccountId.Equals(id)).ToList();
            }
        }
    }
}
