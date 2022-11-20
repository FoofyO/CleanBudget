using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanBudget.Models
{
    [Table("Currencies")]
    public class Currency
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public virtual ICollection<Card> Cards { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<DeductOperation> DeductOperations { get; set; }
        public virtual ICollection<RefillOperation> RefillOperations { get; set; }
        public virtual ICollection<TransferOperation> TransferOperations { get; set; }

        public Currency() => Id = Guid.NewGuid();

        public Currency(string fullName, string shortName) : this()
        {
            FullName = fullName;
            ShortName = shortName;
        }

        public override string ToString()
        {
            return $"{ShortName} | {FullName}";
        }
    }
}
