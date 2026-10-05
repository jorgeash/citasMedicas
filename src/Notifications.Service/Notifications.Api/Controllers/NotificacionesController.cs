using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notifications.Core.Features.Notificaciones.Commands.EnviarConfirmacionPago;

namespace Notifications.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacionesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificacionesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Confirma el pago de una factura: consulta la factura en Billing y envía el correo de
    /// confirmación al paciente.
    /// </summary>
    [HttpPost("confirmacion-pago")]
    public async Task<IActionResult> EnviarConfirmacionPago([FromBody] EnviarConfirmacionPagoCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result.Data);
    }
}
