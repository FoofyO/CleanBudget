using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanBudget.Models
{
    [Table("Users")]
    public class User
    {
        public Guid Id { get; set; }
        [MaxLength(25)]
        public string Firstname { get; set; }
        [MaxLength(25)]
        public string Lastname { get; set; }
        [MaxLength(39)]
        public string Email { get; set; }
        [MaxLength(44)]
        public string Hash { get; set; }
        [MaxLength(36)]
        public string Salt { get; set; }
        public virtual Guid? AccountId { get; set; }
        public virtual Account Account { get; set; }

        public User() => Id = Guid.NewGuid();

        public User(string email, string firstname, string lastname, string hash, string salt) : this()
        {
            Email = email;
            Firstname = firstname;
            Lastname = lastname;
            Hash = hash;
            Salt = salt;
        }

        public void SetAccount(Account account)
        {
            AccountId = account.Id;
        }
    }
}
