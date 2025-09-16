using Core.Models;

namespace Core.Dtos.UserToken;

public record UserTokenCreationDto(Guid UserId, string TokenHash, TokenInfo TokenInfo, DeviceInfo DeviceInfo, RevokeInfo RevokeInfo);