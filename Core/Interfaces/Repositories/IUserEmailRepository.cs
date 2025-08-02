using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface IUserEmailRepository
{
    Task<IEnumerable<UserEmail>> GetUserEmailsAsync(Guid userId, int? limit = null, int? offset = null,
        bool track = true, CancellationToken cancellationToken = default);

    Task<UserEmail?> GetUserEmailAsync(Guid id, bool track = true,
        CancellationToken cancellationToken = default);

    Task<UserEmail?> GetUserEmailAsync(string email, bool track = true,
        CancellationToken cancellationToken = default);

    Task<UserEmail> CreateUserEmailAsync(UserEmail userEmail, CancellationToken cancellationToken = default);
    Task<UserEmail> UpdateUserEmailAsync(UserEmail userEmail, CancellationToken cancellationToken = default);
    Task DeleteUserEmailAsync(Guid id, CancellationToken cancellationToken = default);
}