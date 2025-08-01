using Application.Services;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
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
        services.AddScoped<IUserService, UserService>();
        return services.BuildServiceProvider();
    }
}