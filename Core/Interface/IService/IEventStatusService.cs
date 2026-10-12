using Shared.Dtos.Events;
using Shared.Results;

namespace Core.Status;

public interface IEventStatusService
{
    Task<Result> TransitionAsync(string eventId, string toStatusId, string? comment = null, CancellationToken ct = default);

    Task<Result<IReadOnlyList<string>>> GetAllowedTransitionsAsync(string eventId, CancellationToken ct = default);

    Task<Result<IReadOnlyList<StatusHistoryDto>>> GetHistoryAsync(string eventId, CancellationToken ct = default);
}
