using Application.Extensions;
using Core.Dtos;
using Core.Entities;
using Core.Extensions;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Exceptions.Exceptions.Emails;

namespace Application.Services;

public class UserEmailService(IUserEmailRepository emailRepository, IUserRepository userRepository, 
    UserEmailOptions emailOptions, IEmailValidator emailValidator) : IUserEmailService
{
    private async Task ValidateAsync(Guid userId, bool isPrimary, string? email = null, CancellationToken cancellationToken = default)
    {
        await userRepository.EnsureUserExists(userId, cancellationToken);

        var summary = await emailRepository.GetUserEmailSummaryAsync(userId, cancellationToken);
        var emailCount = summary?.EmailCount ?? 0;

        if (emailCount >= emailOptions.MaxEmailCount)
            throw new EmailCountLimitReachedException(userId, emailCount, emailOptions.MaxEmailCount);

        if (isPrimary && summary?.PrimaryEmail != null)
            throw new MoreThenOnePrimaryEmailException(summary.PrimaryEmail.Email);

        if (!string.IsNullOrWhiteSpace(email))
        {
            if (!emailValidator.IsValidEmail(email))
                throw new EmailIsInvalidException(email);

            if (await emailRepository.IsEmailTakenAsync(email, cancellationToken))
                throw new EmailTakenException(email);
        }
    }

    public async Task<UserEmail> CreateUserEmailAsync(UserEmailCreationDto dto, CancellationToken cancellationToken = default)
    {
        var userId = dto.UserId;
        var email = dto.Email.Trim();
        var normalizedEmail = email.ToNormalizedEmail();
        var isPrimary = dto.IsPrimary;
        var confirmed = dto.Confirmed;
        var emailType = dto.EmailType.ToString();

        await ValidateAsync(userId, isPrimary, email, cancellationToken);

        var model = new UserEmail
        {
            Confirmed = confirmed,
            ConfirmedAt = confirmed ? DateTime.UtcNow : null,
            Email = email,
            NormalizedEmail = normalizedEmail,
            IsPrimary = isPrimary,
            UserId = userId,
            EmailType = emailType
        };

        return await emailRepository.CreateUserEmailAsync(model, cancellationToken);
    }

    public async Task<UserEmail> UpdateUserEmailAsync(UserEmailUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await emailRepository.GetUserEmailAsync(dto.Id, false, cancellationToken)
                       ?? throw new UserEmailNotFoundException(dto.Id);

        var newUserId = dto.UserId ?? existing.UserId;
        var newEmail = string.IsNullOrWhiteSpace(dto.Email) ? existing.Email : dto.Email.Trim();
        var normalizedEmail = newEmail.ToNormalizedEmail();

        var isPrimaryChanged = dto.IsPrimary.HasValue && dto.IsPrimary.Value != existing.IsPrimary;
        var newIsPrimary = isPrimaryChanged ? dto.IsPrimary!.Value : existing.IsPrimary;

        var emailChanged = normalizedEmail != existing.NormalizedEmail;
        await ValidateAsync(newUserId, newIsPrimary, emailChanged ? newEmail : null, cancellationToken);

        existing.UserId = newUserId;
        existing.Email = newEmail;
        existing.NormalizedEmail = normalizedEmail;
        existing.IsPrimary = newIsPrimary;

        if (dto.Confirmed.HasValue && dto.Confirmed.Value && !existing.Confirmed)
        {
            existing.Confirmed = true;
            existing.ConfirmedAt = DateTime.UtcNow;
        }
        else if (dto.Confirmed.HasValue)
        {
            existing.Confirmed = dto.Confirmed.Value;
        }

        return await emailRepository.UpdateUserEmailAsync(existing, cancellationToken);
    }

    public async Task DeleteUserEmailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await emailRepository.DeleteUserEmailAsync(id, cancellationToken);
    }
}