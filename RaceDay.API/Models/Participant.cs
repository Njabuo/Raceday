using System;
using System.Collections.Generic;

namespace RaceDay.API.Models;

public partial class Participant
{
    public int ParticipantId { get; set; }

    public int UserId { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string Gender { get; set; } = null!;

    public string EmergencyContact { get; set; } = null!;

    public string EmergencyPhone { get; set; } = null!;

    public virtual ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();

    public virtual User User { get; set; } = null!;
}
