using System;
using System.Collections.Generic;

namespace BusinessObjects.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Password { get; set; }

    public int Role { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }
}
