using System.Text.RegularExpressions;

namespace Core.Extensions;

/// <summary>
/// Класс нормализации данных.
/// </summary>
public static partial class NormalizationExtensions
{
    [GeneratedRegex(@"\D")]
    private static partial Regex OnlyDigitsRegex();
    public static string? ToNormalized(this string? str) => str?.Trim().ToUpperInvariant();
    public static string? ToNormalizedEmail(this string? email) => email?.Trim().ToLowerInvariant();
    public static string? ToNormalizedPhoneNumber(this string? source)
    {
        if (string.IsNullOrWhiteSpace(source)) return null;
        return OnlyDigitsRegex().Replace(source.Trim(), "");
    }
}