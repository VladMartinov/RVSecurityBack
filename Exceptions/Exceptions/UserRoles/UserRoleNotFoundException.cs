namespace Exceptions.Exceptions.UserRoles;

public class UserRoleNotFoundException(Guid userId, Guid roleId) : 
    NotFoundException("Не удалось найти пользователя с указанной ролью", new { Userid = userId, Roleid = roleId })
{
    
}