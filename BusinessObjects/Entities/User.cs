using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public string FullName { get; set; } = null!;

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = null!;

        public string? Password { get; set; }

        public AccountRole Role { get; set; } 

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? LastLoginAt { get; set; }

    }
}
