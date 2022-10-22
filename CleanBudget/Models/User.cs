using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanBudget.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [Column("Firstname")]
        public string Firstname { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [Column("Lastname")]
        public string Lastname { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [Column("Email")]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        [Column("Hash")]
        public string Hash { get; set; }

        [Required]
        [Column("Salt")]
        public string Salt { get; set; }

        public User()
        {

        }

        public User(string email, string firstname, string lastname, string hash, string salt)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Email = email;
            this.Firstname = firstname;
            this.Lastname = lastname;
            this.Hash = hash;
            this.Salt = salt;
        }
    }
}
