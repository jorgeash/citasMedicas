using Microsoft.Extensions.Options;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Configs;
using NugetPackage_Rest.Interfaces.IServices;

namespace Notifications.Tests;

/// <summary>
/// Construye un IRest real (RestBuilder) sobre un HttpClient con handler falso: así se
/// prueba también la serialización real, sin simular la cadena fluida.
/// </summary>
public static class TestRestFactory
{
    public static IRest Create(HttpMessageHandler handler) =>
        new RestBuilder(new HttpClient(handler), Options.Create(new RequestSettings { EnableRequestLogs = false }));

    public static IOptions<T> CreateOptions<T>(T value) where T : class, new() =>
        Options.Create(value);
}
