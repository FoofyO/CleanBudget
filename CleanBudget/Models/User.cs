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
        [Column("Secondname")]
        public string Secondname { get; set; }

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
    }
}
