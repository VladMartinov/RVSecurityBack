namespace Exceptions.Exceptions.Roles;

public class UnableRemoveSystemRoleException( Guid id, string roleName) :
    BadRequestException("Нельзя удалить системную роль", new { Id = id, RoleName = roleName })
{
    
}