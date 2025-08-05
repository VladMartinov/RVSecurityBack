using Core.Dtos.UserRole;
using Core.Entities;

namespace Core.Interfaces.Services;

public interface IUserRoleService
{
    Task<UserRole> SetUserRole(SetUserRoleDto dto, CancellationToken cancellationToken = default);
    Task RemoveUserRole(RemoveUserRoleDto dto, CancellationToken cancellationToken = default);
}