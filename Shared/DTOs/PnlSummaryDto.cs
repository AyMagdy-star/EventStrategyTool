namespace Shared.DTOs.Pnl;

public enum PnlBasis { Expected, Actual }

public sealed record PnlSummaryDto(
    string EventId,
    PnlBasis Basis,
    decimal Revenue,
    decimal Cost,
    decimal MarketingBudget,
    decimal Profit,
    decimal? RoiPure,
    decimal? RoiInclMarketing);
