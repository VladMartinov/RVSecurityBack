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
    private async Task ValidateAsync(Guid userId, string email, bool isPrimary, CancellationToken cancellationToken = default)
    {
        if (!emailValidator.IsValidEmail(email))
            throw new EmailIsInvalidException(email);
        
        await userRepository.EnsureUserExists(userId, cancellationToken);
        var summary = await emailRepository.GetUserEmailSummaryAsync(userId, cancellationToken);
        int userEmailsCount = summary?.EmailCount ?? 0;
        int maxEmailCount = emailOptions.MaxEmailCount;
        if (userEmailsCount >= maxEmailCount)
            throw new EmailCountLimitReachedException(userId, userEmailsCount, maxEmailCount);
        
        if (isPrimary && summary?.PrimaryEmail != null)
            throw new MoreThenOnePrimaryEmailException(summary.PrimaryEmail.Email);
        
        if (await emailRepository.IsEmailTakenAsync(email, cancellationToken))
            throw new EmailTakenException(email);
        
    }
    
    public async Task<UserEmail> CreateUserEmailAsync(UserEmailCreationDto dto, CancellationToken cancellationToken = default)
    {
        Guid userId = dto.UserId;
        string email = dto.Email.Trim();
        string normalizedEmail = email.ToNormalizedEmail();
        bool isPrimary = dto.IsPrimary;
        
        await ValidateAsync(userId, email, isPrimary, cancellationToken);
        
        bool confirmed = dto.Confirmed;
        string emailType = dto.EmailType.ToString();
        var model = new UserEmail
        {
            Confirmed = confirmed,
            ConfirmedAt = confirmed ? DateTime.UtcNow : null,
            Email = email,
            IsPrimary = isPrimary,
            UserId = userId,
            NormalizedEmail = normalizedEmail,
            EmailType = emailType
        };
        return await emailRepository.CreateUserEmailAsync(model, cancellationToken);
    }
}