using Core.Extensions;
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
    
    public static async Task EnsureRolesExists(this IRoleRepository repository, IEnumerable<Guid> roleIds,
        CancellationToken cancellationToken = default)
    {
        var notFoundRoles = await repository.RolesExistsAsync(roleIds, cancellationToken);
        var list = notFoundRoles.ToList();
        if (list.Count != 0)
            throw new RoleNotFoundException(list);
    }
    
    public static async Task EnsureRoleExists(this IRoleRepository repository, string roleName,
        CancellationToken cancellationToken = default)
    {
        var exists = await repository.RoleExistsAsync(roleName, cancellationToken);
        if (exists)
            throw new RoleNotFoundException(roleName);
    }
    
    public static async Task EnsureRolesExists(this IRoleRepository repository, IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        var set = roleNames.Select(x => x.ToNormalized()).ToHashSet();
        var notFound = await repository.RolesExistsAsync(set, cancellationToken);
        if (set.Count != 0)
            throw new RoleNotFoundException(notFound);
    }
}