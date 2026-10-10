using Business.Core.Configs;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Business.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // URL base del microservicio de direcciones, usada por el cliente IRest.
        services.Configure<ApiAddressesOptions>(configuration.GetSection("ApiAddresses"));

        return services;
    }
}