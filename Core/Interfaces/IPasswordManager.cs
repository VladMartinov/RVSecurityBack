namespace Core.Interfaces;

public interface IPasswordManager
{
    /// <summary>
    /// Получить хеш пароля.
    /// </summary>
    string GetHashOfPassword(string password);

    /// <summary>
    /// Проверить соответствие пароля и хеша.
    /// </summary>
    bool VerifyHashedPassword(string hashedPassword, string providedPassword);
}