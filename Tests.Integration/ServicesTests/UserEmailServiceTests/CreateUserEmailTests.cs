using Bogus;
using Core.Dtos;
using Core.Dtos.UserEmail;
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
public class CreateUserEmailTests : IAsyncLifetime
{
    private readonly UserDbContext _context;
    private readonly IUserEmailService _service;
    private readonly Faker _faker;
    private User _user = null!;
    
    public CreateUserEmailTests(PostgresContainerFixture fixture)
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
    public async Task CreateUserEmail_WithValidData_Succeeds()
    {
        var dto = new UserEmailCreationDto(_user.Id, _faker.Internet.Email(), true, 
            EmailType.Personal, true);
        
        var result = await _service.CreateUserEmailAsync(dto);
        
        Assert.NotNull(result);
        Assert.Equal(dto.Email, result.Email);
        Assert.Equal(dto.UserId, result.UserId);
        Assert.True(result.Confirmed);
        Assert.True(result.IsPrimary);
        Assert.Equal(dto.EmailType.ToString(), result.EmailType);
        Assert.NotNull(result.ConfirmedAt);
    }
    
    [Fact]
    public async Task CreateUserEmail_WhenEmailLimitExceeded_ThrowsEmailCountLimitReachedException()
    {
        var emails = Enumerable.Range(0, 4).Select(_ => new UserEmailCreationDto(_user.Id, 
            _faker.Internet.Email(), false, EmailType.Personal, false));

        foreach (var dto in emails)
            await _service.CreateUserEmailAsync(dto);

        var newDto = new UserEmailCreationDto(
            UserId: _user.Id,
            Email: _faker.Internet.Email(),
            Confirmed: false,
            EmailType: EmailType.Personal,
            IsPrimary: false
        );
        
        await Assert.ThrowsAsync<EmailCountLimitReachedException>(() =>
            _service.CreateUserEmailAsync(newDto));
    }
    
    [Fact]
    public async Task CreateUserEmail_WithInvalidEmail_ThrowsEmailIsInvalidException()
    {
        var dto = new UserEmailCreationDto(_user.Id, "invalid-email", false,
            EmailType.Personal, false);
        
        await Assert.ThrowsAsync<EmailIsInvalidException>(() => _service.CreateUserEmailAsync(dto));
    }
    
    [Fact]
    public async Task CreateUserEmail_WhenMultiplePrimaryEmails_ThrowsMoreThenOnePrimaryEmailException()
    {
        var primaryDto = new UserEmailCreationDto(_user.Id, _faker.Internet.Email(),true, EmailType.Personal, true);

        await _service.CreateUserEmailAsync(primaryDto);

        var secondPrimaryDto = new UserEmailCreationDto(
            UserId: _user.Id,
            Email: _faker.Internet.Email(),
            Confirmed: true,
            EmailType: EmailType.Personal,
            IsPrimary: true
        );
        
        await Assert.ThrowsAsync<MoreThenOnePrimaryEmailException>(() =>
            _service.CreateUserEmailAsync(secondPrimaryDto));
    }
    
    [Fact]
    public async Task CreateUserEmail_WithDuplicateEmail_ThrowsEmailTakenException()
    {
        var email = _faker.Internet.Email();

        var dto = new UserEmailCreationDto(
            UserId: _user.Id,
            Email: email,
            Confirmed: true,
            EmailType: EmailType.Personal,
            IsPrimary: false
        );

        await _service.CreateUserEmailAsync(dto);

        var duplicateDto = new UserEmailCreationDto(
            UserId: _user.Id,
            Email: email,
            Confirmed: true,
            EmailType: EmailType.Personal,
            IsPrimary: false
        );

        await Assert.ThrowsAsync<EmailTakenException>(() =>
            _service.CreateUserEmailAsync(duplicateDto));
    }
}