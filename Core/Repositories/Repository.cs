using System.Linq.Expressions;
using Core.Interface.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    public Repository(DbContext context)
    {
        Context = context;
        Set = context.Set<TEntity>();
    }

    protected DbContext Context { get; }
    protected DbSet<TEntity> Set { get; }

    public IQueryable<TEntity> Query() => Set.AsNoTracking();

    public async Task<TEntity?> GetByIdAsync(string id, CancellationToken ct = default)
        => await Set.FindAsync(new object[] { id }, ct);

    public Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
    {
        IQueryable<TEntity> q = Set.AsNoTracking();
        if (predicate is not null) q = q.Where(predicate);
        return q.ToListAsync(ct);
    }

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        => Set.AnyAsync(predicate, ct);

    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
        => await Set.AddAsync(entity, ct);

    public void Update(TEntity entity) => Set.Update(entity);
    public void Remove(TEntity entity) => Set.Remove(entity);
}
