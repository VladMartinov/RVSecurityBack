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
public class UpdateUserTests : IAsyncLifetime
{
    private readonly UserDbContext _context;
    private readonly IUserService _userRepository;
    private readonly Faker _faker;

    public UpdateUserTests(PostgresContainerFixture fixture)
    {
        var sp = ServiceProvider.Build(fixture.ConnectionString);
        _context = sp.GetRequiredService<UserDbContext>();
        _userRepository = sp.GetRequiredService<IUserService>();
        _faker = new Faker(Mock.Locale);
    }
    
    public async Task InitializeAsync()
    {
        await _context.CreateUser(_faker.Lorem.Letter(100));
    }

    public async Task DisposeAsync()
    {
        await _context.ClearDatabaseFull();
    }
    
    [Fact]
    public async Task UpdateUser_WithValidData_UpdatesSuccessfully()
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync();
        Assert.NotNull(user);
        
        user.UserName = _faker.Internet.UserName();
        user.LockoutEnd = DateTime.UtcNow.AddDays(1);
        var newHash = _faker.Random.String2(100);
        var updatedUser = await _userRepository.UpdateUserAsync(user, newHash);
        
        var userInDb = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);
        Assert.NotNull(userInDb);
        Assert.Equal(user.UserName.Trim(), userInDb.UserName);
        Assert.Equal(newHash, userInDb.PasswordHash);
        Assert.Equal(userInDb.NormalizedUserName, user.UserName.ToNormalized());
        Assert.NotNull(userInDb.LockoutEnd);
        Assert.True(Math.Abs((userInDb.LockoutEnd.Value - user.LockoutEnd.Value).TotalMilliseconds) < 1);
    }
    
    [Fact]
    public async Task UpdateUser_WithTakenUserName_ThrowsUserNameAlreadyTaken()
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync();
        Assert.NotNull(user);
        var secondUser = await _context.CreateUser(_faker.Lorem.Letter(100));
        var hash = _faker.Lorem.Letter(100);
        secondUser.UserName = user.UserName;
        
        await Assert.ThrowsAsync<UserNameAlreadyTaken>(() =>
            _userRepository.UpdateUserAsync(secondUser, hash));
    }
    
    [Fact]
    public async Task UpdateUser_WithEmptyUserName_Throws()
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync();
        Assert.NotNull(user);
        user.UserName = " ";
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _userRepository.UpdateUserAsync(user, user.PasswordHash)
        );

        Assert.Equal("UserName", ex.ParamName);
    }

    [Fact]
    public async Task UpdateUser_WithEmptyPasswordHash_Succeeds()
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync();
        Assert.NotNull(user);
        var updatedUser = await _userRepository.UpdateUserAsync(user, "");

        Assert.Equal(user.PasswordHash, updatedUser.PasswordHash);
    }
}