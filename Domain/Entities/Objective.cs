using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Objective
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Explanation { get; set; }

    public string? Rationale { get; set; }

    public decimal EmployeeAttendancePct { get; set; }

    public int TicketsPerEmployee { get; set; }

    public decimal DirectInvitesPct { get; set; }

    public virtual ICollection<Event> EventPrimaryObjectives { get; set; } = new List<Event>();

    public virtual ICollection<Event> EventSecondaryObjectives { get; set; } = new List<Event>();

    public virtual ICollection<ObjectiveBenchMark> ObjectiveBenchMarks { get; set; } = new List<ObjectiveBenchMark>();
}
