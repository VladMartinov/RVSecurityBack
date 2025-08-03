namespace Exceptions.Exceptions.Emails;

public class EmailTakenException(string? email) : BadRequestException("Данная поста уже занята другим пользователем.", new { Email = email })
{
    
}