using Core.Entities;

namespace Core.Interfaces;

public interface IUserRepository
{
    /// <summary>
    /// Получение пользователя по Id.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="track">Флаг отслеживания сущности.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Пользователь или null.</returns>
    Task<User?> GetUserByIdAsync(Guid userId, bool track = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение пользователя по имени пользователя.
    /// </summary>
    /// <param name="userName">Имя пользователя.</param>
    /// <param name="track">Флаг отслеживания сущности.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Пользователь или null.</returns>
    Task<User?> GetUserByUserNameAsync(string userName, bool track = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение пользователя по email.
    /// </summary>
    /// <param name="email">Email пользователя.</param>
    /// <param name="track">Флаг отслеживания сущности.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Пользователь или null.</returns>
    Task<User?> GetUserByEmailAsync(string email, bool track = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение пользователя по номеру телефона.
    /// </summary>
    /// <param name="phoneNumber">Номер телефона пользователя.</param>
    /// <param name="track">Флаг отслеживания сущности.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Пользователь или null.</returns>
    Task<User?> GetUserByPhoneAsync(string phoneNumber, bool track = true, CancellationToken cancellationToken = default);
    
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
    /// <param name="passwordHash">Новый хэш пароля.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Обновлённый пользователь.</returns>
    Task<User> UpdateUserAsync(User user, string passwordHash, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удаление пользователя по Id.
    /// </summary>
    /// <param name="id">Id пользователя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
}