namespace Billing.Core.Interfaces.Services;

/// <summary>
/// Llama al microservicio de Pagos para registrar el cobro de una factura y devuelve
/// el identificador de la transacción.
/// </summary>
public interface IPagoService
{
    Task<string> ProcesarPagoAsync(int facturaId, decimal monto, string moneda, CancellationToken cancellationToken = default);
}