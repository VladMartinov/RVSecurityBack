namespace Core.Exceptions.Users;

public class UserNameAlreadyTaken(string userName) : BadRequestException("Имя пользователя уже занято", new { UserName = userName })
{
    
}