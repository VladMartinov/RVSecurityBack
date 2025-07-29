namespace Core.Exceptions.Users;

public class UserNotFound(Guid id) : NotFoundException("Пользователь не был найден.", new { Id = id})
{
    
}