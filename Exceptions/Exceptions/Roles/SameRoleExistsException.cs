namespace Exceptions.Exceptions.Roles;

public class SameRoleExistsException(string name) : 
    BadRequestException("Роль с таким названием уже существует", new { Name = name })
{
    
}