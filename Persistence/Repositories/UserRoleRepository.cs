using Core.Entities;
using Core.Interfaces.Repositories;
using Exceptions.Exceptions.UserRoles;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Extensions;

namespace Persistence.Repositories;

public class UserRoleRepository(UserDbContext context) : IUserRoleRepository
{
    public async Task<UserRole> AddRoleToUserAsync(UserRole userRole, CancellationToken cancellationToken = default)
    {
        await context.UserRoles.AddAsync(userRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return userRole;
    }
    
    public async Task<IEnumerable<UserRole>> AddRoleToUserAsync(IEnumerable<UserRole> userRoles, CancellationToken cancellationToken = default)
    {
        var asList = userRoles.ToList();
        
        if (asList.Count == 0) return [];

        await context.UserRoles.AddRangeAsync(asList, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return asList;
    }

    public async Task DeleteUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRole = await context.UserRoles
            .FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId, cancellationToken);
        if(userRole == null)
            throw new UserRoleNotFoundException(userId, roleId);
        context.UserRoles.Remove(userRole);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetUserRolesAsync(Guid userId, bool track = true, int? limit = null, int? offset = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.UserRoles
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.RoleId)
            .ConfigureTracking(track);

        if (offset != null)
            query = query.Skip(offset.Value);
        
        if (limit != null)
            query = query.Take(limit.Value);
        
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default) 
        => await context.UserRoles.AsNoTracking()
            .AnyAsync(x => x.UserId == userId && x.RoleId == roleId, cancellationToken);
}