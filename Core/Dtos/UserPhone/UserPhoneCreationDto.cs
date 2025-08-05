using Core.Enums;

namespace Core.Dtos.UserPhone;

public record UserPhoneCreationDto(Guid UserId, string Phone, bool Confirmed, bool IsPrimary, PhoneType Type);