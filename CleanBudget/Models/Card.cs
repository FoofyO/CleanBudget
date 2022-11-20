using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanBudget.Models
{
    [Table("Cards")]
    public class Card
    {
        public Guid Id { get; set; }
        [MaxLength(20)]
        public string Title { get; set; }
        [MaxLength(100)]
        public string Description { get; set; }
        public double Balance { get; set; }
        public Guid? CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
        public int Queue { get; set; }
        [MaxLength(50)]
        public string Icon { get; set; }
        [MaxLength(50)]
        public string Color { get; set; }
        public Guid? AccountId { get; set; }
        public virtual Account Account { get; set; }
        public virtual ICollection<DeductOperation> DeductOperations { get; set; }
        public virtual ICollection<RefillOperation> RefillOperations { get; set; }

        public Card() => Id = Guid.NewGuid();

        public Card(string title, string description, double balance, Guid? currencyId, int queue, string icon, string color, Guid? accountId) : this()
        {
            Icon = icon;
            Color = color;
            Queue = queue;
            Title = title;
            Balance = balance;
            AccountId = accountId;
            CurrencyId = currencyId;
            Description = description;
        }
    }
}
