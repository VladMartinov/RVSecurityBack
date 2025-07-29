using Core.Interfaces;
using Core.Models;

namespace Security;

public class PasswordValidator(PasswordRules rules) : IPasswordValidator
{
    public IEnumerable<string> Validate(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Пароль не должен быть пустым.");
            return errors;
        }

        if (password.Length < rules.MinLength)
            errors.Add($"Пароль должен быть не короче {rules.MinLength} символов.");

        if (rules.MaxLength is not null && password.Length > rules.MaxLength)
            errors.Add($"Пароль должен быть не длиннее {rules.MaxLength} символов.");

        if (rules.RequireUppercase && !password.Any(char.IsUpper))
            errors.Add("Пароль должен содержать хотя бы одну заглавную букву.");

        if (rules.RequireDigit && !password.Any(char.IsDigit))
            errors.Add("Пароль должен содержать хотя бы одну цифру.");

        if (rules.RequireSpecial && !password.Any(c => rules.SpecialCharacters.Contains(c)))
            errors.Add("Пароль должен содержать хотя бы один специальный символ.");

        return errors;
    }
}