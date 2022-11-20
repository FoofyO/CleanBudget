using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanBudget.Models
{
    [Table("Refills")]
    public class RefillOperation
    {
        public Guid Id { get; set; }
        public double Amount { get; set; }
        public DateTime DateTime { get; set; }
        public Guid? CardId { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? CurrencyId { get; set; }
        public virtual Card Card { get; set; }
        public virtual Account Account { get; set; }
        public virtual Currency Currency { get; set; }

        public RefillOperation()
        {
            Id = Guid.NewGuid();
            DateTime = DateTime.Now;
        }

        public RefillOperation(double amount, Guid cardId, Guid accountId, Guid currencyId) : this()
        {
            Amount = amount;
            CardId = cardId;
            AccountId = accountId;
            CurrencyId = currencyId;
        }
    }
}
