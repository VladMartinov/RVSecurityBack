using Core.Entities;
using Core.Interfaces;

namespace Core.Abstractions;

public abstract class UserManagerBase : UnitOfWorkBase
{
    protected UserManagerBase(IUnitOfWork unitOfWork) : base(unitOfWork) { }

    public abstract Task<User?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);
    public abstract Task<User> CreateUserAsync(User user, string password, CancellationToken cancellationToken = default);
    public abstract Task<User> CreateUser(User user, CancellationToken cancellationToken = default);
    public abstract Task<User> ChangePassword(Guid id, string password, CancellationToken cancellationToken = default);
    public abstract Task AddToRole(Guid id, string role, CancellationToken cancellationToken = default);
    public abstract Task AddToRoles(Guid id, IEnumerable<string> roles, CancellationToken cancellationToken = default);
    public abstract Task<bool> VerifyPassword(Guid id, string password, CancellationToken cancellationToken = default);
    public abstract Task<User> ChangeUserName(Guid id, string userName, CancellationToken cancellationToken = default);
    public abstract Task<User> ChangeLockoutEnd(Guid userId, DateTime? lockoutEnd, CancellationToken cancellationToken = default);
    public abstract Task<User> ChangeTwoFactorEnabled(Guid userId, bool twoFactorEnabled, CancellationToken cancellationToken = default);
    public abstract Task<User> AccessFailed(Guid userId, CancellationToken cancellationToken = default);
}