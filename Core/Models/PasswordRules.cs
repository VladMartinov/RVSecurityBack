namespace Core.Models;

public class PasswordRules
{
    public int MinLength { get; set; } = 8;
    public int? MaxLength { get; set; } = null;
    public bool RequireUppercase { get; set; } = true;
    public bool RequireDigit { get; set; } = true;
    public bool RequireSpecial { get; set; } = false;
    public string SpecialCharacters { get; set; } = "!@#$%^&*()-_=+[]{}|;:,.<>?";
    
    public PasswordRules Clone() => new()
    {
        MinLength = this.MinLength,
        MaxLength = this.MaxLength,
        RequireDigit = this.RequireDigit,
        RequireUppercase = this.RequireUppercase,
        RequireSpecial = this.RequireSpecial,
        SpecialCharacters = this.SpecialCharacters
    };
}