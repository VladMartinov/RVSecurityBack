using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Testcontainers.PostgreSql;

namespace Tests.Integration.TestContainers.Pg;

public class PostgresContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .Build();

    public string ConnectionString => _postgresqlContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _postgresqlContainer.StartAsync();
        var connectionOptions = new DbContextOptionsBuilder<UserDbContext>().UseNpgsql(ConnectionString).Options;
        await using var context = new UserDbContext(connectionOptions);
        await context.Database.EnsureCreatedAsync();
        Console.WriteLine("✅ PostgreSQL container started, database ensured created.");
    }

    public async Task DisposeAsync()
    {
        await _postgresqlContainer.DisposeAsync().AsTask();
    }
}