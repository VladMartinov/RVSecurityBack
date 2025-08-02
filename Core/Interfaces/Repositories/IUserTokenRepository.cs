using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface IUserTokenRepository
{
    Task<UserToken?> GetTokenByIdAsync(Guid id, bool track = true, CancellationToken cancellationToken = default);
    Task<UserToken?> GetTokenByHashAsync(string hash, bool track = true, CancellationToken cancellationToken = default);
    Task<UserToken> CreateUserTokenAsync(UserToken token, CancellationToken cancellationToken = default);
    Task<UserToken> UpdateUserTokenAsync(UserToken token, CancellationToken cancellationToken = default);
    Task DeleteUserTokenAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> TokenExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> TokenExistsAsync(string hash, CancellationToken cancellationToken = default);
}