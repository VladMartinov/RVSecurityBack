using Application.Services;
using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Unit;

public static class ServiceProvider
{
    public static IServiceProvider Build()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IEmailValidator, EmailValidator>();
        
        return services.BuildServiceProvider();
    }
}