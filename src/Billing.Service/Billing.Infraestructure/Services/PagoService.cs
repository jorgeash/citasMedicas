using Billing.Core.Configs;
using Billing.Core.DTOs;
using Billing.Core.Interfaces.Services;
using Microsoft.Extensions.Options;
using NugetPackage_Rest.Interfaces.IServices;

namespace Billing.Infraestructure.Services;

/// <summary>
/// Llama al microservicio de Pagos para registrar el cobro de una factura.
/// Inyecta IRest (nunca HttpClient ni RestBuilder) y las opciones por constructor.
/// </summary>
internal class PagoService : IPagoService
{
    private readonly IRest _rest;
    private readonly DownstreamOptions _options;

    public PagoService(IRest rest, IOptions<DownstreamOptions> options)
    {
        _rest = rest;
        _options = options.Value;
    }

    public async Task<string> ProcesarPagoAsync(int facturaId, decimal monto, string moneda, CancellationToken cancellationToken = default)
    {
        var response = await _rest.Post
            .WithoutAuth()
            .WithUri(_options.PagosBaseUrl, "/api/Pagos/procesar")
            .WithBody(new { facturaId, monto, moneda })
            .DeserializeWithAsync<HttpResponse<string>>(cancellationToken);

        if (string.IsNullOrWhiteSpace(response?.Data))
            throw new InvalidOperationException(response is null ? "El servicio de Pagos no devolvió una respuesta." : "El servicio de Pagos no devolvió un identificador de transacción.");

        return response.Data;
    }
}