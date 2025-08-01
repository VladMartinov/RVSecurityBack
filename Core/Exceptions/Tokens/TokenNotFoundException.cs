namespace Core.Exceptions.Tokens;

public class TokenNotFoundException(Guid id) : NotFoundException("Не удалось найти токен", new { Id = id })
{
    
}