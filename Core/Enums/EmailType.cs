namespace Core.Enums;

public enum EmailType
{
    /// <summary>
    /// Личная почта (например, Gmail, Yahoo и т.п.)
    /// </summary>
    Personal,
    /// <summary>
    /// Рабочая почта
    /// </summary>
    Work,
    /// <summary>
    /// Почта для получения счетов
    /// </summary>
    Billing,
    /// <summary>
    /// Уведомления системы
    /// </summary>
    Notifications,
    /// <summary>
    /// Всё, что не попадает в перечисленные категории
    /// </summary>
    Other       
}