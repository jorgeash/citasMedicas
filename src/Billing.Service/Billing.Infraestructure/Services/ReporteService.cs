using Billing.Core.Configs;
using Billing.Core.Interfaces.Services;
using Microsoft.Extensions.Options;
using NugetPackage_Rest.Interfaces.IServices;

namespace Billing.Infraestructure.Services;

/// <summary>
/// Descarga el comprobante en PDF (GET binario) y lo reenvía al microservicio de
/// Reportes como multipart/form-data.
/// </summary>
internal class ReporteService : IReporteService
{
    private readonly IRest _rest;
    private readonly DownstreamOptions _options;

    public ReporteService(IRest rest, IOptions<DownstreamOptions> options)
    {
        _rest = rest;
        _options = options.Value;
    }

    public async Task<byte[]> DescargarComprobanteAsync(int facturaId, CancellationToken cancellationToken = default)
    {
        return await _rest.Get
            .WithoutAuth()
            .WithUri(_options.ReportesBaseUrl, $"/api/Reportes/facturas/{facturaId}/comprobante")
            .GetContentAsByteArrayAsync(cancellationToken);
    }

    public async Task EnviarComprobanteAsync(int facturaId, byte[] comprobante, CancellationToken cancellationToken = default)
    {
        using var form = new MultipartFormDataContent();
        form.Add(new ByteArrayContent(comprobante), "comprobante", $"factura-{facturaId}.pdf");
        form.Add(new StringContent(facturaId.ToString()), "facturaId");

        await _rest.Post
            .WithoutAuth()
            .WithUri(_options.ReportesBaseUrl, $"/api/Reportes/facturas/{facturaId}/comprobante")
            .WithFormData(form)
            .GetContentAsStringAsync(cancellationToken);
    }
}