namespace Exceptions.Exceptions.Roles;

public class InvalidRoleChangeException(Guid id) : BadRequestException("Нельзя снять системность с системной роли", new { Id = id })
{
    
}