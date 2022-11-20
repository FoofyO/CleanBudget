using System;
using System.Linq;
using CleanBudget.Models;
using CleanBudget.Database;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CleanBudget.Services.Repositories
{
    public class DeductOperationRepository : IRepository<DeductOperation>
    {
        public void Create(DeductOperation item)
        {
            using (var db = new BudgetContext())
            {
                db.DeductOperations.Add(item);
                db.SaveChanges();
            }
        }

        public void Delete(DeductOperation item)
        {
            using (var db = new BudgetContext())
            {
                db.DeductOperations.Remove(item);
                db.SaveChanges();
            }
        }

        public IEnumerable<DeductOperation> GetAll()
        {
            using (var db = new BudgetContext())
            {
                return db.DeductOperations.ToList();
            }
        }

        public DeductOperation GetById(Guid id)
        {
            using (var db = new BudgetContext())
            {
                return db.DeductOperations.FirstOrDefault(o => o.Id.Equals(id));
            }
        }

        public IEnumerable<DeductOperation> GetAccountOperations(Guid id)
        {
            using (var db = new BudgetContext())
            {
                return db.DeductOperations.Include(o => o.Card)
                                    .Include(o => o.Currency)
                                    .Include(o => o.Category)
                                    .Where(o => o.AccountId.Equals(id))
                                    .OrderBy(o => o.DateTime)
                                    .ToList();
            }
        }

        public void Update(DeductOperation item)
        {
            using (var db = new BudgetContext())
            {
                DeductOperation operation = db.DeductOperations.FirstOrDefault(o => o.Id.Equals(item.Id));

                if (operation != null)
                {
                    db.DeductOperations.Update(operation);
                    db.SaveChangesAsync();
                }
            }
        }
    }
}