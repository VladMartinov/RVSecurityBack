using Core.Entities;
using Core.Interfaces.Repositories;
using Exceptions.Exceptions.Tokens;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Extensions;

namespace Persistence.Repositories;

public class UserTokenRepository(UserDbContext context) : IUserTokenRepository
{
    public async Task<UserToken?> GetTokenByIdAsync(Guid id, bool track = true, CancellationToken cancellationToken = default) 
        => await context.UserTokens.ConfigureTracking(track).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<UserToken?> GetTokenByHashAsync(string hash, bool track = true, CancellationToken cancellationToken = default) 
        => await context.UserTokens.ConfigureTracking(track).FirstOrDefaultAsync(x => x.TokenHash == hash, cancellationToken);

    public async Task<UserToken> AddUserTokenAsync(UserToken token, CancellationToken cancellationToken = default)
    {
        await context.UserTokens.AddAsync(token, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return token;
    }

    public async Task<UserToken> UpdateUserTokenAsync(UserToken token, CancellationToken cancellationToken = default)
    {
        var userToken = await GetTokenByIdAsync(token.Id, true, cancellationToken);
        if (userToken == null) throw new TokenNotFoundException(token.Id);
        UpdateUserTokenFields(userToken, token);
        await context.SaveChangesAsync(cancellationToken);
        return userToken;
    }
    
    
    private static void UpdateUserTokenFields(UserToken existingToken, UserToken newToken)
    {
        existingToken.TokenHash = newToken.TokenHash;
        existingToken.Permissions = newToken.Permissions;
        existingToken.Type = newToken.Type;
        existingToken.ExpiresAt = newToken.ExpiresAt;
        existingToken.Revoked = newToken.Revoked;
        existingToken.RevokeReason = newToken.RevokeReason;
        existingToken.DeviceId = newToken.DeviceId;
        existingToken.IpAddress = newToken.IpAddress;
        existingToken.UserAgent = newToken.UserAgent;
        existingToken.UpdatedAt = DateTime.UtcNow;
        existingToken.UserId = newToken.UserId;
    }

    public async Task DeleteUserTokenAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userToken = await GetTokenByIdAsync(id, true, cancellationToken);
        if (userToken == null) throw new TokenNotFoundException(id);
        context.UserTokens.Remove(userToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> TokenExistsAsync(Guid id, CancellationToken cancellationToken = default) 
        => await context.UserTokens.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken);

    public async Task<bool> TokenExistsAsync(string hash, CancellationToken cancellationToken = default)
        => await context.UserTokens.AsNoTracking().AnyAsync(x => x.TokenHash == hash, cancellationToken);
}