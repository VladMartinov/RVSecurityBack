using Core.Dtos.User;
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
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Созданный пользователь с присвоенным Id.</returns>
    Task<User> CreateUserAsync(UserCreationDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновление существующего пользователя.
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Обновлённый пользователь.</returns>
    Task<User> UpdateUserAsync(UserUpdateDto dto, CancellationToken cancellationToken = default);
}