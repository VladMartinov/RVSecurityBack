using Application.Extensions;
using Core.Dtos.UserPhone;
using Core.Entities;
using Core.Extensions;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Exceptions.Exceptions.Phones;
using Mapster;

namespace Application.Services;

public class UserPhoneService(IPhoneValidator phoneValidator, IUserPhoneRepository userPhoneRepository, 
    IUserRepository userRepository, UserPhoneOptions phoneOptions) : IUserPhoneService
{
    private async Task ValidateAsync(Guid userId, bool isPrimary, string? phone, CancellationToken cancellationToken = default)
    {
        await userRepository.EnsureUserExists(userId, cancellationToken);
        var summary = await userPhoneRepository.GetUserPhoneSummaryAsync(userId, cancellationToken);
        
        var phoneCount = summary?.PhoneCount ?? 0;

        if (phoneCount >= phoneOptions.MaxPhoneCount)
            throw new PhoneCountLimitReachedException(userId, phoneCount, phoneOptions.MaxPhoneCount);
        
        if (isPrimary && summary?.PrimaryPhone != null)
            throw new MoreThenOnePrimaryPhoneException(summary.PrimaryPhone.PhoneNumber);
        
        if (phone == null) return;
        if (!phoneValidator.IsValidPhone(phone))
            throw new PhoneNumberIsInvalidException(phone);
        if (await userPhoneRepository.IsPhoneTakenAsync(phone, cancellationToken))
            throw new PhoneTakenException(phone);
    }
    
    public async Task<UserPhone> CreateUserPhoneAsync(UserPhoneCreationDto dto, CancellationToken cancellationToken = default)
    {
        var userPhoneModel = dto.Adapt<UserPhone>();
        await ValidateAsync(userPhoneModel.UserId, userPhoneModel.IsPrimary, userPhoneModel.PhoneNumber, cancellationToken);
        return await userPhoneRepository.AddUserPhoneAsync(userPhoneModel, cancellationToken);
    }

    public async Task<UserPhone> UpdateUserPhoneAsync(UserPhoneUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await userPhoneRepository.GetUserPhoneAsync(dto.Id, true, cancellationToken)
                       ?? throw new UserPhoneNotFoundException(dto.Id);
        
        var newUserId = dto.UserId ?? existing.UserId;
        var newPhone = string.IsNullOrWhiteSpace(dto.Phone) ? existing.PhoneNumber : dto.Phone.Trim();
        var normalizedPhone = newPhone.ToNormalizedPhoneNumber();

        var isPrimaryChanged = dto.IsPrimary.HasValue && dto.IsPrimary.Value != existing.IsPrimary;
        var newIsPrimary = isPrimaryChanged && dto.IsPrimary!.Value;

        var phoneChanged = normalizedPhone != existing.NormalizedPhone;
        
        await ValidateAsync(newUserId, newIsPrimary, phoneChanged ? newPhone : null, cancellationToken);
        
        existing.UserId = newUserId;
        existing.PhoneNumber = newPhone;
        existing.NormalizedPhone = normalizedPhone;
        existing.IsPrimary = newIsPrimary;

        if (dto.Confirmed.HasValue && dto.Confirmed.Value && !existing.Confirmed)
        {
            existing.Confirmed = true;
            existing.ConfirmedAt = DateTime.UtcNow;
        }
        else if (dto.Confirmed.HasValue)
            existing.Confirmed = dto.Confirmed.Value;
        

        return await userPhoneRepository.UpdateUserPhoneAsync(existing, cancellationToken);
    }

    public async Task DeleteUserPhoneAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await userPhoneRepository.DeleteUserPhoneAsync(id, cancellationToken);
    }
}