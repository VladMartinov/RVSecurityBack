using Core.Entities;
using Core.Extensions;
using Core.Interfaces.Repositories;
using Exceptions.Exceptions.Users;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Extensions;

namespace Persistence.Repositories;

public class UserRepository(UserDbContext context) : IUserRepository
{
    public async Task<User?> GetUserByIdAsync(Guid userId, bool track = true, CancellationToken cancellationToken = default) 
        => await context.Users.ConfigureTracking(track)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

    public async Task<User?> GetUserByUserNameAsync(string userName, bool track = true, CancellationToken cancellationToken = default)
        => await context.Users.ConfigureTracking(track)
            .FirstOrDefaultAsync(x => x.NormalizedUserName == userName.ToNormalized(), cancellationToken);

    public async Task<User?> GetUserByEmailAsync(string email, bool track = true, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToNormalizedEmail();
        var userEmail = await context.UserEmails
            .ConfigureTracking(track)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
        return userEmail?.User;
    }

    public async Task<User?> GetUserByPhoneAsync(string phoneNumber, bool track = true, CancellationToken cancellationToken = default)
    {
        var normalizedPhone = phoneNumber.ToNormalizedPhoneNumber();
        var userPhone = await context.UserPhones.ConfigureTracking(track)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.NormalizedPhone == normalizedPhone, cancellationToken);
        return userPhone?.User;
    }

    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var userToUpdate = await context.Users.FirstOrDefaultAsync(x => x.Id == user.Id, cancellationToken) ?? 
                           throw new UserNotFound(user.Id);
        userToUpdate.UpdatedAt = DateTime.UtcNow;
        userToUpdate.UserName = user.UserName;
        userToUpdate.NormalizedUserName = user.NormalizedUserName;
        userToUpdate.PasswordHash = user.PasswordHash;
        userToUpdate.LockoutEnd = user.LockoutEnd;
        
        await context.SaveChangesAsync(cancellationToken);
        return userToUpdate;
    }

    public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ?? 
                   throw new UserNotFound(id);
        context.Users.Remove(user);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsUserNameTakenAsync(string userName, CancellationToken cancellationToken = default)
    {
        var normalizedUserName = userName.ToNormalized();
        return await context.Users.AnyAsync(x => x.NormalizedUserName == normalizedUserName, cancellationToken);
    }

    public async Task<bool> UserExists(Guid id, CancellationToken cancellationToken = default) 
        => await context.Users.AnyAsync(x => x.Id == id, cancellationToken);
}