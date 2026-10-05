using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notifications.Core.Interfaces.Services;
using Notifications.Infraestructure.Services;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Extensions;
using NugetPackage_Rest.Interfaces.IServices;
using Polly;

namespace Notifications.Infraestructure;

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
        //    sin agotamiento de sockets) con la política de resiliencia estándar de Polly.
        services.AddHttpClient<IRest, RestBuilder>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("X-Origen", "Notificaciones");
        })
        .AddStandardResilienceHandler(options =>
        {
            options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
            options.Retry.MaxRetryAttempts = 2;
            options.Retry.Delay = TimeSpan.FromMilliseconds(500);
            options.Retry.BackoffType = DelayBackoffType.Exponential;
            options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            options.CircuitBreaker.FailureRatio = 0.5;
            options.CircuitBreaker.MinimumThroughput = 10;
            options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
            options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);
        });

        // 3. Registra los servicios que usan IRest
        services.AddScoped<IFacturaService, FacturaService>();

        // El envío es simulado, así que no necesita IRest ni estado por request.
        services.AddSingleton<IEmailSender, SimulatedEmailSender>();

        return services;
    }
}
