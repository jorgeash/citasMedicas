using Notifications.Core.DTOs.External;

namespace Notifications.Core.Interfaces.Services;

/// <summary>
/// Consulta al microservicio de Billing los datos de la factura pagada. La interfaz habla
/// en términos de negocio: el handler no sabe que por detrás hay una llamada HTTP.
/// </summary>
public interface IFacturaService
{
    Task<FacturaPagadaResponse> ConsultarFacturaPagadaAsync(int facturaId, CancellationToken cancellationToken = default);
}
