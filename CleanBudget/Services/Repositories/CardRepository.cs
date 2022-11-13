using System;
using System.Linq;
using CleanBudget.Models;
using CleanBudget.Database;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CleanBudget.Services.Repositories
{
    public class CardRepository : IRepository<Card>
    {
        public void Create(Card item)
        {
            using (var db = new BudgetContext())
            {
                db.Cards.Add(item);
                db.SaveChanges();
            }
        }

        public void Delete(Card item)
        {
            using (var db = new BudgetContext())
            {
                db.Cards.Remove(item);
                db.SaveChanges();
            }
        }

        public IEnumerable<Card> GetAll()
        {
            using (var db = new BudgetContext())
            {
                return db.Cards.Include(c => c.Currency).ToList();
            }
        }

        public Card GetById(Guid id)
        {
            using (var db = new BudgetContext())
            {
                return db.Cards.Include(c => c.Currency).FirstOrDefault(u => u.Id.Equals(id));
            }
        }

        public void Update(Card item)
        {
            using (var db = new BudgetContext())
            {
                var card = db.Cards.FirstOrDefault(c => c.Id.Equals(item.Id));

                if (card != null)
                {
                    card.Title = item.Title;
                    card.Balance = item.Balance;
                    card.CurrencyId = item.CurrencyId;
                    card.Description = item.Description;
                    card.Icon = item.Icon;
                    card.Color = item.Color;
                    db.Cards.Update(card);
                    db.SaveChanges();
                }
            }
        }

        public void UpdateQueue(Guid id, int queue)
        {
            using (var db = new BudgetContext())
            {
                var card = db.Cards.FirstOrDefault(c => c.Id.Equals(id));

                if (card != null)
                {
                    card.Queue = queue;
                    db.Cards.Update(card);
                    db.SaveChanges();
                }
            }
        }
        
        public void UpdateBalance(Guid id, double balance)
        {
            using (var db = new BudgetContext())
            {
                var card = db.Cards.Include(c => c.Currency).FirstOrDefault(c => c.Id.Equals(id));

                if (card != null)
                {   
                    card.Balance = balance;
                    db.Cards.Update(card);
                    db.SaveChanges();
                }
            }
        }

        public IEnumerable<Card> GetAccountCards(Guid id)
        {
            using (var db = new BudgetContext())
            {
                return db.Cards.Include(c => c.Currency).Where(c => c.AccountId.Equals(id)).OrderBy(c => c.Queue).ToList();
            }
        }
    }
}
