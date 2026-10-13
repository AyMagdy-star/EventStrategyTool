namespace Core.Interface.IService;

/// <summary>
/// The signed-in application user, for services that need "who is acting" (audit columns, status changes,
/// approvals). Core depends only on this interface, never on ASP.NET; the web host provides the implementation.
/// </summary>
public interface ICurrentUser
{
    /// <summary>True when the caller signed in with Azure AND matches an active row in the User table.</summary>
    bool IsKnownUser { get; }

    /// <summary>User.Id. Throws if there is no known user, so audit fields can never be silently blank.</summary>
    string UserId { get; }

    string? DepartmentId { get; }
    string? RoleName { get; }
    bool IsInRole(string roleName);
}
