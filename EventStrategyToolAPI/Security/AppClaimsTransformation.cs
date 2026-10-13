using System.Security.Claims;
using Core.Interface.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Shared.DTOs.Users;

namespace EventStrategyToolAPI.Security;

public sealed class AppClaimsTransformation(IUserDirectory users, IMemoryCache cache) : IClaimsTransformation
{
    private static readonly TimeSpan KnownUserTtl = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan UnknownUserTtl = TimeSpan.FromSeconds(30);

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true) return principal;

        if (principal.HasClaim(c => c.Type == AppClaimTypes.UserId)) return principal;

        var email = GetEmail(principal);
        if (string.IsNullOrWhiteSpace(email)) return principal;

        var user = await cache.GetOrCreateAsync($"appuser:{email.ToLowerInvariant()}", async entry =>
        {
            var found = await users.FindByEmailAsync(email);
            entry.AbsoluteExpirationRelativeToNow = found is { IsActive: true } ? KnownUserTtl : UnknownUserTtl;
            return found;
        });

        if (user is not { IsActive: true }) return principal;

        var appIdentity = new ClaimsIdentity(
            BuildClaims(user), authenticationType: "AppUser",
            nameType: ClaimTypes.Name, roleType: ClaimTypes.Role);

        var clone = principal.Clone();
        clone.AddIdentity(appIdentity);
        return clone;
    }

    private static IEnumerable<Claim> BuildClaims(AppUserDto user)
    {
        yield return new Claim(AppClaimTypes.UserId, user.Id);
        yield return new Claim(ClaimTypes.Name, user.FullName);
        yield return new Claim(ClaimTypes.Role, user.RoleName);
        if (!string.IsNullOrEmpty(user.DepartmentId))
            yield return new Claim(AppClaimTypes.DepartmentId, user.DepartmentId);
    }

    // v2 Azure tokens carry preferred_username; v1 tokens carry upn.
    private static string? GetEmail(ClaimsPrincipal p)
        => p.FindFirstValue("preferred_username")
           ?? p.FindFirstValue(ClaimTypes.Upn)
           ?? p.FindFirstValue("upn")
           ?? p.FindFirstValue(ClaimTypes.Email)
           ?? p.FindFirstValue("email");
}
