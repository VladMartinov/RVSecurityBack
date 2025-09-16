using Core.Interfaces;
using Core.Models;
using Security;

namespace Tests.Unit.ServiceTests.PasswordValidatorTests;

public class ValidateTests
{
    private readonly PasswordRules _rules = new()
    {
        MinLength = 8,
        MaxLength = 20,
        RequireDigit = true,
        RequireUppercase = true,
        RequireSpecial = true,
        SpecialCharacters = "!@#$%^&*"
    };

    private IPasswordValidator CreateValidator(PasswordRules? rules = null)
    {
        return new PasswordValidator(rules ?? _rules);
    }

    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    public void Validate_ShouldReturnError_WhenPasswordIsEmpty(string password)
    {
        var validator = CreateValidator();
        var errors = validator.Validate(password).ToList();

        Assert.Single(errors);
        Assert.Contains("не должен быть пустым", errors[0]);
    }

    [Fact]
    public void Validate_ShouldReturnAllErrors_WhenPasswordIsTotallyInvalid()
    {
        var password = "abc";
        var validator = CreateValidator();

        var errors = validator.Validate(password).ToList();

        Assert.Contains(errors, e => e.Contains("не короче"));
        Assert.Contains(errors, e => e.Contains("заглавную"));
        Assert.Contains(errors, e => e.Contains("цифру"));
        Assert.Contains(errors, e => e.Contains("специальный символ"));
    }

    [Fact]
    public void Validate_ShouldReturnNoErrors_WhenPasswordIsValid()
    {
        var password = "Valid1!@";
        var validator = CreateValidator();

        var errors = validator.Validate(password);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenPasswordTooLong()
    {
        var password = new string('A', _rules.MaxLength!.Value) + "1#";
        var validator = CreateValidator();

        var errors = validator.Validate(password).ToList();

        Assert.Single(errors);
        Assert.Contains("не длиннее", errors[0]);
    }

    [Fact]
    public void Validate_ShouldPass_WhenPasswordIsAtMinLength()
    {
        var password = "A1!bcdef";
        var validator = CreateValidator();

        var errors = validator.Validate(password);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenPasswordMissingDigit()
    {
        var password = "ValidPass!";
        var validator = CreateValidator();

        var errors = validator.Validate(password).ToList();

        Assert.Contains(errors, e => e.Contains("цифру"));
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenPasswordMissingUppercase()
    {
        var password = "valid1!@";
        var validator = CreateValidator();

        var errors = validator.Validate(password).ToList();

        Assert.Contains(errors, e => e.Contains("заглавную"));
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenPasswordMissingSpecialCharacter()
    {
        var password = "Valid123";
        var validator = CreateValidator();

        var errors = validator.Validate(password).ToList();

        Assert.Contains(errors, e => e.Contains("специальный символ"));
    }

    [Fact]
    public void Validate_ShouldNotRequireDigit_WhenRuleIsDisabled()
    {
        var rules = _rules.Clone();
        rules.RequireDigit = false;
        var validator = CreateValidator(rules);

        var password = "ValidPass!";

        var errors = validator.Validate(password);

        Assert.DoesNotContain(errors, e => e.Contains("цифру"));
    }

    [Fact]
    public void Validate_ShouldNotRequireSpecial_WhenRuleIsDisabled()
    {
        var rules = _rules.Clone();
        rules.RequireSpecial = false;
        var validator = CreateValidator(rules);

        var password = "Valid123";

        var errors = validator.Validate(password);

        Assert.DoesNotContain(errors, e => e.Contains("специальный символ"));
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenRequireSpecialButNoAllowedCharacters()
    {
        var rules = _rules.Clone();
        rules.RequireSpecial = true;
        rules.SpecialCharacters = "";
        var validator = CreateValidator(rules);

        var password = "Valid123";

        var errors = validator.Validate(password).ToList();

        Assert.Contains(errors, e => e.Contains("специальный символ"));
    }
}
