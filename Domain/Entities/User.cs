using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class User
{
    public string Id { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? DepartmentId { get; set; }

    public string RoleId { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? LoginName { get; set; }

    public virtual ICollection<Approval> Approvals { get; set; } = new List<Approval>();

    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();

    public virtual Department? Department { get; set; }

    public virtual ICollection<DepartmentAlignment> DepartmentAlignments { get; set; } = new List<DepartmentAlignment>();

    public virtual ICollection<EventComment> EventComments { get; set; } = new List<EventComment>();

    public virtual ICollection<EventComparison> EventComparisonActualEnteredByUsers { get; set; } = new List<EventComparison>();

    public virtual ICollection<EventComparison> EventComparisonExpectedEnteredByUsers { get; set; } = new List<EventComparison>();

    public virtual ICollection<Event> EventCreatedByUsers { get; set; } = new List<Event>();

    public virtual ICollection<Event> EventEventOwnerUsers { get; set; } = new List<Event>();

    public virtual ICollection<EventResponsibility> EventResponsibilities { get; set; } = new List<EventResponsibility>();

    public virtual ICollection<EventStatusHistory> EventStatusHistories { get; set; } = new List<EventStatusHistory>();

    public virtual Role Role { get; set; } = null!;
}
