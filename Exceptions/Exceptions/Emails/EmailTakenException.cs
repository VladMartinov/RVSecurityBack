namespace Exceptions.Exceptions.Emails;

public class EmailTakenException(string? email) : BadRequestException("Данная почта уже занята другим пользователем.", new { Email = email })
{
    
}