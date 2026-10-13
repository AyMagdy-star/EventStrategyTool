namespace Shared.DTOs.Lookups;

/// <summary>The reference lists served by ILookupService (W1). Users have their own IUserService.</summary>
public enum LookupKind
{
    EventType,
    InviteMethod,
    FundingSource,
    Department,
    Icp,
    GoverningEntity,
    EntertainmentType,
    TicketTierKind,
    DecisionOutcome,
    EventStatus,
    Role
}
