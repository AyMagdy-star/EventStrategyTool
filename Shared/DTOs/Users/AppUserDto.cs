namespace Shared.DTOs.Users;

/// <summary>The identity facts the app needs about a signed-in person, resolved from the User table.</summary>
public sealed record AppUserDto(
    string Id,
    string FullName,
    string Email,
    string? DepartmentId,
    string RoleId,
    string RoleName,
    bool IsActive);
