using Microsoft.EntityFrameworkCore;

namespace Core.Data;

public sealed record RepositoryRegistration(Type ServiceType, Func<DbContext, object> Factory);

public sealed class RepositoryRegistry
{
    private readonly Dictionary<Type, Func<DbContext, object>> _factories;

    public RepositoryRegistry(IEnumerable<RepositoryRegistration> registrations)
        => _factories = registrations.ToDictionary(r => r.ServiceType, r => r.Factory);

    public TRepo Create<TRepo>(DbContext context) where TRepo : class
        => _factories.TryGetValue(typeof(TRepo), out var factory)
            ? (TRepo)factory(context)
            : throw new InvalidOperationException(
                $"No repository registered for {typeof(TRepo).Name}. Call AddRepository<{typeof(TRepo).Name}>(...) in the workflow's DI method.");
}
