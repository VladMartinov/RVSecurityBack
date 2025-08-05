namespace Exceptions.Exceptions.Roles;

public class TooLongRoleNameException(string name, int currentLength, int maximumLength) : BadRequestException("Название роли слишком большое", 
    new {Name = name, CurrentLength = currentLength, MaximumLength = maximumLength})
{
    
}