using Core.Dtos;
using Core.Entities;
using Mapster;

namespace Application.Configs;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<UserTokenCreationDto, UserToken>.NewConfig()
            .Map(dest => dest.TokenHash, src => src.TokenHash)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.Type, src => Enum.GetName(src.TokenInfo.TokenType))
            .Map(dest => dest.ExpiresAt, src => src.TokenInfo.ExpiresAt)
            .Map(dest => dest.Permissions, src => src.TokenInfo.Permissions)
            .Map(dest => dest.IssuedAt, src => src.TokenInfo.IssuedAt)
            .Map(dest => dest.DeviceId, src => src.DeviceInfo.DeviceId)
            .Map(dest => dest.IpAddress, src => src.DeviceInfo.IpAddress)
            .Map(dest => dest.UserAgent, src => src.DeviceInfo.UserAgent)
            .Map(dest => dest.Revoked, src => src.RevokeInfo.IsRevoked)
            .Map(dest => dest.RevokeReason, src => src.RevokeInfo.RevokeReason);
        
        TypeAdapterConfig<UserTokenUpdateDto, UserToken>.NewConfig()
            .Map(dest => dest.DeviceId, src => src.DeviceInfo.DeviceId)
            .Map(dest => dest.UserAgent, src => src.DeviceInfo.UserAgent)
            .Map(dest => dest.IpAddress, src => src.DeviceInfo.IpAddress)
            .Map(dest => dest.Revoked, src => src.RevokeInfo.IsRevoked)
            .Map(dest => dest.RevokeReason, src => src.RevokeInfo.RevokeReason)
            .Map(dest => dest.ExpiresAt, src => src.ExpiresAt);
    }
}