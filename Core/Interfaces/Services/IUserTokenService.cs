using Core.Dtos;
using Core.Dtos.UserToken;
using Core.Entities;

namespace Core.Interfaces.Services;

public interface IUserTokenService
{
    Task<UserToken> CreateUserTokenAsync(UserTokenCreationDto tokenCreationRequest, CancellationToken cancellationToken = default);
    Task<UserToken> UpdateUserTokenAsync(UserTokenUpdateDto tokenUpdateRequest, CancellationToken cancellationToken = default);
    Task DeleteUserTokenAsync(Guid id, CancellationToken cancellationToken = default);
}