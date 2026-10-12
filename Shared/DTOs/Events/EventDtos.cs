namespace Shared.Dtos.Events;

public sealed record EventDepartmentDto(string DepartmentId, string DepartmentRole);

public sealed record EventResponsibilityDto(string ResponsibilityType, string? UserId, string? PersonName);

public sealed record EventRiskUpsideDto(string ItemType, string Description, int SortOrder);

public sealed record SaveEventDto(
    string Name,
    string EventTypeId,
    DateOnly EventDate,
    string? Location,
    string? Venue,
    string? TargetAudience,
    string PrimaryObjectiveId,
    string? SecondaryObjectiveId,
    string? PrimaryIcpId,
    string? SecondaryIcpId,
    string? InviteMethodId,
    string? FundingSourceId,
    int NumberOfSponsors,
    string? EntertainmentTypeId,
    string? EventOwnerUserId,
    string? AccessLevels,
    string? TicketTypes,
    string? SpecialExperiences,
    IReadOnlyList<EventDepartmentDto> Departments,
    IReadOnlyList<EventResponsibilityDto> Responsibilities,
    IReadOnlyList<EventRiskUpsideDto> RisksAndUpsides);

public sealed record EventDto(
    string Id,
    string Name,
    string EventTypeId,
    DateOnly EventDate,
    string? Location,
    string? Venue,
    string? TargetAudience,
    string PrimaryObjectiveId,
    string? SecondaryObjectiveId,
    string? PrimaryIcpId,
    string? SecondaryIcpId,
    string? InviteMethodId,
    string? FundingSourceId,
    int NumberOfSponsors,
    string? EntertainmentTypeId,
    string? GoverningEntityId,
    string StatusId,
    string? EventOwnerUserId,
    string CreatedByUserId,
    DateTime CreatedAt,
    string? AccessLevels,
    string? TicketTypes,
    string? SpecialExperiences,
    IReadOnlyList<EventDepartmentDto> Departments,
    IReadOnlyList<EventResponsibilityDto> Responsibilities,
    IReadOnlyList<EventRiskUpsideDto> RisksAndUpsides);

/// <summary>Row of the event list; no child data.</summary>
public sealed record EventSummaryDto(
    string Id,
    string Name,
    string EventTypeId,
    DateOnly EventDate,
    string StatusId,
    string? EventOwnerUserId);

public sealed record EventListFilter(
    string? StatusId = null,
    DateOnly? From = null,
    DateOnly? To = null,
    string? OwnerUserId = null,
    string? SearchText = null);
