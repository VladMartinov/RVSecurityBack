using Core.Entities;

namespace Core.Interfaces;

public interface IUserManager
{
    Task<User?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User> CreateUserAsync(User user, string password, CancellationToken cancellationToken = default);
    Task<User> CreateUser(User user, CancellationToken cancellationToken = default);
    Task<User> ChangePassword(Guid id, string password, CancellationToken cancellationToken = default);
    Task AddToRole(Guid id, string role, CancellationToken cancellationToken = default);
    Task AddToRoles(Guid id, IEnumerable<string> roles, CancellationToken cancellationToken = default);
    Task<bool> VerifyPassword(Guid id, string password, CancellationToken cancellationToken = default);
    Task<User> ChangeUserName(Guid id, string userName, CancellationToken cancellationToken = default);
    Task<User> ChangeLockoutEnd(Guid userId, DateTime? lockoutEnd, CancellationToken cancellationToken = default);
    Task<User> ChangeTwoFactorEnabled(Guid userId, bool twoFactorEnabled, CancellationToken cancellationToken = default);
    Task<User> AccessFailed(Guid userId, CancellationToken cancellationToken = default);
}