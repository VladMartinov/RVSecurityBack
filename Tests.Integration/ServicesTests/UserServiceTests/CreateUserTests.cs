using Bogus;
using Core.Exceptions.Users;
using Core.Extensions;
using Core.Interfaces.Services;
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

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _userRepository.CreateUserAsync(user, passwordHash)
        );

        Assert.Equal("NormalizedUserName", ex.ParamName);
    }

    [Fact]
    public async Task CreateUser_WithEmptyPasswordHash_Throws()
    {
        var user = MockUser.GenNewUsers(1).Single();
        var emptyPasswordHash = "";

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _userRepository.CreateUserAsync(user, emptyPasswordHash)
        );

        Assert.Equal("passwordHash", ex.ParamName);
    }

    [Fact]
    public async Task CreateUser_WithTakenUserName_Throws()
    {
        var user = await _context.CreateUser(_faker.Lorem.Letter(100));
        
        var duplicate = MockUser.GenNewUsers(1).Single();
        duplicate.UserName = user.UserName;

        await Assert.ThrowsAsync<UserNameAlreadyTaken>(async () =>
            await _userRepository.CreateUserAsync(duplicate, _faker.Lorem.Letter(100))
        );
    }
}