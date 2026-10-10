using Microsoft.Extensions.Options;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Configs;
using NugetPackage_Rest.Interfaces.IServices;

namespace Business.Tests;

/// <summary>
/// Construye un IRest real (RestBuilder) sobre un HttpClient con handler falso: asi se
/// prueba tambien la serializacion real y la cadena fluida de la libreria, sin simularla.
/// </summary>
public static class TestRestFactory
{
    public static IRest Create(HttpMessageHandler handler) =>
        new RestBuilder(new HttpClient(handler), Options.Create(new RequestSettings { EnableRequestLogs = false }));

    public static IOptions<T> CreateOptions<T>(T value) where T : class, new() =>
        Options.Create(value);
}