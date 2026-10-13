using Shared.DTOs.Events;
using Shared.Results;

namespace Core.Events;

public interface IEventService
{
    Task<Result<EventDto>> GetAsync(string eventId, CancellationToken ct = default);

    Task<Result<PagedResult<EventSummaryDto>>> ListAsync(
        EventListFilter filter, PageRequest page, CancellationToken ct = default);

    Task<Result<EventDto>> CreateAsync(SaveEventDto dto, CancellationToken ct = default);

    Task<Result<EventDto>> UpdateAsync(string eventId, SaveEventDto dto, CancellationToken ct = default);

    Task<Result> SetGoverningEntityAsync(string eventId, string governingEntityId, CancellationToken ct = default);
}
