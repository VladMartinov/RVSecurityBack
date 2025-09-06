using Bogus;
using Core.Dtos.User;
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
        var newHash = _faker.Random.String2(100);
        
        var dto = new UserUpdateDto(user.Id, user.UserName, newHash);
        await _userRepository.UpdateUserAsync(dto);
        
        var userInDb = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);
        Assert.NotNull(userInDb);
        Assert.Equal(user.UserName.Trim(), userInDb.UserName);
        Assert.Equal(newHash, userInDb.PasswordHash);
        Assert.Equal(userInDb.NormalizedUserName, user.UserName.ToNormalized());
    }
    
    [Fact]
    public async Task UpdateUser_WithTakenUserName_ThrowsUserNameAlreadyTaken()
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync();
        Assert.NotNull(user);
        
        var secondUser = await _context.CreateUser(_faker.Lorem.Letter(100));
        var hash = _faker.Lorem.Letter(100);
        secondUser.UserName = user.UserName;
        
        var dto = new UserUpdateDto(secondUser.Id, secondUser.UserName, hash);
        
        await Assert.ThrowsAsync<UserNameAlreadyTaken>(() =>
            _userRepository.UpdateUserAsync(dto));
    }
    
    [Fact]
    public async Task UpdateUser_WithEmptyUserName_Throws()
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync();
        Assert.NotNull(user);
        user.UserName = " ";
        var dto = new UserUpdateDto(user.Id, user.UserName, user.PasswordHash);
        await Assert.ThrowsAsync<TooShortUserNameException>(async () =>
            await _userRepository.UpdateUserAsync(dto)
        );
    }

    [Fact]
    public async Task UpdateUser_WithEmptyPasswordHash_Succeeds()
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync();
        Assert.NotNull(user);
        
        var dto = new UserUpdateDto(user.Id, user.UserName, "");
        var updatedUser = await _userRepository.UpdateUserAsync(dto);

        Assert.Equal("", updatedUser.PasswordHash);
    }
}