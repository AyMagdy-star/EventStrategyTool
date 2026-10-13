using Core.Interface;
using Core.Interface.IService;
using Core.Lookups;
using Core.Rules;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Lookups;
using Shared.Results;

namespace Core.Services;

internal sealed class LookupService : ILookupService
{
    private const int MaxDescriptionLength = 500;
    private const int SqlForeignKeyViolation = 547;

    private readonly IUnitOfWorkFactory _uowFactory;

    public LookupService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

    // ---- reads ----

    public async Task<Result<IReadOnlyList<LookupItemDto>>> GetActiveAsync(
        LookupKind kind, string? includeId = null, CancellationToken ct = default)
    {
        await using var uow = _uowFactory.Create();
        var items = await LookupDefinitions.Get(kind).ListAsync(uow, activeOnly: true, includeId, ct);
        return Result.Success<IReadOnlyList<LookupItemDto>>(items);
    }

    public async Task<Result<IReadOnlyList<LookupItemDto>>> GetAllAsync(LookupKind kind, CancellationToken ct = default)
    {
        await using var uow = _uowFactory.Create();
        var items = await LookupDefinitions.Get(kind).ListAsync(uow, activeOnly: false, includeId: null, ct);
        return Result.Success<IReadOnlyList<LookupItemDto>>(items);
    }

    public async Task<Result<LookupItemDto>> GetAsync(LookupKind kind, string id, CancellationToken ct = default)
    {
        await using var uow = _uowFactory.Create();
        var item = await LookupDefinitions.Get(kind).FindAsync(uow, id, ct);
        return item is null ? NotFound(kind, id) : item;
    }

    // ---- writes ----

    public async Task<Result<LookupItemDto>> CreateAsync(LookupKind kind, SaveLookupItemDto item, CancellationToken ct = default)
    {
        var def = LookupDefinitions.Get(kind);
        if (!def.AllowCreateDelete)
            return Error.Forbidden("lookup.fixed_list", $"{kind} entries are fixed and cannot be added.");

        var (dto, error) = Normalize(def, item);
        if (error is not null) return error;

        await using var uow = _uowFactory.Create();
        if (await def.NameExistsAsync(uow, dto.Name, excludeId: null, ct))
            return DuplicateName(kind, dto.Name);

        return await def.AddAsync(uow, dto, ct);
    }

    public async Task<Result<LookupItemDto>> UpdateAsync(
        LookupKind kind, string id, SaveLookupItemDto item, CancellationToken ct = default)
    {
        var def = LookupDefinitions.Get(kind);

        var (dto, error) = Normalize(def, item);
        if (error is not null) return error;

        await using var uow = _uowFactory.Create();
        if (await def.NameExistsAsync(uow, dto.Name, excludeId: id, ct))
            return DuplicateName(kind, dto.Name);

        var updated = await def.UpdateAsync(uow, id, dto, ct);
        return updated is null ? NotFound(kind, id) : updated;
    }

    public async Task<Result> DeleteAsync(LookupKind kind, string id, CancellationToken ct = default)
    {
        var def = LookupDefinitions.Get(kind);
        if (!def.AllowCreateDelete)
            return Result.Failure(Error.Forbidden("lookup.fixed_list", $"{kind} entries are fixed and cannot be deleted."));

        await using var uow = _uowFactory.Create();
        try
        {
            return await def.RemoveAsync(uow, id, ct)
                ? Result.Success()
                : Result.Failure(Error.NotFound("lookup.not_found", $"{kind} '{id}' was not found."));
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: SqlForeignKeyViolation })
        {
            // The database foreign keys are the source of truth for "in use".
            return Result.Failure(Error.Conflict("lookup.in_use",
                $"This {kind} entry is in use and cannot be deleted."));
        }
    }

    // ---- helpers ----

    private static (SaveLookupItemDto Dto, Error? Error) Normalize(ILookupDefinition def, SaveLookupItemDto item)
    {
        var name = item.Name?.Trim() ?? string.Empty;
        var description = string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.Trim();
        var dto = item with { Name = name, Description = description };

        if (name.Length == 0)
            return (dto, Error.Validation("lookup.name_required", "Name is required."));
        if (name.Length > def.NameMaxLength)
            return (dto, Error.Validation("lookup.name_too_long", $"Name cannot be longer than {def.NameMaxLength} characters."));
        if (description is { Length: > MaxDescriptionLength })
            return (dto, Error.Validation("lookup.description_too_long", $"Description cannot be longer than {MaxDescriptionLength} characters."));

        var message = def.Validate(dto);
        return message is null ? (dto, null) : (dto, Error.Validation("lookup.invalid", message));
    }

    private static Error NotFound(LookupKind kind, string id)
        => Error.NotFound("lookup.not_found", $"{kind} '{id}' was not found.");

    private static Error DuplicateName(LookupKind kind, string name)
        => Error.Conflict("lookup.duplicate_name", $"A {kind} named '{name}' already exists.");
}
