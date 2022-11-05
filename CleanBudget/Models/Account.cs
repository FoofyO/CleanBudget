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
        public virtual User User { get; set; }
        public virtual List<Card> Cards { get; set; }
        public virtual List<Category> Categories { get; set; }

        public Account()
        {
            Id = Guid.NewGuid();
            Cards = new List<Card>();
            Categories = new List<Category>();
        }

        public Account(User user) : this()
        {
            UserId = user.Id;
        }
    }
}
