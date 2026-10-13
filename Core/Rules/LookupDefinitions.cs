using Domain.Entities; // TODO: confirm the namespace and class names of the scaffolded entities
using Shared.DTOs.Lookups;

namespace Core.Rules;

/// <summary>
/// The only place that knows how each list differs. Adding a lookup = add a LookupKind value
/// and one entry here.
/// </summary>
internal static class LookupDefinitions
{
    private static readonly Dictionary<LookupKind, ILookupDefinition> All = Build();

    public static ILookupDefinition Get(LookupKind kind) => All[kind];

    private static Dictionary<LookupKind, ILookupDefinition> Build()
    {
        var defs = new ILookupDefinition[]
        {
            // Id, Name, SortOrder, IsActive
            Simple<EventType>(LookupKind.EventType, e => (e.Id, e.Name, e.SortOrder, e.IsActive),
                (e, d) => { e.Name = d.Name; e.SortOrder = d.SortOrder; e.IsActive = d.IsActive; }),
            Simple<InviteMethod>(LookupKind.InviteMethod, e => (e.Id, e.Name, e.SortOrder, e.IsActive),
                (e, d) => { e.Name = d.Name; e.SortOrder = d.SortOrder; e.IsActive = d.IsActive; }),
            Simple<FundingSource>(LookupKind.FundingSource, e => (e.Id, e.Name, e.SortOrder, e.IsActive),
                (e, d) => { e.Name = d.Name; e.SortOrder = d.SortOrder; e.IsActive = d.IsActive; }),
            Simple<Department>(LookupKind.Department, e => (e.Id, e.Name, e.SortOrder, e.IsActive),
                (e, d) => { e.Name = d.Name; e.SortOrder = d.SortOrder; e.IsActive = d.IsActive; }),

            // Id, Name, Description, IsActive (no SortOrder)
            new LookupDefinition<Icp>(LookupKind.Icp,
                e => new LookupItemDto(e.Id, e.Name, 0, e.IsActive, e.Description),
                (e, d) => { e.Name = d.Name; e.Description = d.Description; e.IsActive = d.IsActive; },
                hasSortOrder: false),

            // Fixed by code: the status workflow and approvals depend on these rows
            new LookupDefinition<EventStatus>(LookupKind.EventStatus,
                e => new LookupItemDto(e.Id, e.Name, e.SortOrder, true),
                (e, d) => { e.Name = d.Name; e.SortOrder = d.SortOrder; },
                hasIsActive: false, allowCreateDelete: false, nameMaxLength: 50),
            new LookupDefinition<DecisionOutcome>(LookupKind.DecisionOutcome,
                e => new LookupItemDto(e.Id, e.Name, e.SortOrder, true),
                (e, d) => { e.Name = d.Name; e.SortOrder = d.SortOrder; },
                hasIsActive: false, allowCreateDelete: false, nameMaxLength: 50),
            new LookupDefinition<Role>(LookupKind.Role,
                e => new LookupItemDto(e.Id, e.Name, 0, true, e.Description),
                (e, d) => { e.Name = d.Name; e.Description = d.Description; },
                hasSortOrder: false, hasIsActive: false, allowCreateDelete: false, nameMaxLength: 50),

            new LookupDefinition<TicketTierKind>(LookupKind.TicketTierKind,
                e => new LookupItemDto(e.Id, e.Name, e.SortOrder, true, IsPaying: e.IsPaying),
                (e, d) => { e.Name = d.Name; e.SortOrder = d.SortOrder; e.IsPaying = d.IsPaying ?? false; },
                hasIsActive: false, nameMaxLength: 50),

            new LookupDefinition<GoverningEntity>(LookupKind.GoverningEntity,
                e => new LookupItemDto(e.Id, e.Name, 0, true, MinScore: e.MinScore, MaxScore: e.MaxScore),
                (e, d) => { e.Name = d.Name; e.MinScore = d.MinScore; e.MaxScore = d.MaxScore; },
                d => d.MinScore > d.MaxScore ? "Min score cannot be greater than max score." : null,
                hasSortOrder: false, hasIsActive: false),

            new LookupDefinition<EntertainmentType>(LookupKind.EntertainmentType,
                e => new LookupItemDto(e.Id, e.Name, 0, true, Rate: e.EntertainmentTaxRate),
                (e, d) => { e.Name = d.Name; e.EntertainmentTaxRate = d.Rate!.Value; },
                d => d.Rate is null or < 0 ? "Entertainment tax rate is required and cannot be negative." : null,
                hasSortOrder: false, hasIsActive: false),
        };

        return defs.ToDictionary(d => d.Kind);
    }

    // Helper for the four lists that only have Id, Name, SortOrder and IsActive.
    private static ILookupDefinition Simple<TEntity>(
        LookupKind kind,
        Func<TEntity, (string Id, string Name, int SortOrder, bool IsActive)> read,
        Action<TEntity, SaveLookupItemDto> apply) where TEntity : class, new()
        => new LookupDefinition<TEntity>(
            kind,
            e => { var r = read(e); return new LookupItemDto(r.Id, r.Name, r.SortOrder, r.IsActive); },
            apply);
}
