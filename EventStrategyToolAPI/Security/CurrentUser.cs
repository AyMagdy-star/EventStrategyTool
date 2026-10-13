using System.Security.Claims;
using Core.Interface.IService;

namespace EventStrategyToolAPI.Security;

/// <summary>Reads the claims added by AppClaimsTransformation. No database access.</summary>
public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    public bool IsKnownUser => Principal?.HasClaim(c => c.Type == AppClaimTypes.UserId) ?? false;

    public string UserId => Principal?.FindFirstValue(AppClaimTypes.UserId)
        ?? throw new InvalidOperationException("There is no known application user in the current request.");

    public string? DepartmentId => Principal?.FindFirstValue(AppClaimTypes.DepartmentId);

    public string? RoleName => Principal?.FindFirstValue(ClaimTypes.Role);

    public bool IsInRole(string roleName) => Principal?.IsInRole(roleName) ?? false;
}
