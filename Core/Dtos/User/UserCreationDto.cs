namespace Core.Dtos.User;

public record UserCreationDto(string UserName, string PasswordHash, bool TwoFactorEnabled, DateTime? LockoutEnd);