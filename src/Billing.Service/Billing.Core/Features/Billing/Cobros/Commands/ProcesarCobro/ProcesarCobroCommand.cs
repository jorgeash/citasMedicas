using Billing.Core.DTOs;
using Billing.Core.DTOs.External;
using Billing.Core.Exceptions;
using Billing.Core.Interfaces.Services;
using MediatR;

namespace Billing.Core.Features.Billing.Cobros.Commands.ProcesarCobro;

/// <summary>
/// Cobra una factura: verifica disponibilidad en Citas, la registra en Pagos y, si la
/// pasarela externa está caída, intenta el cobro allí. El handler depende de las
/// interfaces de Core, nunca de IRest.
/// </summary>
public class ProcesarCobroCommand : IRequest<HttpResponse<string>>
{
    public int FacturaId { get; set; }
    public int ServicioId { get; set; }
    public decimal Monto { get; set; }
    public string Moneda { get; set; } = "USD";
    public DateTime FechaCita { get; set; } = DateTime.UtcNow;
}

public class ProcesarCobroCommandHandler : IRequestHandler<ProcesarCobroCommand, HttpResponse<string>>
{
    private readonly ICitaService _citaService;
    private readonly IPagoService _pagoService;
    private readonly IPasarelaGateway _pasarelaGateway;

    public ProcesarCobroCommandHandler(
        ICitaService citaService,
        IPagoService pagoService,
        IPasarelaGateway pasarelaGateway)
    {
        _citaService = citaService;
        _pagoService = pagoService;
        _pasarelaGateway = pasarelaGateway;
    }

    public async Task<HttpResponse<string>> Handle(ProcesarCobroCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var disponible = await _citaService.EstaDisponibleAsync(request.ServicioId, request.FechaCita, cancellationToken);
            if (!disponible)
                throw new DomainException(Errores.CITA_NO_DISPONIBLE,
                    $"El servicio {request.ServicioId} no tiene cupos disponibles para el {request.FechaCita:yyyy-MM-dd}.");

            var referencia = $"FAC-{request.FacturaId}";

            CobroResponse cobro;
            try
            {
                cobro = await _pasarelaGateway.CobrarAsync(new CobroRequest
                {
                    Referencia = referencia,
                    Monto = request.Monto,
                    Moneda = request.Moneda
                }, cancellationToken);
            }
            catch (DomainException)
            {
                // La pasarela externa no respondió: el cobro interno de Pagos es la vía de reserva.
                var transaccion = await _pagoService.ProcesarPagoAsync(request.FacturaId, request.Monto, request.Moneda, cancellationToken);
                return new HttpResponse<string>(transaccion);
            }

            if (!string.Equals(cobro.Estado, "APROBADO", StringComparison.OrdinalIgnoreCase))
                throw new DomainException(Errores.PAGO_FALLIDO,
                    $"La pasarela rechazó el cobro: {string.Join("; ", cobro.Mensajes ?? new List<string>())}");

            await _pagoService.ProcesarPagoAsync(request.FacturaId, request.Monto, request.Moneda, cancellationToken);

            return new HttpResponse<string>(cobro.TransaccionId ?? referencia);
        }
        catch (Exception ex) when (ex is not DomainException)
        {
            throw new DomainException(Errores.PAGO_FALLIDO, $"No fue posible procesar el cobro de la factura {request.FacturaId}: {ex.Message}");
        }
    }
}