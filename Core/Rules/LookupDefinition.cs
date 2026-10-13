using Core.Interface;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Lookups;

namespace Core.Rules;

/// <summary>How one lookup list is read, written and validated. One instance per LookupKind.</summary>
internal interface ILookupDefinition
{
    LookupKind Kind { get; }
    bool AllowCreateDelete { get; }
    int NameMaxLength { get; }

    /// <summary>Kind-specific validation. Returns an error message, or null when valid.</summary>
    string? Validate(SaveLookupItemDto dto);

    Task<IReadOnlyList<LookupItemDto>> ListAsync(IUnitOfWork uow, bool activeOnly, string? includeId, CancellationToken ct);
    Task<LookupItemDto?> FindAsync(IUnitOfWork uow, string id, CancellationToken ct);
    Task<bool> NameExistsAsync(IUnitOfWork uow, string name, string? excludeId, CancellationToken ct);
    Task<LookupItemDto> AddAsync(IUnitOfWork uow, SaveLookupItemDto dto, CancellationToken ct);
    Task<LookupItemDto?> UpdateAsync(IUnitOfWork uow, string id, SaveLookupItemDto dto, CancellationToken ct);
    Task<bool> RemoveAsync(IUnitOfWork uow, string id, CancellationToken ct);
}

/// <summary>
/// Generic implementation. All 11 lookup tables share Id and Name columns; SortOrder and IsActive
/// exist on only some of them, so they are read with EF.Property and switched on by flags.
/// Only the DTO mapping differs per list, and it is passed in as delegates.
/// </summary>
internal sealed class LookupDefinition<TEntity> : ILookupDefinition where TEntity : class, new()
{
    private readonly Func<TEntity, LookupItemDto> _toDto;
    private readonly Action<TEntity, SaveLookupItemDto> _apply;
    private readonly Func<SaveLookupItemDto, string?>? _validate;
    private readonly bool _hasSortOrder;
    private readonly bool _hasIsActive;

    public LookupDefinition(
        LookupKind kind,
        Func<TEntity, LookupItemDto> toDto,
        Action<TEntity, SaveLookupItemDto> apply,
        Func<SaveLookupItemDto, string?>? validate = null,
        bool hasSortOrder = true,
        bool hasIsActive = true,
        bool allowCreateDelete = true,
        int nameMaxLength = 100)
    {
        Kind = kind;
        _toDto = toDto;
        _apply = apply;
        _validate = validate;
        _hasSortOrder = hasSortOrder;
        _hasIsActive = hasIsActive;
        AllowCreateDelete = allowCreateDelete;
        NameMaxLength = nameMaxLength;
    }

    public LookupKind Kind { get; }
    public bool AllowCreateDelete { get; }
    public int NameMaxLength { get; }

    public string? Validate(SaveLookupItemDto dto) => _validate?.Invoke(dto);

    public async Task<IReadOnlyList<LookupItemDto>> ListAsync(
        IUnitOfWork uow, bool activeOnly, string? includeId, CancellationToken ct)
    {
        var q = uow.Repository<TEntity>().Query();

        if (activeOnly && _hasIsActive)
        {
            q = includeId is null
                ? q.Where(e => EF.Property<bool>(e, "IsActive"))
                : q.Where(e => EF.Property<bool>(e, "IsActive") || EF.Property<string>(e, "Id") == includeId);
        }

        var ordered = _hasSortOrder
            ? q.OrderBy(e => EF.Property<int>(e, "SortOrder")).ThenBy(e => EF.Property<string>(e, "Name"))
            : q.OrderBy(e => EF.Property<string>(e, "Name"));

        var rows = await ordered.ToListAsync(ct);
        return rows.Select(_toDto).ToList();
    }

    public async Task<LookupItemDto?> FindAsync(IUnitOfWork uow, string id, CancellationToken ct)
    {
        var entity = await uow.Repository<TEntity>().Query()
            .FirstOrDefaultAsync(e => EF.Property<string>(e, "Id") == id, ct);
        return entity is null ? null : _toDto(entity);
    }

    public Task<bool> NameExistsAsync(IUnitOfWork uow, string name, string? excludeId, CancellationToken ct)
        => uow.Repository<TEntity>().AnyAsync(
            e => EF.Property<string>(e, "Name") == name
                 && (excludeId == null || EF.Property<string>(e, "Id") != excludeId), ct);

    public async Task<LookupItemDto> AddAsync(IUnitOfWork uow, SaveLookupItemDto dto, CancellationToken ct)
    {
        var entity = new TEntity();
        _apply(entity, dto);
        await uow.Repository<TEntity>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);          // the database sequence assigns Id here
        return _toDto(entity);
    }

    public async Task<LookupItemDto?> UpdateAsync(IUnitOfWork uow, string id, SaveLookupItemDto dto, CancellationToken ct)
    {
        var entity = await uow.Repository<TEntity>().GetByIdAsync(id, ct);
        if (entity is null) return null;
        _apply(entity, dto);
        await uow.SaveChangesAsync(ct);
        return _toDto(entity);
    }

    public async Task<bool> RemoveAsync(IUnitOfWork uow, string id, CancellationToken ct)
    {
        var entity = await uow.Repository<TEntity>().GetByIdAsync(id, ct);
        if (entity is null) return false;
        uow.Repository<TEntity>().Remove(entity);
        await uow.SaveChangesAsync(ct);
        return true;
    }
}
