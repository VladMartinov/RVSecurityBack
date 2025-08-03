using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetRoleAsync(Guid id, bool track = true, CancellationToken cancellationToken = default);
    Task<Role?> GetRoleAsync(string name, bool track = true, CancellationToken cancellationToken = default);
    Task<Role> AddRoleAsync(Role role, CancellationToken cancellationToken = default);
    Task<Role> UpdateRoleAsync(Role role, CancellationToken cancellationToken = default);
    Task DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> RoleExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> RoleExistsAsync(Guid id, CancellationToken cancellationToken = default);
}