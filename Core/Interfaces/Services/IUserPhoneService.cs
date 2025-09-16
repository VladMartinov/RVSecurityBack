using Core.Dtos.UserPhone;
using Core.Entities;

namespace Core.Interfaces.Services;

public interface IUserPhoneService
{
    Task<UserPhone> CreateUserPhoneAsync(UserPhoneCreationDto dto, CancellationToken cancellationToken = default);
    Task<UserPhone> UpdateUserPhoneAsync(UserPhoneUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteUserPhoneAsync(Guid id, CancellationToken cancellationToken = default);
}