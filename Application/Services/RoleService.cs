using Core.Dtos.Role;
using Core.Entities;
using Core.Extensions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Exceptions.Exceptions.Roles;
using Mapster;

namespace Application.Services;

public class RoleService(IRoleRepository roleRepository) : IRoleService
{
    private async Task ValidateNameAsync(string? name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        name = name.Trim();
        
        if (name.Length < 3)
            throw new TooShortRoleNameException(name, 3, name.Length);
        if (name.Length > 24)
            throw new TooLongRoleNameException(name, name.Length, 24);
        if (await roleRepository.RoleExistsAsync(name, cancellationToken))
            throw new SameRoleExistsException(name);
    }
    
    public async Task<Role> CreateRoleAsync(RoleCreationDto dto, CancellationToken cancellationToken = default)
    {
        var roleModel = dto.Adapt<Role>();
        await ValidateNameAsync(roleModel.Name, cancellationToken);
        return await roleRepository.AddRoleAsync(roleModel, cancellationToken);
    }

    public async Task<Role> UpdateRoleAsync(RoleUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var existingRole = await roleRepository.GetRoleAsync(dto.Id, true, cancellationToken);
        if (existingRole == null) throw new RoleNotFoundException(dto.Id);
        
        if (existingRole.IsSystem && dto.IsSystem == false)
            throw new InvalidRoleChangeException(dto.Id);
        
        string? newName = dto.Name?.Trim();
        bool nameChanged = !string.IsNullOrWhiteSpace(newName) && newName != existingRole.Name;
        
        await ValidateNameAsync(nameChanged ? newName : null, cancellationToken);
        
        if (nameChanged)
        {
            existingRole.Name = newName!;
            existingRole.NormalizedName = newName!.ToNormalized();
        }
        
        existingRole.Description = dto.Description;
        existingRole.IsSystem = dto.IsSystem ?? existingRole.IsSystem;
        return await roleRepository.UpdateRoleAsync(existingRole, cancellationToken);
    }

    public async Task DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await roleRepository.GetRoleAsync(id, true, cancellationToken);
        if (role == null) 
            throw new RoleNotFoundException(id);
        if (role.IsSystem)
            throw new UnableRemoveSystemRoleException(id, role.Name);
        await roleRepository.DeleteRoleAsync(id, cancellationToken);
    }
}