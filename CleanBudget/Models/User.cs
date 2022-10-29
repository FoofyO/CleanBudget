using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanBudget.Models
{
    [Table("Users")]
    public class User
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public virtual Account Account { get; set; }

        public User() { }

        public User(string email, string firstname, string lastname, string hash, string salt)
        {
            this.Id = Guid.NewGuid();
            this.Email = email;
            this.Firstname = firstname;
            this.Lastname = lastname;
            this.Hash = hash;
            this.Salt = salt;
        }

        public void SetAccount(Account account) => this.Account = account;
    }
}
