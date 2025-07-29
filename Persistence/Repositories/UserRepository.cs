using Core.Entities;
using Core.Exceptions.Users;
using Core.Extensions;
using Core.Interfaces;
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

    public async Task<User> CreateUserAsync(User user, string passwordHash, CancellationToken cancellationToken = default)
    {
        user.UserName = user.UserName.Trim();
        var normalizedUserName = user.UserName.ToNormalized();
        if (string.IsNullOrWhiteSpace(normalizedUserName))
            throw new ArgumentNullException(nameof(normalizedUserName), "Имя пользователя не может быть пустым");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentNullException(nameof(passwordHash), "Хэш пароля не может быть пустым");
        if (!await IsUserNameFree(user.UserName, cancellationToken))
            throw new UserNameAlreadyTaken(user.UserName);
        var newUser = new User
        {
            UserName = user.UserName,
            NormalizedUserName = normalizedUserName,
            PasswordHash = passwordHash,
            TwoFactorEnabled = user.TwoFactorEnabled,
            LockoutEnd = user.LockoutEnd,
        };
        await context.Users.AddAsync(newUser, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return newUser;
    }

    public async Task<User> UpdateUserAsync(User user, string passwordHash, CancellationToken cancellationToken = default)
    {
        var userId = user.Id;
        user.UserName = user.UserName.Trim();
        var userToUpdate = await context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken) ?? 
                           throw new UserNotFound(userId);
        var normalizedUserName = user.UserName.ToNormalized();
        if (string.IsNullOrWhiteSpace(normalizedUserName))
            throw new ArgumentNullException(nameof(normalizedUserName), "Имя пользователя не может быть пустым");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentNullException(nameof(passwordHash), "Хэш пароля не может быть пустым");
        if (userToUpdate.NormalizedUserName != normalizedUserName && !await IsUserNameFree(user.UserName, cancellationToken))
            throw new UserNameAlreadyTaken(userToUpdate.UserName);
        
        userToUpdate.UpdatedAt = DateTime.UtcNow;
        userToUpdate.UserName = user.UserName;
        userToUpdate.NormalizedUserName = normalizedUserName;
        userToUpdate.PasswordHash = passwordHash;
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

    /// <summary>
    /// Check if username is taken.
    /// </summary>
    /// <param name="userName">Username</param>
    /// <param name="cancellationToken"></param>
    /// <returns><c>true</c> - if username is not taken, <c>false</c> - if username is taken</returns>
    private async Task<bool> IsUserNameFree(string userName, CancellationToken cancellationToken = default) 
        => await GetUserByUserNameAsync(userName, false, cancellationToken) == null;
    
}