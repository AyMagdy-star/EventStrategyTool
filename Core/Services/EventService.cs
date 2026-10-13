using Core.Repositories;
using Core.Lookups;
using Core.Rules;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Events;
using Shared.Results;
using Core.Interface;
using Core.Interface.IService;
using Core.Data;

namespace Core.Events;

internal sealed class EventService(IUnitOfWorkFactory uowFactory, ICurrentUser currentUser) : IEventService
{
    private const int SqlForeignKeyViolation = 547;

    // ---- reads ----

    public async Task<Result<EventDto>> GetAsync(string eventId, CancellationToken ct = default)
    {
        await using var uow = uowFactory.Create();
        var ev = await uow.Repository<Event>().Query().FirstOrDefaultAsync(e => e.Id == eventId, ct);
        return ev is null ? NotFound(eventId) : await LoadDtoAsync(uow, ev, ct);
    }

    public async Task<Result<PagedResult<EventSummaryDto>>> ListAsync(
        EventListFilter filter, PageRequest page, CancellationToken ct = default)
    {
        var statusId = filter.StatusId;
        var ownerId = filter.OwnerUserId;
        var from = filter.From;
        var to = filter.To;
        var text = string.IsNullOrWhiteSpace(filter.SearchText) ? null : filter.SearchText.Trim();

        await using var uow = uowFactory.Create();
        var q = uow.Repository<Event>().Query();

        if (statusId is not null) q = q.Where(e => e.StatusId == statusId);
        if (ownerId is not null) q = q.Where(e => e.EventOwnerUserId == ownerId);
        if (from is not null) q = q.Where(e => e.EventDate >= from);
        if (to is not null) q = q.Where(e => e.EventDate <= to);
        if (text is not null) q = q.Where(e => e.Name.Contains(text));

        var paged = await q
            .OrderByDescending(e => e.EventDate).ThenBy(e => e.Name).ThenBy(e => e.Id)
            .Select(e => new EventSummaryDto(e.Id, e.Name, e.EventTypeId, e.EventDate, e.StatusId, e.EventOwnerUserId))
            .ToPagedAsync(page, ct);

        return paged;
    }

    // ---- writes ----

    public async Task<Result<EventDto>> CreateAsync(SaveEventDto dto, CancellationToken ct = default)
    {
        if (!currentUser.IsKnownUser)
            return Error.Forbidden("event.no_user", "Only a known, active user can create events.");

        var errors = EventRules.Validate(dto);
        if (errors.Count > 0) return Invalid(errors);

        await using var uow = uowFactory.Create();

        var draft = await uow.Repository<EventStatus>().Query()
            .FirstOrDefaultAsync(s => s.Name == EventRules.DraftStatusName, ct);
        if (draft is null)
            return Error.Unexpected("event.no_draft_status", $"The '{EventRules.DraftStatusName}' status is missing. Run the seed script.");

        var userId = currentUser.UserId;
        await uow.BeginTransactionAsync(ct);
        try
        {
            // First save: the database sequence assigns the event Id that the child rows need.
            var ev = new Event
            {
                StatusId = draft.Id,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
            };
            ApplyScalars(ev, dto);
            await uow.Repository<Event>().AddAsync(ev, ct);
            await uow.SaveChangesAsync(ct);

            await AddChildrenAsync(uow, ev.Id, dto, ct);
            await uow.Repository<EventStatusHistory>().AddAsync(new EventStatusHistory
            {
                EventId = ev.Id,
                FromStatusId = null,
                ToStatusId = draft.Id,
                ChangedByUserId = userId,
                ChangedAt = DateTime.UtcNow,
                Comment = "Event created",
            }, ct);
            await uow.SaveChangesAsync(ct);

            await uow.CommitAsync(ct);
            return await LoadDtoAsync(uow, ev, ct);
        }
        catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
        {
            await uow.RollbackAsync(ct);
            return MissingReference();
        }
    }

    public async Task<Result<EventDto>> UpdateAsync(string eventId, SaveEventDto dto, CancellationToken ct = default)
    {
        var errors = EventRules.Validate(dto);
        if (errors.Count > 0) return Invalid(errors);

        await using var uow = uowFactory.Create();
        var ev = await uow.Repository<Event>().GetByIdAsync(eventId, ct);
        if (ev is null) return NotFound(eventId);

        if (await IsLockedAsync(uow, eventId, ct))
            return Error.Conflict("event.locked", "This event is locked and can no longer be edited.");

        try
        {
            ApplyScalars(ev, dto);
            await SyncDepartmentsAsync(uow, eventId, dto.Departments, ct);
            await ReplaceResponsibilitiesAsync(uow, eventId, dto.Responsibilities, ct);
            await ReplaceRisksAsync(uow, eventId, dto.RisksAndUpsides, ct);

            await uow.SaveChangesAsync(ct);   // one save = one database transaction
            return await LoadDtoAsync(uow, ev, ct);
        }
        catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
        {
            return MissingReference();
        }
    }

    /// <summary>
    /// Not blocked by the lock: W6 sets the governing entity as part of submitting an assessment,
    /// which is the step that locks it.
    /// </summary>
    public async Task<Result> SetGoverningEntityAsync(string eventId, string governingEntityId, CancellationToken ct = default)
    {
        await using var uow = uowFactory.Create();
        var ev = await uow.Repository<Event>().GetByIdAsync(eventId, ct);
        if (ev is null) return Result.Failure(NotFound(eventId));

        ev.GoverningEntityId = governingEntityId;
        try
        {
            await uow.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
        {
            return Result.Failure(Error.Validation("event.governing_entity_missing", "The selected governing entity does not exist."));
        }
    }

    // ---- child rows ----

    private static async Task AddChildrenAsync(IUnitOfWork uow, string eventId, SaveEventDto dto, CancellationToken ct)
    {
        foreach (var d in dto.Departments)
            await uow.Repository<EventDepartment>().AddAsync(NewDepartment(eventId, d), ct);
        foreach (var r in dto.Responsibilities)
            await uow.Repository<EventResponsibility>().AddAsync(NewResponsibility(eventId, r), ct);
        foreach (var r in dto.RisksAndUpsides)
            await uow.Repository<EventRiskUpside>().AddAsync(NewRisk(eventId, r), ct);
    }

    // Departments have a composite key (EventId, DepartmentId), so they are diffed rather than deleted and re-added.
    private static async Task SyncDepartmentsAsync(
        IUnitOfWork uow, string eventId, IReadOnlyList<EventDepartmentDto> wanted, CancellationToken ct)
    {
        var repo = uow.Repository<EventDepartment>();
        var existing = await repo.ListAsync(d => d.EventId == eventId, ct);

        foreach (var row in existing)
        {
            var match = wanted.FirstOrDefault(w => w.DepartmentId == row.DepartmentId);
            if (match is null) repo.Remove(row);
            else if (row.DepartmentRole != match.DepartmentRole)
            {
                row.DepartmentRole = match.DepartmentRole;
                repo.Update(row);
            }
        }

        foreach (var w in wanted.Where(w => existing.All(row => row.DepartmentId != w.DepartmentId)))
            await repo.AddAsync(NewDepartment(eventId, w), ct);
    }

    private static async Task ReplaceResponsibilitiesAsync(
        IUnitOfWork uow, string eventId, IReadOnlyList<EventResponsibilityDto> wanted, CancellationToken ct)
    {
        var repo = uow.Repository<EventResponsibility>();
        foreach (var row in await repo.ListAsync(r => r.EventId == eventId, ct)) repo.Remove(row);
        foreach (var r in wanted) await repo.AddAsync(NewResponsibility(eventId, r), ct);
    }

    private static async Task ReplaceRisksAsync(
        IUnitOfWork uow, string eventId, IReadOnlyList<EventRiskUpsideDto> wanted, CancellationToken ct)
    {
        var repo = uow.Repository<EventRiskUpside>();
        foreach (var row in await repo.ListAsync(r => r.EventId == eventId, ct)) repo.Remove(row);
        foreach (var r in wanted) await repo.AddAsync(NewRisk(eventId, r), ct);
    }

    private static EventDepartment NewDepartment(string eventId, EventDepartmentDto d)
        => new() { EventId = eventId, DepartmentId = d.DepartmentId, DepartmentRole = d.DepartmentRole };

    private static EventResponsibility NewResponsibility(string eventId, EventResponsibilityDto r)
        => new()
        {
            EventId = eventId,
            ResponsibilityType = r.ResponsibilityType.Trim(),
            UserId = Clean(r.UserId),
            PersonName = Clean(r.PersonName),
        };

    private static EventRiskUpside NewRisk(string eventId, EventRiskUpsideDto r)
        => new() { EventId = eventId, ItemType = r.ItemType.Trim(), Description = r.Description.Trim(), SortOrder = r.SortOrder };

    // ---- mapping and helpers ----

    private static void ApplyScalars(Event ev, SaveEventDto d)
    {
        ev.Name = d.Name.Trim();
        ev.EventTypeId = d.EventTypeId;
        ev.EventDate = d.EventDate;
        ev.Location = Clean(d.Location);
        ev.Venue = Clean(d.Venue);
        ev.TargetAudience = Clean(d.TargetAudience);
        ev.PrimaryObjectiveId = d.PrimaryObjectiveId;
        ev.SecondaryObjectiveId = Clean(d.SecondaryObjectiveId);
        ev.PrimaryIcpId = Clean(d.PrimaryIcpId);
        ev.SecondaryIcpId = Clean(d.SecondaryIcpId);
        ev.InviteMethodId = Clean(d.InviteMethodId);
        ev.FundingSourceId = Clean(d.FundingSourceId);
        ev.NumberOfSponsors = d.NumberOfSponsors;
        ev.EntertainmentTypeId = Clean(d.EntertainmentTypeId);
        ev.EventOwnerUserId = Clean(d.EventOwnerUserId);
        ev.AccessLevels = Clean(d.AccessLevels);
        ev.TicketTypes = Clean(d.TicketTypes);
        ev.SpecialExperiences = Clean(d.SpecialExperiences);
    }

    private static async Task<EventDto> LoadDtoAsync(IUnitOfWork uow, Event ev, CancellationToken ct)
    {
        var departments = await uow.Repository<EventDepartment>().ListAsync(d => d.EventId == ev.Id, ct);
        var responsibilities = await uow.Repository<EventResponsibility>().ListAsync(r => r.EventId == ev.Id, ct);
        var risks = (await uow.Repository<EventRiskUpside>().ListAsync(r => r.EventId == ev.Id, ct))
            .OrderBy(r => r.SortOrder).ToList();

        return new EventDto(
            ev.Id, ev.Name, ev.EventTypeId, ev.EventDate, ev.Location, ev.Venue, ev.TargetAudience,
            ev.PrimaryObjectiveId, ev.SecondaryObjectiveId, ev.PrimaryIcpId, ev.SecondaryIcpId,
            ev.InviteMethodId, ev.FundingSourceId, ev.NumberOfSponsors, ev.EntertainmentTypeId,
            ev.GoverningEntityId, ev.StatusId, ev.EventOwnerUserId, ev.CreatedByUserId, ev.CreatedAt,
            ev.AccessLevels, ev.TicketTypes, ev.SpecialExperiences,
            departments.Select(d => new EventDepartmentDto(d.DepartmentId, d.DepartmentRole)).ToList(),
            responsibilities.Select(r => new EventResponsibilityDto(r.ResponsibilityType, r.UserId, r.PersonName)).ToList(),
            risks.Select(r => new EventRiskUpsideDto(r.ItemType, r.Description, r.SortOrder)).ToList());
    }

    // ASSUMPTION (pending D1/D10): an event is locked once any of its assessments is locked.
    private static Task<bool> IsLockedAsync(IUnitOfWork uow, string eventId, CancellationToken ct)
        => uow.Repository<Assessment>().AnyAsync(a => a.EventId == eventId && a.IsLocked, ct);

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static bool IsForeignKeyViolation(DbUpdateException ex)
        => ex.InnerException is SqlException { Number: SqlForeignKeyViolation };

    private static Error NotFound(string id) => Error.NotFound("event.not_found", $"Event '{id}' was not found.");

    private static Error Invalid(List<string> errors) => Error.Validation("event.invalid", string.Join(" ", errors));

    private static Error MissingReference() => Error.Validation("event.reference_missing",
        "One of the selected values (type, objective, ICP, department, user and so on) no longer exists.");
}
