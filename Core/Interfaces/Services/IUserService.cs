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
    /// <param name="user">Объект пользователя (без Id).</param>
    /// <param name="passwordHash">Хэш пароля пользователя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Созданный пользователь с присвоенным Id.</returns>
    Task<User> CreateUserAsync(User user, string passwordHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновление существующего пользователя.
    /// </summary>
    /// <param name="user">Пользователь с обновлёнными данными.</param>
    /// <param name="passwordHash"></param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Обновлённый пользователь.</returns>
    Task<User> UpdateUserAsync(User user, string? passwordHash, CancellationToken cancellationToken = default);
}