using Application.Extensions;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Exceptions.Exceptions.UserRoles;

namespace Application.Services;

public class UserRoleService(IUserRoleRepository userRoleRepository, IRoleRepository roleRepository, 
    IUserRepository userRepository, IUnitOfWork unitOfWork) : IUserRoleService
{
    private async Task ValidateAsync(Guid userId, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        await roleRepository.EnsureRolesExists(roleIds, cancellationToken);
        await userRepository.EnsureUserExists(userId, cancellationToken);
    }
    
    private async Task ValidateAsync(Guid userId, IEnumerable<string> rolesNames, CancellationToken cancellationToken = default)
    {
        await roleRepository.EnsureRolesExists(rolesNames, cancellationToken);
        await userRepository.EnsureUserExists(userId, cancellationToken);
    }
    
    public async Task<UserRole> SetUserRole(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(userId, [roleId], cancellationToken);
        var userRoleModel = new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTime.UtcNow
        };
        await unitOfWork.AddAsync(userRoleModel, cancellationToken);
        return userRoleModel;
    }
    public async Task<IEnumerable<UserRole>> SetUserRoles(Guid userId, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        var rolesSet = roleIds.ToHashSet();
        await ValidateAsync(userId, rolesSet, cancellationToken);
        var userRoleModel = rolesSet.Select(x => new UserRole
        {
            UserId = userId,
            RoleId = x,
            AssignedAt = DateTime.UtcNow
        }).ToList();
        await unitOfWork.AddRangeAsync(userRoleModel, cancellationToken);
        return userRoleModel;
    }

    public async Task<UserRole> SetUserRole(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(userId, [roleName], cancellationToken);
        var role = (await roleRepository.GetRoleAsync(roleName, false, cancellationToken))!;
        var userRoleModel = new UserRole
        {
            UserId = userId,
            RoleId = role.Id,
            AssignedAt = DateTime.UtcNow
        };
        await unitOfWork.AddAsync(userRoleModel, cancellationToken);
        return userRoleModel;
    }

    public async Task<IEnumerable<UserRole>> SetUserRoles(Guid userId, IEnumerable<string> rolesNames, CancellationToken cancellationToken = default)
    {
        var rolesSet = rolesNames.ToHashSet();
        await ValidateAsync(userId, rolesSet, cancellationToken);
        var roles = await roleRepository.GetRolesAsync(rolesSet, false, cancellationToken);
        var userRoleModel = roles.Select(x => new UserRole
        {
            UserId = userId,
            RoleId = x.Id,
            AssignedAt = DateTime.UtcNow
        }).ToList();
        await unitOfWork.AddRangeAsync(userRoleModel, cancellationToken);
        return userRoleModel;
    }


    public async Task RemoveUserRole(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(userId, [roleId], cancellationToken);
        var userRole = await userRoleRepository.GetUserRoleAsync(userId, roleId, true, cancellationToken)
                       ?? throw new UserRoleNotFoundException(userId, roleId);
        unitOfWork.Remove(userRole);
    }
}