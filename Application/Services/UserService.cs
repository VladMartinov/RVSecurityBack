using Application.Extensions;
using Core.Entities;
using Core.Extensions;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Exceptions.Exceptions.Users;
using Mapster;

namespace Application.Services;

public class UserService(IUserRepository userRepository, IUnitOfWork unitOfWork) : IUserService
{
    private async Task ValidateAsync(Guid? userId, string? userName, CancellationToken cancellationToken = default)
    {
        if (userId != null)
            await userRepository.EnsureUserExists(userId.Value, cancellationToken);
        if (userName != null)
        {
            var trimmedUserName = userName.Trim();
            if (trimmedUserName == string.Empty || trimmedUserName.Length < 3)
                throw new TooShortUserNameException(trimmedUserName, trimmedUserName.Length, 3);
            if (await userRepository.IsUserNameTakenAsync(trimmedUserName, cancellationToken))
                throw new UserNameAlreadyTaken(trimmedUserName);
        }
    }
    public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetUserByIdAsync(id, true, cancellationToken)
                           ?? throw new UserNotFound(id);
        unitOfWork.Remove(user);
    }

    public async Task<User> CreateUserAsync(User user, string passwordHash, CancellationToken cancellationToken = default)
    {
        var newUser = user.Adapt<User>();
        newUser.NormalizedUserName = newUser.UserName.ToNormalized();
        newUser.PasswordHash = passwordHash;
        
        await ValidateAsync(null, newUser.UserName, cancellationToken);
        await unitOfWork.AddAsync(newUser, cancellationToken);
        return newUser;
    }

    public async Task<User> ChangePasswordHashAsync(Guid userId, string passwordHash, CancellationToken cancellationToken = default)
        => await UpdateUserAsync(userId, u => u.PasswordHash = passwordHash, cancellationToken);
    

    public async Task<User> ChangeUserNameAsync(Guid userId, string userName, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetUserByIdAsync(userId, true, cancellationToken)
                   ?? throw new UserNotFound(userId);
        userName = userName.Trim();
        var normalizedUserName = userName.ToNormalized();
        await ValidateAsync(null, userName, cancellationToken);
        user.UserName = userName;
        user.NormalizedUserName = normalizedUserName;
        return user;
    }

    public async Task<User> ChangeLockoutEnd(Guid userId, DateTime? lockoutEnd, CancellationToken cancellationToken = default)
        => await UpdateUserAsync(userId, u => u.LockoutEnd = lockoutEnd, cancellationToken);

    public async Task<User> ChangeTwoFactorEnabled(Guid userId, bool twoFactorEnabled, CancellationToken cancellationToken = default)
        => await UpdateUserAsync(userId, u => u.TwoFactorEnabled = twoFactorEnabled, cancellationToken);

    public async Task<User> AccessFailedAsync(Guid userId, CancellationToken cancellationToken = default)
        => await UpdateUserAsync(userId, u => u.AccessFailedCount+=1, cancellationToken);
    
    private async Task<User> UpdateUserAsync(Guid userId, Action<User> update, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByIdAsync(userId, true, cancellationToken)
                   ?? throw new UserNotFound(userId);
        update(user);
        return user;
    }
}