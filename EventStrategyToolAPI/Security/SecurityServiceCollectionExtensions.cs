using Core.Interface.IService;
using Microsoft.AspNetCore.Authentication;

namespace EventStrategyToolAPI.Security;

public static class SecurityServiceCollectionExtensions
{
    /// <summary>
    /// Call after AddAuthentication().AddMicrosoftIdentityWebApi(...), AddDataInfrastructure(...) and AddCoreServices().
    /// </summary>
    public static IServiceCollection AddAppSecurity(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddScoped<IClaimsTransformation, AppClaimsTransformation>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        return services;
    }
}
