using Core.Entities;
using Core.Models;

namespace Core.Interfaces.Repositories;

public interface IUserPhoneRepository
{
    Task<IEnumerable<UserPhone>> GetUserPhonesAsync(Guid userId, int? limit = null, int? offset = null,
        bool track = true, CancellationToken cancellationToken = default);

    Task<UserPhone?> GetUserPhoneAsync(Guid id, bool track = true,
        CancellationToken cancellationToken = default);

    Task<UserPhone?> GetUserPhoneAsync(string phone, bool track = true,
        CancellationToken cancellationToken = default);

    Task<UserPhone?> GetUserPrimaryPhoneAsync(Guid userId, bool track = true,
        CancellationToken cancellationToken = default);

    Task<UserPhone> AddUserPhoneAsync(UserPhone userPhone, CancellationToken cancellationToken = default);
    Task<UserPhone> UpdateUserPhoneAsync(UserPhone userPhone, CancellationToken cancellationToken = default);
    Task DeleteUserPhoneAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsPhoneTakenAsync(string phone, CancellationToken cancellationToken = default);
    Task<int> GetUserPhoneCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> UserHasPrimaryPhoneAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserPhoneSummary?> GetUserPhoneSummaryAsync(Guid userId, CancellationToken cancellationToken = default);
}