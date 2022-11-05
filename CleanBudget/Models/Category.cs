using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanBudget.Models
{
    [Table("Categories")]
    public class Category
    {
        public Guid Id { get; set; }
        [MaxLength(50)]
        public string Title { get; set; }
        public double Balance { get; set; }
        [MaxLength(50)]
        public string Currency { get; set; }
        [MaxLength(50)]
        public string Icon { get; set; }
        [MaxLength(50)]
        public string Color { get; set; }
        public int Queue { get; set; }
        public Guid? AccountId { get; set; }
        public virtual Account Account { get; set; }

        public Category()
        {
            Id = Guid.NewGuid();
            Queue = 0;
        }

        public Category(string title, double balance, string currency, string icon, string color, Account account) : this()
        {
            Icon = icon;
            Title = title;
            Color = color;
            Balance = balance;
            Account = account;
            Currency = currency;
            AccountId = account.Id;
        }
    }
}
