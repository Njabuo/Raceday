using System;
using System.Collections.Generic;

namespace RaceDay.API.Models;

public partial class Organiser
{
    public int OrganiserId { get; set; }

    public int UserId { get; set; }

    public string OrganisationName { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual User User { get; set; } = null!;
}
