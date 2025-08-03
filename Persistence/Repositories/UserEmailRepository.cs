using Core.Entities;
using Core.Extensions;
using Core.Interfaces.Repositories;
using Core.Models;
using Exceptions.Exceptions.Emails;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Extensions;

namespace Persistence.Repositories;

public class UserEmailRepository(UserDbContext context) : IUserEmailRepository
{
    public async Task<IEnumerable<UserEmail>> GetUserEmailsAsync(Guid userId, int? limit = null, int? offset = null,
        bool track = true, CancellationToken cancellationToken = default)
    {
        var query = context.UserEmails
            .ConfigureTracking(track)
            .Where(e => e.UserId == userId);

        if (offset != null)
            query = query.Skip(offset.Value);

        if (limit != null)
            query = query.Take(limit.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<UserEmail?> GetUserEmailAsync(Guid id, bool track = true,
        CancellationToken cancellationToken = default)
    {
        return await context.UserEmails.ConfigureTracking(track)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<UserEmail?> GetUserEmailAsync(string email, bool track = true,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToNormalizedEmail();
        return await context.UserEmails.ConfigureTracking(track)
            .FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<UserEmail?> GetUserPrimaryEmailAsync(Guid userId, bool track = true, CancellationToken cancellationToken = default)
    => await context.UserEmails.ConfigureTracking(track)
        .FirstOrDefaultAsync(x => x.UserId == userId && x.IsPrimary == true, cancellationToken);

    public async Task<UserEmail> CreateUserEmailAsync(UserEmail userEmail, CancellationToken cancellationToken = default)
    {
        await context.UserEmails.AddAsync(userEmail, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return userEmail;
    }

    public async Task<UserEmail> UpdateUserEmailAsync(UserEmail userEmail, CancellationToken cancellationToken = default)
    {
        var id = userEmail.Id;
        var toUpdate = await GetUserEmailAsync(id, true, cancellationToken)
                        ?? throw new UserEmailNotFoundException(id);
        UpdateUserEmailFields(toUpdate, userEmail);
        await context.SaveChangesAsync(cancellationToken);
        return toUpdate;
    }

    private void UpdateUserEmailFields(UserEmail existingEmail, UserEmail newEmail)
    {
        existingEmail.Email = newEmail.Email;
        existingEmail.NormalizedEmail = newEmail.Email.ToNormalizedEmail();
        existingEmail.UpdatedAt = DateTime.UtcNow;
        existingEmail.UserId = newEmail.UserId;
        existingEmail.Confirmed = newEmail.Confirmed;
        existingEmail.EmailType = newEmail.EmailType;
        existingEmail.IsPrimary = newEmail.IsPrimary;
    }

    public async Task DeleteUserEmailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userEmail = await GetUserEmailAsync(id, true, cancellationToken)
                        ?? throw new UserEmailNotFoundException(id);
        context.UserEmails.Remove(userEmail);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToNormalizedEmail();
        return await context.UserEmails.AnyAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<int> GetUserEmailCountAsync(Guid userId, CancellationToken cancellationToken = default) 
        => await context.UserEmails.CountAsync(x => x.UserId == userId, cancellationToken);

    public Task<bool> UserHasPrimaryEmailAsync(Guid userId, CancellationToken cancellationToken = default) 
        => context.UserEmails.AnyAsync(x => x.UserId == userId && x.IsPrimary == true, cancellationToken);

    public async Task<UserEmailSummary?> GetUserEmailSummaryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var summary = await context.UserEmails
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.UserId)
            .Select(group => new UserEmailSummary
            {
                UserId = group.Key,
                EmailCount = group.Count(),
                PrimaryEmail = group.FirstOrDefault(e => e.IsPrimary)
            })
            .FirstOrDefaultAsync(cancellationToken);
        return summary;
    }
}