using Core.Interfaces;
using static BCrypt.Net.BCrypt;

namespace Security;

public class PasswordManager : IPasswordManager
{
    public string GetHashOfPassword(string password)
    {
        return HashPassword(password);
    }

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        return Verify(providedPassword, hashedPassword);
    }
}