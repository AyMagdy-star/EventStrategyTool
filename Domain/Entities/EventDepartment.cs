using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class EventDepartment
{
    public string EventId { get; set; } = null!;

    public string DepartmentId { get; set; } = null!;

    public string DepartmentRole { get; set; } = null!;

    public virtual Department Department { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
}
