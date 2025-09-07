using Bogus;
using Core.Extensions;
using Core.Interfaces.Services;
using Exceptions.Exceptions.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using Tests.Integration.Extensions;
using Tests.Integration.MockData;
using Tests.Integration.TestContainers.Pg;

namespace Tests.Integration.ServicesTests.UserServiceTests;

[Collection("Postgres collection")]
public class CreateUserTests : IAsyncLifetime
{
    private readonly UserDbContext _context;
    private readonly IUserService _userRepository;
    private readonly Faker _faker;
    public CreateUserTests(PostgresContainerFixture fixture)
    {
        var sp = ServiceProvider.Build(fixture.ConnectionString);
        _context = sp.GetRequiredService<UserDbContext>();
        _userRepository = sp.GetRequiredService<IUserService>();
        _faker = new Faker(Mock.Locale);
    }
    
    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _context.ClearDatabaseFull();
    }

    [Fact]
    public async Task CreateUser_WithValidData_Succeeds()
    {
        var user = MockUser.GenNewUsers(1).Single();
        var passwordHash = _faker.Lorem.Letter(100);
        
        var createdUser = await _userRepository.CreateUserAsync(user, passwordHash);
        var userInDb = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == createdUser.Id);
        
        Assert.NotNull(userInDb);
        Assert.Equal(user.UserName, createdUser.UserName);
        Assert.Equal(user.UserName.ToNormalized(), createdUser.NormalizedUserName);
        Assert.Equal(passwordHash, createdUser.PasswordHash);
        Assert.Equal(passwordHash, userInDb.PasswordHash);
        Assert.Equal(user.TwoFactorEnabled, createdUser.TwoFactorEnabled);
        Assert.Equal(user.LockoutEnd, createdUser.LockoutEnd);
        Assert.True(userInDb.CreatedAt <= DateTime.UtcNow && userInDb.CreatedAt >= DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public async Task CreateUser_WithEmptyUserName_Throws()
    {
        var user = MockUser.GenNewUsers(1).Single();
        user.UserName = "   ";
        var passwordHash = _faker.Lorem.Letter(100);

        await Assert.ThrowsAsync<TooShortUserNameException>(async () =>
            await _userRepository.CreateUserAsync(user, passwordHash)
        );
    }

    [Fact]
    public async Task CreateUser_WithEmptyPasswordHash_Succeeds()
    {
        var user = MockUser.GenNewUsers(1).Single();
        var emptyPasswordHash = "";

        var us = await _userRepository.CreateUserAsync(user, emptyPasswordHash);
        
        var userInDb = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == us.Id);
        
        Assert.NotNull(userInDb);
        Assert.Equal(user.UserName, userInDb.UserName);
        Assert.Equal(user.UserName.ToNormalized(), userInDb.NormalizedUserName);
        Assert.Equal(emptyPasswordHash, userInDb.PasswordHash);
        Assert.Equal(user.TwoFactorEnabled, userInDb.TwoFactorEnabled);
    }

    [Fact]
    public async Task CreateUser_WithTakenUserName_Throws()
    {
        var user = await _context.CreateUser(_faker.Lorem.Letter(100));

        await Assert.ThrowsAsync<UserNameAlreadyTaken>(async () =>
            await _userRepository.CreateUserAsync(user, _faker.Lorem.Letter(100))
        );
    }
}