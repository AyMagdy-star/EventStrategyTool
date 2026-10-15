using Microsoft.Extensions.DependencyInjection;

namespace Core.ScoringConfiguration;
public static class ScoringConfigurationRegistration
{
    public static IServiceCollection AddScoringConfigurationWorkflow(this IServiceCollection services)
    {
        services.AddScoped<IScoringConfigurationService, ScoringConfigurationService>();
        return services;
    }
}
