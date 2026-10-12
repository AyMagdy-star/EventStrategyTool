namespace Core.Interface;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<TEntity> Repository<TEntity>() where TEntity : class;

    TRepo GetRepository<TRepo>() where TRepo : class;

    Task<int> SaveChangesAsync(CancellationToken ct = default);

    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}
