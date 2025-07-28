using Core.Entities;

namespace Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(Guid userId, bool track = true, CancellationToken cancellationToken = default);
    Task<User?> GetUserByUserNameAsync(string userName, bool track = true, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, bool track = true, CancellationToken cancellationToken = default);
    Task<User?> GetUserByPhoneAsync(string phoneNumber, bool track = true, CancellationToken cancellationToken = default);
}