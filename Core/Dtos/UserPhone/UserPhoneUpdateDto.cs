using Core.Enums;

namespace Core.Dtos.UserPhone;

public record UserPhoneUpdateDto(Guid Id, Guid? UserId, string? Phone, bool? Confirmed, bool? IsPrimary, PhoneType? PhoneType);