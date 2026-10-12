using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Domain.Data;
using Core.UnitOfWork;

namespace Core.Data;

public static class DataServiceCollectionExtensions
{
    public static IServiceCollection AddDataInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContextFactory<EventStrategyToolDbContext>(o => o.UseSqlServer(connectionString));
        services.AddSingleton<RepositoryRegistry>();
        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
        return services;
    }

    public static IServiceCollection AddRepository<TRepo>(
        this IServiceCollection services, Func<DbContext, TRepo> factory) where TRepo : class
        => services.AddSingleton(new RepositoryRegistration(typeof(TRepo), ctx => factory(ctx)));
}
