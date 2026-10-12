using Core.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Domain.Data;
using Core.Data;
using Core.Repositories;

namespace Core.UnitOfWork;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private readonly RepositoryRegistry _registry;
    private readonly Dictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;

    public UnitOfWork(DbContext context, RepositoryRegistry registry)
    {
        _context = context;
        _registry = registry;
    }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        var key = typeof(IRepository<TEntity>);
        if (!_repositories.TryGetValue(key, out var repo))
        {
            repo = new Repository<TEntity>(_context);
            _repositories[key] = repo;
        }
        return (IRepository<TEntity>)repo;
    }

    public TRepo GetRepository<TRepo>() where TRepo : class
    {
        var key = typeof(TRepo);
        if (!_repositories.TryGetValue(key, out var repo))
        {
            repo = _registry.Create<TRepo>(_context);
            _repositories[key] = repo;
        }
        return (TRepo)repo;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is not null)
            throw new InvalidOperationException("A transaction is already active on this unit of work.");
        _transaction = await _context.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_transaction is null) throw new InvalidOperationException("No active transaction.");
        await _transaction.CommitAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction is null) return;
        await _transaction.RollbackAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null) await _transaction.DisposeAsync(); // uncommitted = rolled back
        await _context.DisposeAsync();
    }
}

internal sealed class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IDbContextFactory<EventStrategyToolDbContext> _contextFactory; // TODO: real DbContext name
    private readonly RepositoryRegistry _registry;

    public UnitOfWorkFactory(IDbContextFactory<EventStrategyToolDbContext> contextFactory, RepositoryRegistry registry)
    {
        _contextFactory = contextFactory;
        _registry = registry;
    }

    public IUnitOfWork Create() => new UnitOfWork(_contextFactory.CreateDbContext(), _registry);
}
