using Clinical.Core.Services;
using Clinical.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clinical.Infrastructure.Extensions;

public static class Extension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var schedulingBaseUrl = configuration["Services:Scheduling:BaseUrl"];
        var identityBaseUrl = configuration["Services:Identity:BaseUrl"];

        var hayClienteReal = !string.IsNullOrWhiteSpace(schedulingBaseUrl) || !string.IsNullOrWhiteSpace(identityBaseUrl);

        if (hayClienteReal)
        {
            var tokenBaseUrl = configuration["Identity:BaseUrl"];
            if (string.IsNullOrWhiteSpace(tokenBaseUrl))
                tokenBaseUrl = identityBaseUrl;

            if (string.IsNullOrWhiteSpace(tokenBaseUrl))
                throw new InvalidOperationException("Falta Identity:BaseUrl en appsettings.json (se usa para obtener el token de servicio).");

            services.AddHttpClient("identity", client =>
            {
                client.BaseAddress = new Uri(tokenBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            });
        }

        if (string.IsNullOrWhiteSpace(schedulingBaseUrl))
        {
            services.AddScoped<ISchedulingClient, StubSchedulingClient>();
        }
        else
        {
            var cuenta = ObtenerCuenta(configuration, "Scheduling");
            services.AddHttpClient<ISchedulingClient, SchedulingClient>(client =>
            {
                client.BaseAddress = new Uri(schedulingBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddHttpMessageHandler(sp => CrearHandler(sp, cuenta, "Scheduling"));
        }

        if (string.IsNullOrWhiteSpace(identityBaseUrl))
        {
            services.AddScoped<IIdentityClient, StubIdentityClient>();
        }
        else
        {
            var cuenta = ObtenerCuenta(configuration, "Identity");
            services.AddHttpClient<IIdentityClient, IdentityClient>(client =>
            {
                client.BaseAddress = new Uri(identityBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddHttpMessageHandler(sp => CrearHandler(sp, cuenta, "Identity"));
        }

        return services;
    }

    private static ServiceAccountSettings ObtenerCuenta(IConfiguration configuration, string nombre)
    {
        var cuenta = configuration.GetSection($"Services:{nombre}:ServiceAccount").Get<ServiceAccountSettings>();

        if (cuenta is null || string.IsNullOrWhiteSpace(cuenta.Username) || string.IsNullOrWhiteSpace(cuenta.Password))
            throw new InvalidOperationException(
                $"Falta Services:{nombre}:ServiceAccount (Username/Password) en appsettings.json. " +
                $"Los microservicios exigen JWT y Clinical necesita una cuenta de servicio para llamarlos.");

        return cuenta;
    }

    private static ServiceTokenHandler CrearHandler(
        IServiceProvider sp,
        ServiceAccountSettings cuenta,
        string destino)
    {
        var factory = sp.GetRequiredService<IHttpClientFactory>();
        return new ServiceTokenHandler(cuenta, factory.CreateClient("identity"), destino);
    }
}
