using Shared.DTOs.Lookups;
using Shared.Results;

namespace Core.Lookups;


public interface ILookupReader
{
    Task<Result<IReadOnlyList<LookupItemDto>>> GetActiveAsync(
        LookupKind kind, string? includeId = null, CancellationToken ct = default);

    Task<Result<LookupItemDto>> GetAsync(LookupKind kind, string id, CancellationToken ct = default);
}

public interface ILookupService : ILookupReader
{
    Task<Result<IReadOnlyList<LookupItemDto>>> GetAllAsync(LookupKind kind, CancellationToken ct = default);

    Task<Result<LookupItemDto>> CreateAsync(LookupKind kind, SaveLookupItemDto item, CancellationToken ct = default);

    Task<Result<LookupItemDto>> UpdateAsync(LookupKind kind, string id, SaveLookupItemDto item, CancellationToken ct = default);

    Task<Result> DeleteAsync(LookupKind kind, string id, CancellationToken ct = default);
}
