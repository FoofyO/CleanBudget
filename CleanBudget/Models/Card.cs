using System;
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

        public Card()
        {
            Id = Guid.NewGuid();
        }

        public Card(string title, string description, double balance, Guid? currencyId, int queue, string icon, string color, Guid? accountId) : this()
        {
            Title = title;
            Description = description;
            Balance = balance;
            CurrencyId = currencyId;
            Queue = queue;
            Icon = icon;
            Color = color;
            AccountId = accountId;
        }
    }
}
