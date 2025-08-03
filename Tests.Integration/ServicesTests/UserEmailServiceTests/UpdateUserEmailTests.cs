using Bogus;
using Core.Dtos;
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
public class UpdateUserEmailTests : IAsyncLifetime
{
    private readonly UserDbContext _context;
    private readonly IUserEmailService _service;
    private readonly Faker _faker;
    private User _user = null!;
    
    public UpdateUserEmailTests(PostgresContainerFixture fixture)
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
    public async Task UpdateUserEmail_WithValidData_UpdatesSuccessfully()
    {
        var email = _faker.Internet.Email();
        var created = await _service.CreateUserEmailAsync(new(_user.Id, email, false, EmailType.Work, false));

        var updatedEmail = _faker.Internet.Email();
        var dto = new UserEmailUpdateDto(
            Id: created.Id,
            UserId: _user.Id,
            Email: updatedEmail,
            IsPrimary: true,
            Type: EmailType.Personal,
            Confirmed: true
        );

        var result = await _service.UpdateUserEmailAsync(dto);

        Assert.Equal(updatedEmail, result.Email);
        Assert.Equal(_user.Id, result.UserId);
        Assert.True(result.IsPrimary);
        Assert.True(result.Confirmed);
        Assert.NotNull(result.ConfirmedAt);
    }
    
    [Fact]
    public async Task UpdateUserEmail_WithInvalidId_ThrowsUserEmailNotFoundException()
    {
        var dto = new UserEmailUpdateDto(
            Id: Guid.NewGuid(),
            UserId: _user.Id,
            Email: _faker.Internet.Email(),
            IsPrimary: false,
            Type: EmailType.Work,
            Confirmed: false
        );

        await Assert.ThrowsAsync<UserEmailNotFoundException>(() => _service.UpdateUserEmailAsync(dto));
    }
    
    [Fact]
    public async Task UpdateUserEmail_SetConfirmedTrue_SetsConfirmedAt()
    {
        var created = await _service.CreateUserEmailAsync(new(_user.Id, _faker.Internet.Email(), false, EmailType.Personal, false));

        var dto = new UserEmailUpdateDto(
            Id: created.Id,
            UserId: _user.Id,
            Email: created.Email,
            IsPrimary: created.IsPrimary,
            Type: EmailType.Personal,
            Confirmed: true
        );

        var result = await _service.UpdateUserEmailAsync(dto);

        Assert.True(result.Confirmed);
        Assert.NotNull(result.ConfirmedAt);
    }
    
    [Fact]
    public async Task UpdateUserEmail_ToDuplicateEmail_ThrowsEmailTakenException()
    {
        var email1 = _faker.Internet.Email();
        var email2 = _faker.Internet.Email();

        await _service.CreateUserEmailAsync(new(_user.Id, email1, false, EmailType.Personal, false));
        var created2 = await _service.CreateUserEmailAsync(new(_user.Id, email2, false, EmailType.Personal, false));

        var dto = new UserEmailUpdateDto(
            Id: created2.Id,
            UserId: _user.Id,
            Email: email1,
            IsPrimary: false,
            Type: EmailType.Personal,
            Confirmed: false
        );

        await Assert.ThrowsAsync<EmailTakenException>(() => _service.UpdateUserEmailAsync(dto));
    }
    
    [Fact]
    public async Task UpdateUserEmail_WhenPrimaryExists_ThrowsMoreThenOnePrimaryEmailException()
    {
        await _service.CreateUserEmailAsync(new(_user.Id, _faker.Internet.Email(), true, EmailType.Personal, true));
        var second = await _service.CreateUserEmailAsync(new(_user.Id, _faker.Internet.Email(), false, EmailType.Personal, false));

        var dto = new UserEmailUpdateDto(
            Id: second.Id,
            UserId: _user.Id,
            Email: second.Email,
            IsPrimary: true,
            Type: EmailType.Personal,
            Confirmed: false
        );

        await Assert.ThrowsAsync<MoreThenOnePrimaryEmailException>(() => _service.UpdateUserEmailAsync(dto));
    }
    
    [Fact]
    public async Task UpdateUserEmail_SetConfirmedFalse_DoesNotChangeConfirmedAt()
    {
        var email = _faker.Internet.Email();
        var created = await _service.CreateUserEmailAsync(new(_user.Id, email, false, EmailType.Personal, true));
    
        var originalConfirmedAt = created.ConfirmedAt;

        var dto = new UserEmailUpdateDto(
            Id: created.Id,
            UserId: _user.Id,
            Email: email,
            IsPrimary: created.IsPrimary,
            Type: EmailType.Personal,
            Confirmed: false
        );

        var result = await _service.UpdateUserEmailAsync(dto);

        Assert.False(result.Confirmed);
        Assert.Equal(originalConfirmedAt, result.ConfirmedAt);
    }
    
    [Fact]
    public async Task UpdateUserEmail_WithoutChangingEmailOrUserId_Succeeds()
    {
        var email = _faker.Internet.Email();
        var created = await _service.CreateUserEmailAsync(new(_user.Id, email, false, EmailType.Work, false));

        var dto = new UserEmailUpdateDto(
            Id: created.Id,
            Email: null, 
            UserId: null,
            IsPrimary: created.IsPrimary,
            Type: EmailType.Work,
            Confirmed: created.Confirmed
        );

        var result = await _service.UpdateUserEmailAsync(dto);

        Assert.Equal(created.Id, result.Id);
        Assert.Equal(created.Email, result.Email);
    }
}