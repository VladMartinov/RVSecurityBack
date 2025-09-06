using Application.Extensions;
using Core.Dtos.User;
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
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<User> CreateUserAsync(UserCreationDto dto, CancellationToken cancellationToken = default)
    {
        var newUser = dto.Adapt<User>();
        newUser.PasswordHash = dto.PasswordHash;
        newUser.NormalizedUserName = dto.UserName.ToNormalized();

        await ValidateAsync(null, newUser.UserName, cancellationToken);
        await unitOfWork.AddAsync(newUser, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return newUser;
    }

    public async Task<User> UpdateUserAsync(UserUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var userName = dto.UserName?.Trim();
        var normalizedUserName = userName?.ToNormalized();
        
        var userToUpdate = await userRepository.GetUserByIdAsync(dto.Id, true, cancellationToken)
                           ?? throw new UserNotFound(dto.Id);

        var userNameChanged = userName != null && userToUpdate.NormalizedUserName != normalizedUserName;
        userName ??= "";
        var passwordHash = dto.PasswordHash ?? userToUpdate.PasswordHash;
        
        await ValidateAsync(dto.Id, userNameChanged ? userName : null, cancellationToken);
        
        userToUpdate.PasswordHash = passwordHash;

        if (userNameChanged)
        {
            userToUpdate.UserName = userName;
            userToUpdate.NormalizedUserName = normalizedUserName!;
        }
        
        userToUpdate.UpdatedAt = DateTime.UtcNow;
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return userToUpdate;
    }
}