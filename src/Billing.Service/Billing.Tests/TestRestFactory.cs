using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Configs;
using NugetPackage_Rest.Interfaces.IServices;

namespace Billing.Tests;

/// <summary>
/// Construye un IRest real (RestBuilder) sobre un HttpClient con handler falso: así se
/// prueba también la serialización real, sin simular la cadena fluida.
/// </summary>
public static class TestRestFactory
{
    public static IRest Create(HttpMessageHandler handler) =>
        new RestBuilder(new HttpClient(handler), Options.Create(new RequestSettings { EnableRequestLogs = false }));

    /// <summary>
    /// IServiceScopeFactory sobre un IRest falso, para probar los servicios Singleton que
    /// resuelven IRest por scope en lugar de inyectarlo.
    /// </summary>
    public static IServiceScopeFactory CreateScopeFactory(HttpMessageHandler handler)
    {
        var rest = Create(handler);
        var services = new ServiceCollection();
        services.AddScoped(_ => rest);
        return services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
    }

    public static IOptions<T> CreateOptions<T>(T value) where T : class, new() =>
        Options.Create(value);
}
