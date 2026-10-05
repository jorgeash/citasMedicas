namespace Billing.Core.Interfaces.Services;

/// <summary>
/// Genera y almacena el comprobante de una factura en el microservicio de Reportes.
/// </summary>
public interface IReporteService
{
    Task<byte[]> DescargarComprobanteAsync(int facturaId, CancellationToken cancellationToken = default);

    Task EnviarComprobanteAsync(int facturaId, byte[] comprobante, CancellationToken cancellationToken = default);
}