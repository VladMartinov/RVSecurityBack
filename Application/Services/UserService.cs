using Application.Extensions;
using Core.Dtos.User;
using Core.Entities;
using Core.Extensions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Exceptions.Exceptions.Users;
using Mapster;

namespace Application.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private async Task ValidateAsync(Guid? userId, string? userName, string passwordHash, CancellationToken cancellationToken = default)
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

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new InvalidPasswordHashException();
    }
    public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await userRepository.DeleteUserAsync(id, cancellationToken);
    }

    public async Task<User> CreateUserAsync(UserCreationDto dto, CancellationToken cancellationToken = default)
    {
        var newUser = dto.Adapt<User>();
        newUser.PasswordHash = dto.PasswordHash;

        await ValidateAsync(null, newUser.UserName, newUser.PasswordHash, cancellationToken);
        return await userRepository.AddUserAsync(newUser, cancellationToken);
    }

    public async Task<User> UpdateUserAsync(UserUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var userName = dto.UserName?.Trim();
        var normalizedUserName = userName?.ToNormalized();
        
        var userToUpdate = await userRepository.GetUserByIdAsync(dto.Id, true, cancellationToken)
                           ?? throw new UserNotFound(dto.Id);

        var userNameChanged = userName != null && userToUpdate.NormalizedUserName != normalizedUserName;
        var passwordHash = dto.PasswordHash ?? userToUpdate.PasswordHash;
        
        await ValidateAsync(dto.Id, userNameChanged ? userName : userToUpdate.UserName, passwordHash, cancellationToken);
        
        userToUpdate.PasswordHash = passwordHash;

        if (userNameChanged)
        {
            userToUpdate.UserName = userName!;
            userToUpdate.NormalizedUserName = normalizedUserName!;
        }
        

        return await userRepository.UpdateUserAsync(userToUpdate, cancellationToken);
    }
}