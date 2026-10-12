using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Department
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<DepartmentAlignment> DepartmentAlignments { get; set; } = new List<DepartmentAlignment>();

    public virtual ICollection<EventComment> EventComments { get; set; } = new List<EventComment>();

    public virtual ICollection<EventDepartment> EventDepartments { get; set; } = new List<EventDepartment>();

    public virtual ICollection<Kpi> Kpis { get; set; } = new List<Kpi>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
