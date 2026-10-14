using Core.Interface;
using Core.Interface.IService;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Users;

namespace Core.Services;

internal sealed class UserDirectory(IUnitOfWorkFactory uowFactory) : IUserDirectory
{
    public async Task<AppUserDto?> FindByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = email.Trim();
        await using var uow = uowFactory.Create();

        return await uow.Repository<User>().Query()
            .Where(u => u.Email == normalized)
            .Select(u => new AppUserDto(u.Id, u.FullName, u.Email, u.DepartmentId, u.RoleId, u.Role.Name, u.IsActive))
            .FirstOrDefaultAsync(ct);
    }
}
