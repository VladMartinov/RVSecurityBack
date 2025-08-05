using Core.Enums;

namespace Core.Dtos.UserEmail;

public record UserEmailCreationDto(Guid UserId, string Email, bool Confirmed, EmailType EmailType, bool IsPrimary);