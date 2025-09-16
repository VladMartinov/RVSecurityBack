namespace Exceptions.Exceptions.Phones;

public class PhoneTakenException(string phone) : BadRequestException("Данный номер телефона занят другим пользователем", new { Phone = phone })
{
    
}