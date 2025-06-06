using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs
{
    public class UserResponseDto
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public AccountRole Role { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }
    }
}
