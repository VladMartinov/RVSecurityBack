namespace Exceptions.Exceptions.Users;

public class TooShortUserNameException(string? userName, int currentLength, int minLength) : 
    BadRequestException("Имя пользователя слишком короткое", new { UserName = userName, CurrentLength = currentLength, MinLength = minLength })
{
    
}