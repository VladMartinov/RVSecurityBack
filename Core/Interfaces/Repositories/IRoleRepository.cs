using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Role> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Role> CreateRoleAsync(Role role, CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> CreateRolesAsync(IEnumerable<Role> roles, CancellationToken cancellationToken = default);
    Task<Role> UpdateRoleAsync(Role role, CancellationToken cancellationToken = default);
    Task DeleteRoleAsync(Role role, CancellationToken cancellationToken = default);
}