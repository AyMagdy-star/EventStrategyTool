namespace Shared.DTOs.Pnl;

/// <summary>Calculated P&amp;L. Ratios are null when their denominator is zero.</summary>
public sealed record PnlResultDto(
    int TotalPayingAttendees,
    int TotalNonPayingAttendees,

    decimal GrossTicketRevenue,
    decimal SponsorRevenue,
    decimal TotalRevenue,
    decimal InviteRevenueLoss,
    decimal NetRevenue,              // workbook "Total Revenue - Revenue loss from Invites"

    decimal Vat,
    decimal EntertainmentTax,
    decimal OtherTax,
    decimal TotalTax,

    decimal VenueCost,               // production + artist
    decimal Commission,
    decimal TicketingFees,           // commission + validation + invitation
    decimal OtherCosts,              // police + permissions + digital marketing
    decimal TotalCost,

    decimal Profit,
    decimal? ProfitMargin,           // profit / net revenue
    decimal ProfitInclMarketing,     // profit + marketing budget
    decimal? RoiInclMarketing);      // (net revenue + marketing budget - cost) / cost
