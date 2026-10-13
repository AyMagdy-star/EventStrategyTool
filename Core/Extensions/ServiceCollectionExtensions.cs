using Core.Interface.IService;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers every Core service. One line per service, so merges between Dev A and Dev B stay trivial.
    /// Call AddDataInfrastructure(...) (Core/Data) first.
    /// Scoped, because EventService and the API security classes use the per-request ICurrentUser.
    /// </summary>
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // W1 reference data and users
        services.AddScoped<ILookupService, LookupService>();
        services.AddScoped<ILookupReader>(sp => sp.GetRequiredService<ILookupService>());
        services.AddScoped<IUserDirectory, UserDirectory>();

        // W2 events
        services.AddScoped<IEventService, EventService>();

        // W3 P&L
        services.AddSingleton<IPnlCalculator, PnlCalculator>();   // pure and stateless

        // W4 status and approval: add IEventStatusService here when implemented
        // W5 scoring configuration: add the scoring configuration service here

        return services;
    }
}
