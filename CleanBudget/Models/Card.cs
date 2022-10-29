using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CleanBudget.Models
{
    [Table("Cards")]
    public class Card
    {
        public Guid Id { get; set; }
        public double Balance { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public string Currency { get; set; }
        public Guid AccountId { get; set; }
        public virtual Account Account { get; set; }

        public Card() { }

        public Card(double balance)
        {
            this.Id = Guid.NewGuid();
            this.Balance = balance;
        }
    }
}
