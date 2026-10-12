using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class DepartmentAlignment
{
    public string Id { get; set; } = null!;

    public string AssessmentId { get; set; } = null!;

    public string KpiId { get; set; } = null!;

    public string DepartmentId { get; set; } = null!;

    public bool IsAligned { get; set; }

    public string? AlignedByUserId { get; set; }

    public DateTime? AlignedAt { get; set; }

    public string? Comment { get; set; }

    public virtual User? AlignedByUser { get; set; }

    public virtual Assessment Assessment { get; set; } = null!;

    public virtual Department Department { get; set; } = null!;

    public virtual Kpi Kpi { get; set; } = null!;
}
