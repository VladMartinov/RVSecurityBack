using Core.Dtos.Role;
using Core.Entities;

namespace Core.Interfaces.Services;

public interface IRoleService
{
    Task<Role> CreateRoleAsync(RoleCreationDto dto, CancellationToken cancellationToken = default);
    Task<Role> UpdateRoleAsync(RoleUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default);
}