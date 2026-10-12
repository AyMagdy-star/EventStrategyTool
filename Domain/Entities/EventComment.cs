using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class EventComment
{
    public string Id { get; set; } = null!;

    public string EventId { get; set; } = null!;

    public string DepartmentId { get; set; } = null!;

    public string Comment { get; set; } = null!;

    public string? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
}
