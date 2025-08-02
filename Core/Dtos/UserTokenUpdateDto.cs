using Core.Models;

namespace Core.Dtos;

public record UserTokenUpdateDto(Guid Id, DeviceInfo DeviceInfo, RevokeInfo RevokeInfo, DateTime? ExpiresAt);