using Lab.Core.Services;
using Lab.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lab.Infrastructure.Extensions;

public static class Extension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var identityBaseUrl = configuration["Services:Identity:BaseUrl"];

        AddExternalClient<IClinicalRecordsClient, ClinicalRecordsClient, StubClinicalRecordsClient>(
            services, configuration, "ClinicalRecords", identityBaseUrl);

        AddExternalClient<IIdentityClient, IdentityClient, StubIdentityClient>(
            services, configuration, "Identity", identityBaseUrl);

        return services;
    }

    private static void AddExternalClient<TContract, TReal, TStub>(
        IServiceCollection services,
        IConfiguration configuration,
        string nombre,
        string? identityBaseUrl)
        where TContract : class
        where TReal : class, TContract
        where TStub : class, TContract
    {
        var baseUrl = configuration[$"Services:{nombre}:BaseUrl"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            services.AddScoped<TContract, TStub>();
            return;
        }

        var cuenta = configuration.GetSection($"Services:{nombre}:ServiceAccount")
            .Get<ServiceAccountSettings>();

        if (cuenta is null || string.IsNullOrWhiteSpace(cuenta.Username) || string.IsNullOrWhiteSpace(cuenta.Password))
        {
            throw new InvalidOperationException(
                $"Falta Services:{nombre}:ServiceAccount (Username/Password) en appsettings.json. " +
                $"Los microservicios protegidos exigen JWT y Laboratory necesita una cuenta de servicio.");
        }

        if (string.IsNullOrWhiteSpace(identityBaseUrl))
        {
            throw new InvalidOperationException(
                "Falta Services:Identity:BaseUrl en appsettings.json (lo usa Laboratory para obtener el token de servicio).");
        }

        services.AddHttpClient("lab-identity", client =>
        {
            client.BaseAddress = new Uri(identityBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        services.AddHttpClient<TContract, TReal>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        })
        .AddHttpMessageHandler(sp => new ServiceTokenHandler(
            cuenta,
            sp.GetRequiredService<IHttpClientFactory>().CreateClient("lab-identity"),
            nombre));
    }
}
