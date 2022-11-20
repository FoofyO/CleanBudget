using System;
using System.Linq;
using CleanBudget.Models;
using CleanBudget.Database;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CleanBudget.Services.Repositories
{
    public class TransferOperationRepository : IRepository<TransferOperation>
    {
        public void Create(TransferOperation item)
        {
            using (var db = new BudgetContext())
            {
                db.TransferOperations.Add(item);
                db.SaveChanges();
            }
        }

        public void Delete(TransferOperation item)
        {
            using (var db = new BudgetContext())
            {
                db.TransferOperations.Remove(item);
                db.SaveChanges();
            }
        }

        public IEnumerable<TransferOperation> GetAll()
        {
            using (var db = new BudgetContext())
            {
                return db.TransferOperations.ToList();
            }
        }

        public TransferOperation GetById(Guid id)
        {
            using (var db = new BudgetContext())
            {
                return db.TransferOperations.FirstOrDefault(o => o.Id.Equals(id));
            }
        }

        public IEnumerable<TransferOperation> GetAccountOperations(Guid id)
        {
            using (var db = new BudgetContext())
            {
                return db.TransferOperations.Include(o => o.Card)
                                    .Include(o => o.Currency)
                                    .Include(o => o.Receiver)
                                    .Where(o => o.AccountId.Equals(id))
                                    .OrderBy(o => o.DateTime)
                                    .ToList();
            }
        }

        public void Update(TransferOperation item)
        {
            using (var db = new BudgetContext())
            {
                TransferOperation operation = db.TransferOperations.FirstOrDefault(o => o.Id.Equals(item.Id));

                if (operation != null)
                {
                    db.TransferOperations.Update(operation);
                    db.SaveChangesAsync();
                }
            }
        }
    }
}