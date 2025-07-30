using Bogus;
using Core.Entities;
using Core.Exceptions.Users;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using Tests.Integration.Extensions;
using Tests.Integration.MockData;
using Tests.Integration.TestContainers.Pg;

namespace Tests.Integration.RepositoryTests.UserRepositoryTests;

[Collection("Postgres collection")]
public class DeleteUserTests : IAsyncLifetime
{
    private readonly UserDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly Faker _faker;
    private User _user = null!;

    public DeleteUserTests(PostgresContainerFixture fixture)
    {
        var sp = ServiceProvider.Build(fixture.ConnectionString);
        _context = sp.GetRequiredService<UserDbContext>();
        _userRepository = sp.GetRequiredService<IUserRepository>();
        _faker = new Faker(Mock.Locale);
    }
    
    public async Task InitializeAsync()
    {
        _user = await _context.CreateUser(_faker.Lorem.Letter(100));
    }

    public async Task DisposeAsync()
    {
        await _context.ClearDatabaseFull();
    }
    
    [Fact]
    public async Task DeleteUser_WithValidId_DeletesSuccessfully()
    {
        await _userRepository.DeleteUserAsync(_user.Id);
        
        var userInDb = await _context.Users.FirstOrDefaultAsync(x => x.Id == _user.Id);
        Assert.Null(userInDb);
    }

    [Fact]
    public async Task DeleteUser_WithInvalidId_ThrowsUserNotFound()
    {
        var nonExistentId = Guid.NewGuid();
        
        await Assert.ThrowsAsync<UserNotFound>(() =>
            _userRepository.DeleteUserAsync(nonExistentId));
    }
}