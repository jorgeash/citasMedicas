using Billing.Core.DTOs;
using Billing.Core.Exceptions;
using Billing.Core.Services;
using MediatR;

namespace Billing.Core.Features.Billing.Pagos.Commands.ProcesarPago;

public class ProcesarPagoCommand : IRequest<HttpResponse<string>>
{
    public Guid FacturaId { get; set; }
    public decimal Monto { get; set; }
    public string Moneda { get; set; } = "USD";
}

/// <summary>Depende de la interfaz IPagoService (Core), nunca de IRest ni de la
/// librería HTTP: toda falla técnica se traduce a un error de dominio con código propio.</summary>
public class ProcesarPagoCommandHandler : IRequestHandler<ProcesarPagoCommand, HttpResponse<string>>
{
    private readonly IPagoService _pagoService;

    public ProcesarPagoCommandHandler(IPagoService pagoService)
    {
        _pagoService = pagoService;
    }

    public async Task<HttpResponse<string>> Handle(ProcesarPagoCommand request, CancellationToken cancellationToken)
    {
        string transaccionId;
        try
        {
            transaccionId = await _pagoService.ProcesarPagoAsync(request.FacturaId, request.Monto, request.Moneda, cancellationToken);
        }
        catch (Exception ex) when (ex is not DomainException)
        {
            throw new DomainException(Errores.PAGO_FALLIDO, $"No fue posible procesar el pago: {ex.Message}");
        }

        return new HttpResponse<string>(transaccionId);
    }
}
