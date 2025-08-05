using Core.Dtos.Role;
using Core.Dtos.User;
using Core.Dtos.UserPhone;
using Core.Dtos.UserRole;
using Core.Dtos.UserToken;
using Core.Entities;
using Core.Extensions;
using Mapster;

namespace Application.Configs;

public static class MapsterConfig
{
    public static void Configure()
    {
        ConfigureRoleMappings();
        ConfigureUserRoleMappings();
        ConfigureUserPhoneMappings();
        ConfigureUserTokenMappings();
    }

    private static void ConfigureRoleMappings()
    {
        TypeAdapterConfig<RoleCreationDto, Role>.NewConfig()
            .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Name, src => src.Name.Trim())
            .Map(dest => dest.NormalizedName, src => src.Name.ToNormalized())
            .Map(dest => dest.IsSystem, src => src.IsSystem);
    }

    private static void ConfigureUserRoleMappings()
    {
        TypeAdapterConfig<SetUserRoleDto, UserRole>.NewConfig()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.RoleId, src => src.RoleId)
            .Map(dest => dest.AssignedAt, src => DateTime.UtcNow);
    }

    private static void ConfigureUserPhoneMappings()
    {
        TypeAdapterConfig<UserPhoneCreationDto, UserPhone>.NewConfig()
            .Map(dest => dest.NormalizedPhone, src => src.Phone.ToNormalizedPhoneNumber())
            .Map(dest => dest.PhoneNumber, src => src.Phone)
            .Map(dest => dest.Confirmed, src => src.Confirmed)
            .Map(dest => dest.ConfirmedAt, src => src.Confirmed ? DateTime.UtcNow : (DateTime?)null)
            .Map(dest => dest.PhoneType, src => src.Type.ToString())
            .Map(dest => dest.IsPrimary, src => src.IsPrimary)
            .Map(dest => dest.UserId, src => src.UserId);
    }

    private static void ConfigureUserTokenMappings()
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
    }

    private static void ConfigureUserMappings()
    {
        TypeAdapterConfig<UserCreationDto, User>.NewConfig()
            .Map(dest => dest.UserName, src => src.UserName)
            .Map(dest => dest.NormalizedUserName, src => src.UserName.ToNormalized())
            .Map(dest => dest.TwoFactorEnabled, src => src.TwoFactorEnabled)
            .Map(dest => dest.LockoutEnd, src => src.LockoutEnd);
    }
}