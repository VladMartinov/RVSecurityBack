using Core.Entities;
using Core.Extensions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Exceptions.Exceptions.Users;

namespace Application.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await userRepository.DeleteUserAsync(id, cancellationToken);
    }

    public async Task<User> CreateUserAsync(User user, string passwordHash, CancellationToken cancellationToken = default)
    {
        var newUser = new User
        {
            UserName = user.UserName.Trim(),
            NormalizedUserName = user.UserName.ToNormalized()!,
            PasswordHash = passwordHash,
            TwoFactorEnabled = user.TwoFactorEnabled,
            LockoutEnd = user.LockoutEnd,
        };
        if (string.IsNullOrWhiteSpace(newUser.NormalizedUserName))
            throw new ArgumentNullException(nameof(newUser.NormalizedUserName), "Имя пользователя не может быть пустым");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentNullException(nameof(passwordHash), "Хэш пароля не может быть пустым");
        if (await userRepository.IsUserNameTakenAsync(user.UserName, cancellationToken))
            throw new UserNameAlreadyTaken(user.UserName);
        
        return await userRepository.AddUserAsync(newUser, cancellationToken);
    }

    public async Task<User> UpdateUserAsync(User user, string? passwordHash, CancellationToken cancellationToken = default)
    {
        user.UserName = user.UserName.Trim();
        var normalizedUserName = user.UserName.ToNormalized();

        if (string.IsNullOrWhiteSpace(normalizedUserName))
            throw new ArgumentNullException(nameof(user.UserName), "Имя пользователя не может быть пустым");
        
        var userToUpdate = await userRepository.GetUserByIdAsync(user.Id, true, cancellationToken)
                           ?? throw new UserNotFound(user.Id);

        if (userToUpdate.NormalizedUserName != normalizedUserName 
            && await userRepository.IsUserNameTakenAsync(user.UserName, cancellationToken))
            throw new UserNameAlreadyTaken(user.UserName);
        
        userToUpdate.UserName = user.UserName;
        userToUpdate.NormalizedUserName = normalizedUserName;
        userToUpdate.PasswordHash = string.IsNullOrWhiteSpace(passwordHash) ? userToUpdate.PasswordHash : passwordHash;
        userToUpdate.LockoutEnd = user.LockoutEnd;

        return await userRepository.UpdateUserAsync(userToUpdate, cancellationToken);
    }
}