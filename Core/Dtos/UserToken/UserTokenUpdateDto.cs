using Core.Models;

namespace Core.Dtos.UserToken;

public record UserTokenUpdateDto(Guid Id, DeviceInfo DeviceInfo, RevokeInfo RevokeInfo, DateTime? ExpiresAt);