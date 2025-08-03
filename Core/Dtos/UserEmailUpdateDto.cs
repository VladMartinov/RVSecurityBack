using Core.Enums;

namespace Core.Dtos;

public record UserEmailUpdateDto(Guid Id, Guid? UserId = null, string? Email = null, bool? Confirmed = null, bool? IsPrimary = null, EmailType? Type = null);