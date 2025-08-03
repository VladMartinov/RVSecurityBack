namespace Exceptions.Exceptions.Emails;

public class EmailIsInvalidException(string email) : BadRequestException("Почта не является действительной", new { Email = email })
{
    
}