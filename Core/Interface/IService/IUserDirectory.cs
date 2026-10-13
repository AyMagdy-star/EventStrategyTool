using Shared.DTOs.Users;

namespace Core.Interface.IService;

/// <summary>Resolves an Azure sign-in (by email / UPN) to the application's own User row.</summary>
public interface IUserDirectory
{
    /// <summary>Null when no User row matches. Email comparison relies on the database's case-insensitive collation.</summary>
    Task<AppUserDto?> FindByEmailAsync(string email, CancellationToken ct = default);
}
