using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanBudget.Models
{
    [Table("Categories")]
    public class Category
    {
        public Guid Id { get; set; }
        [MaxLength(20)]
        public string Title { get; set; }
        public double Consumption { get; set; }
        public int Queue { get; set; }
        [MaxLength(50)]
        public string Icon { get; set; }
        [MaxLength(50)]
        public string Color { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? CurrencyId { get; set; }
        public virtual Account Account { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual ICollection<DeductOperation> Operations { get; set; }

        public Category() => Id = Guid.NewGuid();

        public Category(string title, Guid? currencyId, int queue, string icon, string color, Guid? accountId) : this()
        {
            Icon = icon;
            Color = color;
            Queue = queue;
            Title = title;
            Consumption = 0;
            AccountId = accountId;
            CurrencyId = currencyId;
        }
    }
}
