using Billing.Core.Interfaces.Services;
using Billing.Infraestructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Extensions;
using NugetPackage_Rest.Interfaces.IServices;
namespace Billing.Infraestructure;

public static class InfraestructureExtensions
{
    /// <summary>
    /// Todo el registro HTTP del microservicio vive aquí: la librería NugetPackage_Rest
    /// solo se conoce desde esta capa.
    /// </summary>
    public static IServiceCollection AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Enlaza "RestSettings" → IOptions<RequestSettings>
        services.AddRequestLogging(configuration);

        // 2. Registra IRest como typed client de IHttpClientFactory (pool de handlers,
        //    sin agotar sockets) y le agrega la política de resiliencia estándar de Polly:
        //    reintentos con backoff y jitter para fallas transitorias (5xx, 408, 429 y
        //    errores de red), timeout total y circuit breaker. NO reintenta un 4xx
        //    definitivo como un 404, porque la respuesta sería la misma.
        services.AddHttpClient<IRest, RestBuilder>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("X-Origen", "Billing");
        })
        .AddStandardResilienceHandler(options =>
        {
            // Valores por defecto de la guía, con un timeout total coherente con el
            // HttpClient: si se agota, Polly corta antes de que lo haga HttpClient.
            options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
            options.Retry.MaxRetryAttempts = 2;
            options.Retry.Delay = TimeSpan.FromMilliseconds(500);
            options.Retry.BackoffType = DelayBackoffType.Exponential;
            // El 429 y el 503 dependen de la cabecera Retry-After que devuelve el servidor;
            // Polly la respeta, pero el timeout por intento acota la espera.
            options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            options.CircuitBreaker.FailureRatio = 0.5;
            options.CircuitBreaker.MinimumThroughput = 10;
            options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
            options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);
        });

        // 3. Registra los servicios que usan IRest
        services.AddScoped<IPagoService, PagoService>();
        services.AddScoped<ICitaService, CitaService>();
        services.AddScoped<IReporteService, ReporteService>();

        // Singleton a propósito: guarda el token en memoria entre requests
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddScoped<IPasarelaGateway, PasarelaGateway>();

        return services;
    }
}