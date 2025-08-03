namespace Exceptions.Exceptions.Emails;

public class MoreThenOnePrimaryEmailException(string existingPrimaryEmail) : 
    BadRequestException("Допустима максимум одна основная почта.", new { ExistingPrimaryEmail = existingPrimaryEmail})
{
    
}