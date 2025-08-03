namespace Exceptions.Exceptions.Roles;

public class RoleNotFoundException(Guid id) : NotFoundException("Не удалось найти роль", new { Id = id })
{
    
}