namespace Core.Dtos.Role;

public record RoleUpdateDto(Guid Id, string? Name, string? Description, bool? IsSystem);