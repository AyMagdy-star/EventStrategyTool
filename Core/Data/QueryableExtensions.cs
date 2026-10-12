using Microsoft.EntityFrameworkCore;
using EventStrategyTool.Shared.Results;

namespace Core.Data;

public static class QueryableExtensions
{
    //Counts, then fetches one page. The query must already be ordered.
    public static async Task<PagedResult<T>> ToPagedAsync<T>(
        this IQueryable<T> query, PageRequest request, CancellationToken ct = default)
    {
        var total = await query.CountAsync(ct);
        var items = await query.Skip(request.Skip).Take(request.SafePageSize).ToListAsync(ct);
        return new PagedResult<T>(items, request.SafePage, request.SafePageSize, total);
    }
}
