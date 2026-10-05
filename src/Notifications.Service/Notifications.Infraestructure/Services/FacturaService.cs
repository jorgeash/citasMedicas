using System.Net;
using Microsoft.Extensions.Options;
using Notifications.Core.Configs;
using Notifications.Core.DTOs;
using Notifications.Core.DTOs.External;
using Notifications.Core.Exceptions;
using Notifications.Core.Interfaces.Services;
using NugetPackage_Rest.Exceptions;
using NugetPackage_Rest.Interfaces.IServices;

namespace Notifications.Infraestructure.Services;

/// <summary>
/// Consulta al microservicio de Billing con IRest. Aquí es donde se inyecta la interfaz de
/// la librería: nunca RestBuilder ni HttpClient.
/// </summary>
internal class FacturaService : IFacturaService
{
    private readonly IRest _rest;
    private readonly DownstreamOptions _options;

    public FacturaService(IRest rest, IOptions<DownstreamOptions> options)
    {
        _rest = rest;
        _options = options.Value;
    }

    public async Task<FacturaPagadaResponse> ConsultarFacturaPagadaAsync(int facturaId, CancellationToken cancellationToken = default)
    {
        HttpResponse<FacturaPagadaResponse>? response;

        try
        {
            response = await _rest.Get
                .WithoutAuth()
                .WithUri(_options.BillingBaseUrl, $"/api/Cobros/{facturaId}/detalle")
                .DeserializeWithAsync<HttpResponse<FacturaPagadaResponse>>(cancellationToken);
        }
        catch (ApiException ex) when (ex.StatusCode == (int)HttpStatusCode.NotFound)
        {
            // Patrón 1 de la guía: traducir la falla técnica a un error de negocio con código propio.
            throw new DomainException(Errores.FACTURA_NO_ENCONTRADA,
                $"La factura {facturaId} no existe en el microservicio de Billing.");
        }

        if (response?.Data == null)
            throw new DomainException(Errores.FACTURA_NO_ENCONTRADA,
                $"La factura {facturaId} no devolvió información de la factura.");

        return response.Data;
    }
}