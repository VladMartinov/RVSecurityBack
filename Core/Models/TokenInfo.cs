using Core.Enums;

namespace Core.Models;

public record TokenInfo(TokenType TokenType, HashSet<string> Permissions, DateTime IssuedAt, DateTime? ExpiresAt);