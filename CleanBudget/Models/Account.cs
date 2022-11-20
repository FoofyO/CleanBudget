using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanBudget.Models
{
    [Table("Accounts")]
    public class Account
    {
        public Guid  Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? CurrencyId { get; set; }
        public virtual User User { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual ICollection<Card> Cards { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<DeductOperation> DeductOperations { get; set; }
        public virtual ICollection<RefillOperation> RefillOperations { get; set; }
        public virtual ICollection<TransferOperation> TransferOperations { get; set; }

        public Account() => Id = Guid.NewGuid();

        public Account(User user, Currency currency) : this()
        {
            UserId = user.Id;
            CurrencyId = currency.Id;
        }
    }
}
