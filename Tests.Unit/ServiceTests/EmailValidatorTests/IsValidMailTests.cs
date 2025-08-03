using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Unit.ServiceTests.EmailValidatorTests;

public class IsValidMailTests
{
    private readonly IEmailValidator _emailValidator;

    public IsValidMailTests()
    {
        var sp = ServiceProvider.Build();
        _emailValidator = sp.GetRequiredService<IEmailValidator>();
    }
    
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("USER@EXAMPLE.COM")]
    [InlineData("user.name+tag+sorting@example.com")]
    [InlineData("user_name@example.co.uk")]
    [InlineData("u@x.io")]
    public void ValidEmails_ShouldReturnTrue(string email)
    {
        var result = _emailValidator.IsValidEmail(email);
        Assert.True(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    public void NullOrWhitespaceEmails_ShouldReturnFalse(string email)
    {
        var result = _emailValidator.IsValidEmail(email);
        Assert.False(result);
    }

    [Theory]
    [InlineData("plainaddress")]
    [InlineData("@missing-local.org")]
    [InlineData("missing-at.com")]
    [InlineData("missingdomain@")]
    [InlineData("missing@dot")]
    [InlineData("user@.com")]
    [InlineData("user@domain..com")]
    [InlineData("user..double@domain.com")]
    [InlineData("user@-domain.com")]
    [InlineData("user@domain-.com")]
    [InlineData("user@domain.c")]
    public void InvalidEmails_ShouldReturnFalse(string email)
    {
        var result = _emailValidator.IsValidEmail(email);
        Assert.False(result);
    }

    [Fact]
    public void LocalPartTooLong_ShouldReturnFalse()
    {
        var localPart = new string('a', 65);
        var email = $"{localPart}@example.com";
        var result = _emailValidator.IsValidEmail(email);
        Assert.False(result);
    }

    [Fact]
    public void DomainTooLong_ShouldReturnFalse()
    {
        var domain = new string('b', 256);
        var email = $"user@{domain}.com";
        var result = _emailValidator.IsValidEmail(email);
        Assert.False(result);
    }

    [Fact]
    public void EmailTooLong_ShouldReturnFalse()
    {
        var email = $"{new string('a', 245)}@ex.com";
        var result = _emailValidator.IsValidEmail(email);
        Assert.False(result);
    }
}