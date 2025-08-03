using Application.Services;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
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
        var emailOptions = new UserEmailOptions();
        emailOptions.MaxEmailCount = 4;
        
        services.AddDbContext<UserDbContext>(options => 
            options.UseNpgsql(postgresConnectionString)
                .EnableSensitiveDataLogging().LogTo(Console.WriteLine, LogLevel.Information));
        services.AddSingleton(emailOptions);
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserEmailRepository, UserEmailRepository>();
        services.AddScoped<IUserTokenRepository, UserTokenRepository>();
        services.AddScoped<IEmailValidator, EmailValidator>();
        services.AddScoped<IUserEmailService, UserEmailService>();
        services.AddScoped<IUserService, UserService>();
        return services.BuildServiceProvider();
    }
}