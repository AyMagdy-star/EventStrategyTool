namespace Shared.Dtos.Lookups;

public sealed record LookupItemDto(
    string Id,
    string Name,
    int SortOrder,
    bool IsActive,
    string? Description = null,
    decimal? Rate = null,        // EntertainmentType.EntertainmentTaxRate
    decimal? MinScore = null,    // GoverningEntity
    decimal? MaxScore = null,    // GoverningEntity
    bool? IsPaying = null);      // TicketTierKind

public sealed record SaveLookupItemDto(
    string Name,
    int SortOrder,
    bool IsActive = true,
    string? Description = null,
    decimal? Rate = null,
    decimal? MinScore = null,
    decimal? MaxScore = null,
    bool? IsPaying = null);
