using Core.Abstractions;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Exceptions.Exceptions.Users;

namespace Application.Managers;

public class UserManager(IUserService userService, IUserRoleService userRoleService, IPasswordManager passwordManager,
    IUserRepository userRepository, IUnitOfWork unitOfWork) : UserManagerBase(unitOfWork)
{
    public override async Task<User?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
        => await userRepository.GetUserByIdAsync(userId, true, cancellationToken);
    public override async Task<User> CreateUserAsync(User user, string password, CancellationToken cancellationToken = default)
        => await userService.CreateUserAsync(user, passwordManager.GetHashOfPassword(password), cancellationToken);
    
    public override async Task<User> CreateUser(User user, CancellationToken cancellationToken = default)
        => await userService.CreateUserAsync(user, "", cancellationToken);
    
    public override async Task<User> ChangeLockoutEnd(Guid userId, DateTime? lockoutEnd, CancellationToken cancellationToken = default)
        => await userService.ChangeLockoutEnd(userId, lockoutEnd, cancellationToken);
    
    public override async Task<User> ChangeTwoFactorEnabled(Guid userId, bool twoFactorEnabled, CancellationToken cancellationToken = default)
        => await userService.ChangeTwoFactorEnabled(userId, twoFactorEnabled, cancellationToken);
    
    public override async Task<User> AccessFailed(Guid userId, CancellationToken cancellationToken = default)
        => await userService.AccessFailedAsync(userId, cancellationToken);

    public override async Task<User> ChangePassword(Guid id, string password, CancellationToken cancellationToken = default)
        => await userService.ChangePasswordHashAsync(id, passwordManager.GetHashOfPassword(password), cancellationToken);

    public override async Task AddToRole(Guid id, string role, CancellationToken cancellationToken = default)
        => await userRoleService.SetUserRole(id, role, cancellationToken);
    
    public override async Task<User> ChangeUserName(Guid id, string userName, CancellationToken cancellationToken = default)
        => await userService.ChangeUserNameAsync(id, userName, cancellationToken);
    
    public override async Task AddToRoles(Guid id, IEnumerable<string> roles, CancellationToken cancellationToken = default)
        => await userRoleService.SetUserRoles(id, roles, cancellationToken);

    public override async Task<bool> VerifyPassword(Guid id, string password, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetUserByIdAsync(id, false, cancellationToken)
                   ?? throw new UserNotFound(id);
        return passwordManager.VerifyHashedPassword(user.PasswordHash, password);
    }
    
}