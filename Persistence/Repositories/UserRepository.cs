using Core.Entities;
using Core.Extensions;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Extensions;

namespace Persistence.Repositories;

public class UserRepository(UserDbContext context) : IUserRepository
{
    public async Task<User?> GetUserByIdAsync(Guid userId, bool track = true, CancellationToken cancellationToken = default) 
        => await context.Users.ConfigureTracking(track).FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

    public async Task<User?> GetUserByUserNameAsync(string userName, bool track = true, CancellationToken cancellationToken = default)
        => await context.Users.ConfigureTracking(track).FirstOrDefaultAsync(x => x.NormalizedUserName == userName.ToNormalized(), cancellationToken);

    public async Task<User?> GetUserByEmailAsync(string email, bool track = true, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToNormalizedEmail();
        var userEmail = await context.UserEmails
            .ConfigureTracking(track)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
        return userEmail?.User;
    }

    public async Task<User?> GetUserByPhoneAsync(string phoneNumber, bool track = true, CancellationToken cancellationToken = default)
    {
        var normalizedPhone = phoneNumber.ToNormalizedPhoneNumber();
        var userPhone = await context.UserPhones.ConfigureTracking(track)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.NormalizedPhone == normalizedPhone, cancellationToken);
        return userPhone?.User;
    }
}