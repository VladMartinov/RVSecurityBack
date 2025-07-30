using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Tests.Integration.Extensions;

public static class DatabaseExtensions
{
    private static readonly (string Schema, string Table)[] Tables =
    [
        ("auth", "roles"),
        ("auth", "users"),
        ("auth", "user_emails"),
        ("auth", "user_phones"),
        ("auth", "user_roles"),
        ("auth", "user_tokens")
    ];

    public static async Task ClearDatabaseFull(this UserDbContext context)
    {
        var tablesList = string.Join(", ", Tables.Select(t => $@"""{t.Schema}"".""{t.Table}"""));
        var sql = $@"TRUNCATE TABLE {tablesList} RESTART IDENTITY CASCADE";
        await context.Database.ExecuteSqlRawAsync(sql);
    }
}