namespace Exceptions.Exceptions.Tokens;

public class EmptyPermissionsException() : BadRequestException("Права доступа не могут быть пустыми")
{
    
}