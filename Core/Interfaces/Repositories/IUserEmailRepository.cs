using Core.Entities;
using Core.Models;

namespace Core.Interfaces.Repositories;

public interface IUserEmailRepository
{
    Task<IEnumerable<UserEmail>> GetUserEmailsAsync(Guid userId, int? limit = null, int? offset = null,
        bool track = true, CancellationToken cancellationToken = default);

    Task<UserEmail?> GetUserEmailAsync(Guid id, bool track = true,
        CancellationToken cancellationToken = default);

    Task<UserEmail?> GetUserEmailAsync(string email, bool track = true,
        CancellationToken cancellationToken = default);
    Task<UserEmail?> GetUserPrimaryEmailAsync(Guid userId, bool track = true, CancellationToken cancellationToken = default);

    Task<UserEmail> AddUserEmailAsync(UserEmail userEmail, CancellationToken cancellationToken = default);
    Task<UserEmail> UpdateUserEmailAsync(UserEmail userEmail, CancellationToken cancellationToken = default);
    Task DeleteUserEmailAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default);
    Task<int> GetUserEmailCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> UserHasPrimaryEmailAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserEmailSummary?> GetUserEmailSummaryAsync(Guid userId, CancellationToken cancellationToken = default);
}