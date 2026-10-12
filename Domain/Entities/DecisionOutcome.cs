using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class DecisionOutcome
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int SortOrder { get; set; }

    public virtual ICollection<Approval> Approvals { get; set; } = new List<Approval>();

    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
}
