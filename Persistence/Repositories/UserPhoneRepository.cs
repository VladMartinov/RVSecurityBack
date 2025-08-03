using Core.Entities;
using Core.Extensions;
using Core.Interfaces.Repositories;
using Core.Models;
using Exceptions.Exceptions.Phones;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Extensions;

namespace Persistence.Repositories;

public class UserPhoneRepository(UserDbContext context) : IUserPhoneRepository
{
    public async Task<IEnumerable<UserPhone>> GetUserPhonesAsync(Guid userId, int? limit = null, int? offset = null,
        bool track = true, CancellationToken cancellationToken = default)
    {
        var query = context.UserPhones
            .ConfigureTracking(track)
            .OrderBy(x => x.Id)
            .Where(e => e.UserId == userId);

        if (offset != null)
            query = query.Skip(offset.Value);

        if (limit != null)
            query = query.Take(limit.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<UserPhone?> GetUserPhoneAsync(Guid id, bool track = true,
        CancellationToken cancellationToken = default)
    {
        return await context.UserPhones.ConfigureTracking(track)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<UserPhone?> GetUserPhoneAsync(string phone, bool track = true,
        CancellationToken cancellationToken = default)
    {
        var normalizedPhone = phone.ToNormalizedPhoneNumber();
        return await context.UserPhones.ConfigureTracking(track)
            .FirstOrDefaultAsync(x => x.NormalizedPhone == normalizedPhone, cancellationToken);
    }

    public async Task<UserPhone?> GetUserPrimaryPhoneAsync(Guid userId, bool track = true, CancellationToken cancellationToken = default)
    => await context.UserPhones.ConfigureTracking(track)
        .FirstOrDefaultAsync(x => x.UserId == userId && x.IsPrimary == true, cancellationToken);

    public async Task<UserPhone> AddUserPhoneAsync(UserPhone userPhone, CancellationToken cancellationToken = default)
    {
        await context.UserPhones.AddAsync(userPhone, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return userPhone;
    }

    public async Task<UserPhone> UpdateUserPhoneAsync(UserPhone userPhone, CancellationToken cancellationToken = default)
    {
        var id = userPhone.Id;
        var toUpdate = await GetUserPhoneAsync(id, true, cancellationToken) ?? throw new UserPhoneNotFoundException(id);
        UpdateUserPhoneFields(toUpdate, userPhone);
        await context.SaveChangesAsync(cancellationToken);
        return toUpdate;
    }

    private void UpdateUserPhoneFields(UserPhone existingPhone, UserPhone newPhone)
    {
        existingPhone.PhoneNumber = newPhone.PhoneNumber;
        existingPhone.NormalizedPhone = newPhone.PhoneNumber.ToNormalizedPhoneNumber();
        existingPhone.UpdatedAt = DateTime.UtcNow;
        existingPhone.UserId = newPhone.UserId;
        existingPhone.Confirmed = newPhone.Confirmed;
        existingPhone.ConfirmedAt = newPhone.ConfirmedAt;
        existingPhone.PhoneType = newPhone.PhoneType;
        existingPhone.IsPrimary = newPhone.IsPrimary;
    }

    public async Task DeleteUserPhoneAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userPhone = await GetUserPhoneAsync(id, true, cancellationToken)
                        ?? throw new UserPhoneNotFoundException(id);
        context.UserPhones.Remove(userPhone);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsPhoneTakenAsync(string phone, CancellationToken cancellationToken = default)
    {
        var normalizedPhone = phone.ToNormalizedPhoneNumber();
        return await context.UserPhones.AsNoTracking()
            .AnyAsync(x => x.NormalizedPhone == normalizedPhone, cancellationToken);
    }

    public async Task<int> GetUserPhoneCountAsync(Guid userId, CancellationToken cancellationToken = default) 
        => await context.UserPhones.AsNoTracking()
            .CountAsync(x => x.UserId == userId, cancellationToken);

    public async Task<bool> UserHasPrimaryPhoneAsync(Guid userId, CancellationToken cancellationToken = default) 
        => await context.UserPhones.AsNoTracking()
            .AnyAsync(x => x.UserId == userId && x.IsPrimary == true, cancellationToken);

    public async Task<UserPhoneSummary?> GetUserPhoneSummaryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var summary = await context.UserPhones
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.UserId)
            .Select(group => new UserPhoneSummary
            {
                UserId = group.Key,
                PhoneCount = group.Count(),
                PrimaryPhone = group.FirstOrDefault(e => e.IsPrimary)
            })
            .FirstOrDefaultAsync(cancellationToken);
        return summary;
    }
}