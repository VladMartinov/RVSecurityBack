namespace Core.Interfaces;

public interface IPasswordValidator
{
    /// <summary>
    /// Проверяет пароль на корректность 
    /// </summary>
    /// <param name="password">Пароль который надо проверить</param>
    /// <returns>Ошибки если есть или пустой массив</returns>
    IEnumerable<string> Validate(string password);
}