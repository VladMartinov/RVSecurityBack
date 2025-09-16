namespace Exceptions.Exceptions.Emails;

public class UserEmailNotFoundException(Guid id) : 
    NotFoundException("Не удалось найти почту", new { Id = id })
{
    
}