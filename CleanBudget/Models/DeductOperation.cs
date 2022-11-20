using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanBudget.Models
{
    [Table("Deducts")]
    public class DeductOperation
    {
        public Guid Id { get; set; }
        public double Amount { get; set; }
        public DateTime DateTime { get; set; }
        public Guid? CardId { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? CurrencyId { get; set; }
        public virtual Card Card { get; set; }
        public virtual Account Account { get; set; }
        public virtual Category Category { get; set; }
        public virtual Currency Currency { get; set; }

        public DeductOperation()
        {
            Id = Guid.NewGuid();
            DateTime = DateTime.Now;
        }

        public DeductOperation(double amount, Guid? cardId, Guid? accountId, Guid? currencyId, Guid? categoryId) : this()
        {
            Amount = amount;
            CardId = cardId;
            AccountId = accountId;
            CurrencyId = currencyId;
            CategoryId = categoryId;
        }
    }
}
