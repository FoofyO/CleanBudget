using System;
using System.Linq;
using CleanBudget.Models;
using CleanBudget.Database;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CleanBudget.Services.Repositories
{
    public class RefillOperationRepository : IRepository<RefillOperation>
    {
        public void Create(RefillOperation item)
        {
            using (var db = new BudgetContext())
            {
                db.RefillOperations.Add(item);
                db.SaveChanges();
            }
        }

        public void Delete(RefillOperation item)
        {
            using (var db = new BudgetContext())
            {
                db.RefillOperations.Remove(item);
                db.SaveChanges();
            }
        }

        public IEnumerable<RefillOperation> GetAll()
        {
            using (var db = new BudgetContext())
            {
                return db.RefillOperations.ToList();
            }
        }

        public RefillOperation GetById(Guid id)
        {
            using (var db = new BudgetContext())
            {
                return db.RefillOperations.FirstOrDefault(o => o.Id.Equals(id));
            }
        }

        public IEnumerable<RefillOperation> GetAccountOperations(Guid id)
        {
            using (var db = new BudgetContext())
            {
                return db.RefillOperations.Include(o => o.Card)
                                    .Include(o => o.Currency)
                                    .Where(o => o.AccountId.Equals(id))
                                    .OrderBy(o => o.DateTime)
                                    .ToList();
            }
        }

        public void Update(RefillOperation item)
        {
            using (var db = new BudgetContext())
            {
                RefillOperation operation = db.RefillOperations.FirstOrDefault(o => o.Id.Equals(item.Id));

                if (operation != null)
                {
                    db.RefillOperations.Update(operation);
                    db.SaveChangesAsync();
                }
            }
        }
    }
}