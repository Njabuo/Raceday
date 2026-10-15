using System;
using System.Collections.Generic;

namespace RaceDay.API.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Organiser? Organiser { get; set; }

    public virtual Participant? Participant { get; set; }
}
