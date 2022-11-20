using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanBudget.Models
{
    [Table("Transfers")]
    public class TransferOperation
    {
        public Guid Id { get; set; }
        public double Amount { get; set; }
        public DateTime DateTime { get; set; }
        public Guid? CardId { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? CurrencyId { get; set; }
        public Guid? ReceiverId { get; set; }
        public virtual Card Card { get; set; }
        public virtual Account Account { get; set; }
        public virtual Card Receiver { get; set; }
        public virtual Currency Currency { get; set; }

        public TransferOperation() 
        {
            Id = Guid.NewGuid();
            DateTime = DateTime.Now;
        }

        public TransferOperation(double amount, Guid cardId, Guid accountId, Guid currencyId, Guid receiverId) : this()
        {
            Amount = amount;
            CardId = cardId;
            AccountId = accountId;
            CurrencyId = currencyId;
            ReceiverId = receiverId;
        }
    }
}
