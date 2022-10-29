using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanBudget.Models
{
    [Table("Accounts")]
    public class Account
    {
        public Guid  Id { get; set; }
        public Guid? UserId { get; set; }
        public virtual User User { get; set; }

        public Account() => this.Id = Guid.NewGuid();

        public Account(User user) : this()
        {
            this.User = user;
            this.UserId = user.Id;
        }
    }
}
