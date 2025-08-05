using Core.Interfaces.Repositories;
using Exceptions.Exceptions.Roles;

namespace Application.Extensions;

public static class RoleRepositoryExtensions
{
    public static async Task EnsureRoleExists(this IRoleRepository repository, Guid roleId,
        CancellationToken cancellationToken = default)
    {
        if (!await repository.RoleExistsAsync(roleId, cancellationToken))
            throw new RoleNotFoundException(roleId);
    }
}