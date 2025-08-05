using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface IUserRoleRepository
{
    Task<UserRole> AddRoleToUserAsync(UserRole userRole, CancellationToken cancellationToken = default);

    Task<IEnumerable<UserRole>> AddRoleToUserAsync(IEnumerable<UserRole> userRoles,
        CancellationToken cancellationToken = default);

    Task DeleteUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    Task<IEnumerable<UserRole>> GetUserRolesAsync(Guid userId, bool track = true, int? limit = null, int? offset = null,
        CancellationToken cancellationToken = default);
    Task<UserRole?> GetUserRoleAsync(Guid userId, Guid roleId, bool track = true, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
}