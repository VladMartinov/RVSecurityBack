using Application.Extensions;
using Core.Dtos.UserRole;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Exceptions.Exceptions.UserRoles;
using Mapster;

namespace Application.Services;

public class UserRoleService(IUserRoleRepository userRoleRepository, IRoleRepository roleRepository, 
    IUserRepository userRepository, IUnitOfWork unitOfWork) : IUserRoleService
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
        await unitOfWork.AddAsync(userRoleModel, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return userRoleModel;
    }

    public async Task RemoveUserRole(RemoveUserRoleDto dto, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(dto.UserId, dto.RoleId, cancellationToken);
        var userRole = await userRoleRepository.GetUserRoleAsync(dto.UserId, dto.RoleId, true, cancellationToken)
                       ?? throw new UserRoleNotFoundException(dto.UserId, dto.RoleId);
        unitOfWork.Remove(userRole);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}