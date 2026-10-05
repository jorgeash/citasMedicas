using Billing.Core.DTOs.External;

namespace Billing.Core.Interfaces.Services;

/// <summary>
/// Cobro contra la pasarela de pagos externa (API de un tercero con OAuth 2.0).
/// </summary>
public interface IPasarelaGateway
{
    Task<CobroResponse> CobrarAsync(CobroRequest request, CancellationToken cancellationToken = default);

    Task<CobroResponse> ConsultarCobroAsync(string referencia, CancellationToken cancellationToken = default);
}