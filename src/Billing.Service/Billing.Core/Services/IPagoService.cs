namespace Billing.Core.Services;

/// <summary>Llama a la pasarela de pagos externa para cobrar una factura y devuelve
/// el identificador de la transacción.</summary>
public interface IPagoService
{
    Task<string> ProcesarPagoAsync(Guid facturaId, decimal monto, string moneda, CancellationToken cancellationToken = default);
}
