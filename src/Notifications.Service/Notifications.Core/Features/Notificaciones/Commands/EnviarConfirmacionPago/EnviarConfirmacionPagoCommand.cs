using MediatR;
using Notifications.Core.DTOs;
using Notifications.Core.DTOs.External;
using Notifications.Core.Exceptions;
using Notifications.Core.Interfaces.Services;
using Notifications.Domain.Entities;

namespace Notifications.Core.Features.Notificaciones.Commands.EnviarConfirmacionPago;

/// <summary>
/// Confirma el pago de una factura: consulta la factura en Billing y, si está pagada,
/// envía el correo de confirmación al paciente. El handler depende de las interfaces de
/// Core, nunca de IRest.
/// </summary>
public class EnviarConfirmacionPagoCommand : IRequest<HttpResponse<NotificacionEnviada>>
{
    public int FacturaId { get; set; }
}

public class EnviarConfirmacionPagoCommandHandler : IRequestHandler<EnviarConfirmacionPagoCommand, HttpResponse<NotificacionEnviada>>
{
    private readonly IFacturaService _facturaService;
    private readonly IEmailSender _emailSender;

    public EnviarConfirmacionPagoCommandHandler(IFacturaService facturaService, IEmailSender emailSender)
    {
        _facturaService = facturaService;
        _emailSender = emailSender;
    }

    public async Task<HttpResponse<NotificacionEnviada>> Handle(EnviarConfirmacionPagoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var factura = await _facturaService.ConsultarFacturaPagadaAsync(request.FacturaId, cancellationToken);

            if (!string.Equals(factura.Estado, "PAGADA", StringComparison.OrdinalIgnoreCase))
                throw new DomainException(Errores.FACTURA_NO_PAGADA,
                    $"La factura {request.FacturaId} está en estado '{factura.Estado}' y aún no se puede confirmar.");

            if (string.IsNullOrWhiteSpace(factura.Email))
                throw new DomainException(Errores.CORREO_NO_ENVIADO,
                    $"La factura {request.FacturaId} no tiene correo de destino registrado.");

            var notificacion = await _emailSender.EnviarAsync(
                factura.FacturaId,
                factura.Email,
                $"Confirmación de pago de la factura {factura.FacturaId}",
                $"Hola {factura.Paciente}, recibimos el pago de {factura.Monto:0.00} {factura.Moneda} "
                    + $"por la factura {factura.FacturaId}. Gracias por su compra.",
                cancellationToken);

            return new HttpResponse<NotificacionEnviada>(notificacion);
        }
        catch (Exception ex) when (ex is not DomainException)
        {
            throw new DomainException(Errores.SERVICIO_NO_DISPONIBLE,
                $"No fue posible confirmar el pago de la factura {request.FacturaId}: {ex.Message}");
        }
    }
}
