using Application.Extensions;
using Core.Dtos.UserRole;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Exceptions.Exceptions.UserRoles;
using Mapster;

namespace Application.Services;

public class UserRoleService(IUserRoleRepository userRoleRepository, IRoleRepository roleRepository, 
    IUserRepository userRepository) : IUserRoleService
{
    private async Task ValidateAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        await roleRepository.EnsureRoleExists(roleId, cancellationToken);
        await userRepository.EnsureUserExists(userId, cancellationToken);
    }
    
    public async Task<UserRole> SetUserRole(SetUserRoleDto dto, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(dto.UserId, dto.RoleId, cancellationToken);
        var userRoleModel = dto.Adapt<UserRole>();
        return await userRoleRepository.AddRoleToUserAsync(userRoleModel, cancellationToken);
    }

    public async Task RemoveUserRole(RemoveUserRoleDto dto, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(dto.UserId, dto.RoleId, cancellationToken);
        var urExists = await userRoleRepository.ExistsAsync(dto.UserId, dto.RoleId, cancellationToken);
        if (!urExists) throw new UserRoleNotFoundException(dto.UserId, dto.RoleId);
        await userRoleRepository.DeleteUserRoleAsync(dto.UserId, dto.RoleId, cancellationToken);
    }
}