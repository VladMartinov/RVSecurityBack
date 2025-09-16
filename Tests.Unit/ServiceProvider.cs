using Application.Services;
using Application.Validators;
using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Unit;

public static class ServiceProvider
{
    public static IServiceProvider Build()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IEmailValidator, EmailValidator>();
        services.AddSingleton<IPhoneValidator, PhoneValidator>();
        
        return services.BuildServiceProvider();
    }
}