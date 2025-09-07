using Core.Entities;

namespace Core.Interfaces.Services;

public interface IUserService
{
    /// <summary>
    /// Удаление пользователя по Id.
    /// </summary>
    /// <param name="id">Id пользователя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создание нового пользователя.
    /// </summary>
    /// <param name="passwordHash"></param>
    /// <param name="user"></param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Созданный пользователь с присвоенным Id.</returns>
    Task<User> CreateUserAsync(User user, string passwordHash, CancellationToken cancellationToken = default);
    Task<User> ChangePasswordHashAsync(Guid userId, string passwordHash, CancellationToken cancellationToken = default);
    Task<User> ChangeUserNameAsync(Guid userId, string userName, CancellationToken cancellationToken = default);
    Task<User> ChangeLockoutEnd(Guid userId, DateTime? lockoutEnd, CancellationToken cancellationToken = default);
    Task<User> ChangeTwoFactorEnabled(Guid userId, bool twoFactorEnabled, CancellationToken cancellationToken = default);
    Task<User> AccessFailedAsync(Guid userId, CancellationToken cancellationToken = default);
}