using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Unit.ServiceTests.PhoneValidatorTests;

public class ValidateTests
{
    private readonly IPhoneValidator _validator;

    public ValidateTests()
    {
        var sp = ServiceProvider.Build();
        _validator = sp.GetRequiredService<IPhoneValidator>();
    }
    
    [Theory]
    [InlineData("+1 (555) 123-4567")]
    [InlineData("+44 20 7946 0958")]
    [InlineData("8 (800) 555-35-35")]
    [InlineData("89001234567")]
    [InlineData("+7 912 345-67-89")]
    [InlineData("+123456789012")]
    public void ValidPhones_ShouldReturnTrue(string phone)
    {
        var result = _validator.IsValidPhone(phone);
        Assert.True(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void NullOrWhitespacePhones_ShouldReturnFalse(string phone)
    {
        var result = _validator.IsValidPhone(phone);
        Assert.False(result);
    }

    [Theory]
    [InlineData("abc1234567")]
    [InlineData("123-abc-7890")]
    [InlineData("+()--")]
    [InlineData("123456")] 
    [InlineData("+1 (555) 123-4567-9999-0000-1111")]
    public void InvalidPhones_ShouldReturnFalse(string phone)
    {
        var result = _validator.IsValidPhone(phone);
        Assert.False(result);
    }

    [Fact]
    public void PhoneWithOnlySymbols_ShouldReturnFalse()
    {
        var result = _validator.IsValidPhone("++++----");
        Assert.False(result);
    }

    [Fact]
    public void PhoneWithLessThanSevenDigits_ShouldReturnFalse()
    {
        var result = _validator.IsValidPhone("123-45-6");
        Assert.False(result);
    }
}