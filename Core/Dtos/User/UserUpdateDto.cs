namespace Core.Dtos.User;

public record UserUpdateDto(Guid Id, string? UserName, string? PasswordHash);