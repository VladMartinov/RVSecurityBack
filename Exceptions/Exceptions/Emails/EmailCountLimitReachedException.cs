namespace Exceptions.Exceptions.Emails;

public class EmailCountLimitReachedException(Guid userId, int currentCount, int maxCount) : 
    BadRequestException("Достигнуто максимальное количество почт для одного пользователя.", 
        new { UserId = userId, CurrentCount = currentCount, MaxCount = maxCount })
{
    
}