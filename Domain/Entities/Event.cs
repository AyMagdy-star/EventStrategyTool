using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Event
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string EventTypeId { get; set; } = null!;

    public DateOnly EventDate { get; set; }

    public string? Location { get; set; }

    public string? Venue { get; set; }

    public string? TargetAudience { get; set; }

    public string PrimaryObjectiveId { get; set; } = null!;

    public string? SecondaryObjectiveId { get; set; }

    public string? PrimaryIcpId { get; set; }

    public string? SecondaryIcpId { get; set; }

    public string? InviteMethodId { get; set; }

    public string? FundingSourceId { get; set; }

    public int NumberOfSponsors { get; set; }

    public string? EntertainmentTypeId { get; set; }

    public string? GoverningEntityId { get; set; }

    public string StatusId { get; set; } = null!;

    public string? EventOwnerUserId { get; set; }

    public string CreatedByUserId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? AccessLevels { get; set; }

    public string? TicketTypes { get; set; }

    public string? SpecialExperiences { get; set; }

    public virtual ICollection<Approval> Approvals { get; set; } = new List<Approval>();

    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual EntertainmentType? EntertainmentType { get; set; }

    public virtual ICollection<EventComment> EventComments { get; set; } = new List<EventComment>();

    public virtual ICollection<EventComparison> EventComparisons { get; set; } = new List<EventComparison>();

    public virtual ICollection<EventDepartment> EventDepartments { get; set; } = new List<EventDepartment>();

    public virtual User? EventOwnerUser { get; set; }

    public virtual ICollection<EventResponsibility> EventResponsibilities { get; set; } = new List<EventResponsibility>();

    public virtual ICollection<EventRiskUpside> EventRiskUpsides { get; set; } = new List<EventRiskUpside>();

    public virtual ICollection<EventStatusHistory> EventStatusHistories { get; set; } = new List<EventStatusHistory>();

    public virtual ICollection<EventTicketTier> EventTicketTiers { get; set; } = new List<EventTicketTier>();

    public virtual EventType EventType { get; set; } = null!;

    public virtual FundingSource? FundingSource { get; set; }

    public virtual GoverningEntity? GoverningEntity { get; set; }

    public virtual InviteMethod? InviteMethod { get; set; }

    public virtual ICollection<PnlLine> PnlLines { get; set; } = new List<PnlLine>();

    public virtual Icp? PrimaryIcp { get; set; }

    public virtual Objective PrimaryObjective { get; set; } = null!;

    public virtual Icp? SecondaryIcp { get; set; }

    public virtual Objective? SecondaryObjective { get; set; }

    public virtual EventStatus Status { get; set; } = null!;
}
