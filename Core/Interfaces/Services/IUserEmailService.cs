using Core.Dtos;
using Core.Entities;

namespace Core.Interfaces.Services;

public interface IUserEmailService
{
    Task<UserEmail> CreateUserEmailAsync(UserEmailCreationDto dto, CancellationToken cancellationToken = default);
    Task<UserEmail> UpdateUserEmailAsync(UserEmailUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteUserEmailAsync(Guid id, CancellationToken cancellationToken = default);
}