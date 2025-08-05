using Core.Dtos;
using Core.Dtos.UserToken;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Exceptions.Exceptions.Tokens;
using Exceptions.Exceptions.Users;
using Mapster;

namespace Application.Services;

public class UserTokenService(IUserTokenRepository userTokenRepository, IUserRepository userRepository) : IUserTokenService
{
    private async Task Validate(UserTokenCreationDto tokenCreationRequest, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tokenCreationRequest.TokenHash))
            throw new InvalidTokenException();
        if (tokenCreationRequest.TokenInfo.Permissions.Count == 0)
            throw new EmptyPermissionsException();
        if (!await userRepository.UserExists(tokenCreationRequest.UserId, cancellationToken))
            throw new UserNotFound(tokenCreationRequest.UserId);
        if (await userTokenRepository.TokenExistsAsync(tokenCreationRequest.TokenHash, cancellationToken))
            throw new TokenHashAlreadyTakenException();
        
    }
    
    public async Task<UserToken> CreateUserTokenAsync(UserTokenCreationDto tokenCreationRequest, CancellationToken cancellationToken = default)
    {
        await Validate(tokenCreationRequest, cancellationToken);
        var userToken = tokenCreationRequest.Adapt<UserToken>();
        return await userTokenRepository.AddUserTokenAsync(userToken, cancellationToken);
    }

    public async Task<UserToken> UpdateUserTokenAsync(UserTokenUpdateDto tokenUpdateRequest, CancellationToken cancellationToken = default)
    {
        var token = await userTokenRepository.GetTokenByIdAsync(tokenUpdateRequest.Id, true, cancellationToken)
            ?? throw new TokenNotFoundException(tokenUpdateRequest.Id);
        
        token.ExpiresAt = tokenUpdateRequest.ExpiresAt;
        token.DeviceId = tokenUpdateRequest.DeviceInfo.DeviceId;
        token.UserAgent = tokenUpdateRequest.DeviceInfo.UserAgent;
        token.IpAddress = tokenUpdateRequest.DeviceInfo.IpAddress;
        token.Revoked = tokenUpdateRequest.RevokeInfo.IsRevoked;
        token.RevokeReason = tokenUpdateRequest.RevokeInfo.RevokeReason;
        
        return await userTokenRepository.UpdateUserTokenAsync(token, cancellationToken);
    }

    public async Task DeleteUserTokenAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await userTokenRepository.DeleteUserTokenAsync(id, cancellationToken);
    }
}