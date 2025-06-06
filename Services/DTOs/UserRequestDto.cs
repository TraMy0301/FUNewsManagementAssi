using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs
{
    public class UserRequestDto
    {

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string? Password { get; set; }

        public AccountRole Role { get; set; }
        public bool IsActive { get; set; }

    }
}
