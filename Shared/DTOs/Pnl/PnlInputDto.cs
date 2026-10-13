namespace Shared.DTOs.Pnl;

/// <summary>One ticket tier: what it costs and how many people.</summary>
public sealed record TicketLineDto(decimal UnitPrice, int Quantity);

/// <summary>
/// Everything the P&amp;L needs, mirroring the inputs on the workbook's "P&amp;L Statement" sheet.
/// Rates are fractions (0.14 means 14%).
/// </summary>
public sealed record PnlInputDto(
    IReadOnlyList<TicketLineDto> PayingTiers,       // workbook rows 10-21: up to 6 tiers
    IReadOnlyList<TicketLineDto> NonPayingTiers,    // workbook rows 26-37: direct invites and employee tickets
    decimal SponsorRevenue,
    decimal EntertainmentTaxRate,                   // from the event's entertainment type (0 for "No Entertainment")
    decimal ProductionCost,
    decimal ArtistCost,
    decimal ValidationFee,
    decimal InvitationFee,
    decimal PoliceServiceCost,
    decimal PermissionsCost,
    decimal DigitalMarketingCost,
    decimal MarketingBudget,
    decimal VatRate = 0.14m,
    decimal OtherTaxPerPayingTicket = 7.01m,
    decimal CommissionRate = 0m);                   // workbook shows 2.5% but its amount is 0
