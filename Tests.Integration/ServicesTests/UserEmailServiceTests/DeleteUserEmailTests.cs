using Bogus;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Exceptions.Exceptions.Emails;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using Tests.Integration.Extensions;
using Tests.Integration.MockData;
using Tests.Integration.TestContainers.Pg;

namespace Tests.Integration.ServicesTests.UserEmailServiceTests;

[Collection("Postgres collection")]
public class DeleteUserEmailTests : IAsyncLifetime
{
    private readonly UserDbContext _context;
    private readonly IUserEmailService _service;
    private readonly Faker _faker;
    private User _user = null!;
    
    public DeleteUserEmailTests(PostgresContainerFixture fixture)
    {
        var sp = ServiceProvider.Build(fixture.ConnectionString);
        _context = sp.GetRequiredService<UserDbContext>();
        _service = sp.GetRequiredService<IUserEmailService>();
        _faker = new Faker(Mock.Locale);
    }
    
    public async Task InitializeAsync()
    {
        _user = await _context.CreateUser("some_hash");
    }

    public async Task DisposeAsync()
    {
        await _context.ClearDatabaseFull();
    }
    
    [Fact]
    public async Task DeleteUserEmail_WhenEmailExists_DeletesSuccessfully()
    {
        var email = _faker.Internet.Email();
        var created = await _service.CreateUserEmailAsync(new(_user.Id, email, false, EmailType.Personal, false));

        await _service.DeleteUserEmailAsync(created.Id);

        var deleted = await _context.UserEmails.FindAsync(created.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteUserEmail_WhenEmailDoesNotExist_DoesNothingOrThrows()
    {
        var id = Guid.NewGuid();
        
        await Assert.ThrowsAsync<UserEmailNotFoundException>(() => _service.DeleteUserEmailAsync(id));
    }

    [Fact]
    public async Task DeleteUserEmail_PrimaryEmail_DeletesSuccessfully()
    {
        var email = _faker.Internet.Email();
        var created = await _service.CreateUserEmailAsync(new(_user.Id, email, false, EmailType.Work, true));

        await _service.DeleteUserEmailAsync(created.Id);

        var deleted = await _context.UserEmails.FindAsync(created.Id);
        Assert.Null(deleted);
    }
}