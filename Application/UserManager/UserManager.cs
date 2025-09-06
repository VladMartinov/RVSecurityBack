using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;

namespace Application.UserManager;

public class UserManager(IUserService userService, IUserRoleService userRoleService) : IUserManager
{
    
}