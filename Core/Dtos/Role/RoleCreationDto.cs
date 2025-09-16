namespace Core.Dtos.Role;

public record RoleCreationDto(string Name, string? Description, bool IsSystem);