using Core.Entities;
using Core.Extensions;
using Persistence.Context;

namespace Tests.Integration.MockData;

public static class GenerateMockInDb
{
    public static async Task<User> CreateUser(this UserDbContext context, string passwordHash)
    {
        var user = MockUser.GenNewUsers(1).Single();
        user.NormalizedUserName = user.UserName.ToNormalized()!;
        user.PasswordHash = passwordHash;
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }
}