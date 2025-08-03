using Core.Entities;
using Core.Extensions;
using Core.Interfaces.Repositories;
using Exceptions.Exceptions.Roles;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Extensions;

namespace Persistence.Repositories;

public class RoleRepository(UserDbContext context) : IRoleRepository
{
    public async Task<Role?> GetRoleAsync(Guid id, bool track = true, CancellationToken cancellationToken = default) 
        => await context.Roles.ConfigureTracking(track).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    
    public async Task<Role?> GetRoleAsync(string name, bool track = true, CancellationToken cancellationToken = default)
    {
        var normalizedRoleName = name.ToNormalized();
        return await context.Roles.ConfigureTracking(track)
            .FirstOrDefaultAsync(x => x.NormalizedName == normalizedRoleName, cancellationToken);
    }

    public async Task<Role> AddRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        await context.Roles.AddAsync(role, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return role;
    }

    public async Task<Role> UpdateRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        var id = role.Id;
        var toUpdate = await GetRoleAsync(id, true, cancellationToken) ?? throw new RoleNotFoundException(id);
        UpdateRoleFields(toUpdate, role);
        await context.SaveChangesAsync(cancellationToken);
        return toUpdate;
    }
    
    private void UpdateRoleFields(Role existingRole, Role newRole)
    {
        existingRole.Name = newRole.Name;
        existingRole.NormalizedName = newRole.Name.ToNormalized();
        existingRole.Description = newRole.Description;
        existingRole.UpdatedAt = DateTime.UtcNow;
        existingRole.IsSystem = newRole.IsSystem;
    }

    public async Task DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await GetRoleAsync(id, true, cancellationToken)
            ?? throw new RoleNotFoundException(id);
        context.Roles.Remove(role);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RoleExistsAsync(string name, CancellationToken cancellationToken = default)
        => await context.Roles.AsNoTracking().AnyAsync(x => x.NormalizedName == name.ToNormalized(), cancellationToken);
    
    public async Task<bool> RoleExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Roles.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken);
}