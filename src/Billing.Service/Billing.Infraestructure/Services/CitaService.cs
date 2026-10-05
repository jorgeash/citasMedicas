using Billing.Core.Configs;
using Billing.Core.DTOs;
using Billing.Core.Interfaces.Services;
using Microsoft.Extensions.Options;
using NugetPackage_Rest.Interfaces.IServices;

namespace Billing.Infraestructure.Services;

/// <summary>
/// Consulta al microservicio de Citas con parámetros de query.
/// </summary>
internal class CitaService : ICitaService
{
    private readonly IRest _rest;
    private readonly DownstreamOptions _options;

    public CitaService(IRest rest, IOptions<DownstreamOptions> options)
    {
        _rest = rest;
        _options = options.Value;
    }

    public async Task<bool> EstaDisponibleAsync(int servicioId, DateTime fecha, CancellationToken cancellationToken = default)
    {
        var endpoint = $"/api/Citas/disponibilidad?servicioId={servicioId}&fecha={Uri.EscapeDataString(fecha.ToString("yyyy-MM-dd"))}";

        var response = await _rest.Get
            .WithoutAuth()
            .WithUri(_options.CitasBaseUrl, endpoint)
            .DeserializeWithAsync<HttpResponse<int>>(cancellationToken);

        return response?.Data > 0;
    }
}