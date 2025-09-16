namespace Core.Models;

public class UserPhoneOptions
{
    /// <summary>Максимальное количество номеров телефона на одного пользователя.</summary>
    public int MaxPhoneCount { get; set; } = 255;
}