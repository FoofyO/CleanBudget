using System;
using System.Linq;
using System.Collections.Generic;
using CleanBudget.Database;
using CleanBudget.Models;

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
                return db.Cards.ToList();
            }
        }

        public Card GetById(Guid id)
        {
            using (var db = new BudgetContext())
            {
                return GetAll().FirstOrDefault(u => u.Id.Equals(id));
            }
        }

        public void Update(Card item)
        {
            using (var db = new BudgetContext())
            {
                var card = GetAll().FirstOrDefault(c => c.Id.Equals(item.Id));

                if (card != null)
                {
                    card.Title = item.Title;
                    card.Balance = item.Balance;
                    card.Currency = item.Currency;
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
                var card = GetAll().FirstOrDefault(c => c.Id.Equals(id));

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
                var card = GetAll().FirstOrDefault(c => c.Id.Equals(id));

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
                return db.Cards.Where(c => c.AccountId.Equals(id)).ToList();
            }
        }
    }
}
