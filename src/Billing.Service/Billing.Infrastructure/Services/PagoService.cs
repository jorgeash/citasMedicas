using Billing.Core.Configs;
using Billing.Core.Services;
using Billing.Core.DTOs;
using NugetPackage_Rest.Interfaces.IServices;
using Microsoft.Extensions.Options;

namespace Billing.Infrastructure.Services;

internal class PagoService : IPagoService
{
    private readonly IRest _rest;
    private readonly PasarelaOptions _options;

    public PagoService(IRest rest, IOptions<PasarelaOptions> options)
    {
        _rest = rest;
        _options = options.Value;
    }

    public async Task<string> ProcesarPagoAsync(Guid facturaId, decimal monto, string moneda, CancellationToken cancellationToken = default)
    {
        var response = await _rest.Post
            .WithoutAuth()
            .WithUri(_options.BaseUrl, "/api/Pagos/procesar")
            .WithBody(new { facturaId, monto, moneda })
            .DeserializeWithAsync<HttpResponse<string>>();

        if (string.IsNullOrWhiteSpace(response?.Data))
            throw new InvalidOperationException("La pasarela no devolvió un identificador de transacción.");

        return response.Data;
    }
}
