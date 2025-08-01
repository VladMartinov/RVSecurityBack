using Bogus;
using Core.Entities;
using Core.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using Tests.Integration.Extensions;
using Tests.Integration.MockData;
using Tests.Integration.TestContainers.Pg;

namespace Tests.Integration.RepositoryTests.UserRepositoryTests;

[Collection("Postgres collection")]
public class GetUserTests : IAsyncLifetime
{
    private readonly UserDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly Faker _faker;
    private User _user = null!;

    public GetUserTests(PostgresContainerFixture fixture)
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
    public async Task GetUserById_WithValidData_ReturnsUser()
    {
        var userId = _user.Id;
        var user = await _userRepository.GetUserByIdAsync(userId);
        Assert.NotNull(user);
        Assert.Equal(_user.NormalizedUserName, user.NormalizedUserName);
        Assert.Equal(_user.Id, user.Id);
    }
    
    [Fact]
    public async Task GetUserById_WithInvalidData_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var user = await _userRepository.GetUserByIdAsync(userId);
        Assert.Null(user);
    }
}