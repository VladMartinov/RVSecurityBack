namespace Exceptions.Exceptions.Roles;

public class TooShortRoleNameException(string name, int minLength, int currentLength) : 
    BadRequestException("Название роли слишком короткое", new { Name = name, MinLength = minLength, CurrentLength = currentLength })
{
    
}