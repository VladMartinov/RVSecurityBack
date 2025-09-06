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

    public async Task<bool> TokenExistsAsync(Guid id, CancellationToken cancellationToken = default) 
        => await context.UserTokens.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken);

    public async Task<bool> TokenExistsAsync(string hash, CancellationToken cancellationToken = default)
        => await context.UserTokens.AsNoTracking().AnyAsync(x => x.TokenHash == hash, cancellationToken);
}