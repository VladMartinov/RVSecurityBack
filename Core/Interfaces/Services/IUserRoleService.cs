using Core.Entities;

namespace Core.Interfaces.Services;

public interface IUserRoleService
{
    Task<UserRole> SetUserRole(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserRole>> SetUserRoles(Guid userId, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default);
    Task<UserRole> SetUserRole(Guid userId, string roleName, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserRole>> SetUserRoles(Guid userId, IEnumerable<string> rolesNames, CancellationToken cancellationToken = default);
    Task RemoveUserRole(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
}