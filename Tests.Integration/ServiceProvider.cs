using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence.Context;
using Persistence.Repositories;

namespace Tests.Integration;

public static class ServiceProvider
{
    public static IServiceProvider Build(string postgresConnectionString)
    {
        var services = new ServiceCollection();
        
        services.AddDbContext<UserDbContext>(options => 
            options.UseNpgsql(postgresConnectionString)
                .EnableSensitiveDataLogging().LogTo(Console.WriteLine, LogLevel.Information));
        services.AddScoped<IUserRepository, UserRepository>();
        return services.BuildServiceProvider();
    }
}