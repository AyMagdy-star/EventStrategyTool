namespace Shared.DTOs.Events;

public sealed record StatusHistoryDto(
    string Id,
    string EventId,
    string? FromStatusId,
    string ToStatusId,
    string ChangedByUserId,
    DateTime ChangedAt,
    string? Comment);
