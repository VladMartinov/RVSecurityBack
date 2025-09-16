namespace Exceptions.Exceptions.Phones;

public class PhoneNumberIsInvalidException(string? phone) : BadRequestException("Не действительный номер телефона", new { Phone = phone })
{
    
}