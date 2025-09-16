namespace Exceptions.Exceptions.Phones;

public class UserPhoneNotFoundException(Guid id) : NotFoundException("Не удалось найти номер телефона", new { Id = id})
{
    
}