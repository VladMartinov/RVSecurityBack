namespace Exceptions.Exceptions.Phones;

public class PhoneCountLimitReachedException(Guid userId, int currentCount, int maxCount) : 
    BadRequestException("Достигнуто максимальное количество номеров телефона для одного пользователя.", 
        new { UserId = userId, CurrentCount = currentCount, MaxCount = maxCount })
{
    
}