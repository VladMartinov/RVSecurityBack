namespace Exceptions.Exceptions.Phones;

public class MoreThenOnePrimaryPhoneException(string existingPrimaryPhone) : BadRequestException("Допустим максимум один основной номер телефона.", 
    new { ExistingPrimaryPhone = existingPrimaryPhone})
{
    
}