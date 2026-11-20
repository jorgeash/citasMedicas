using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Notification.Core.Exceptions;
using Notification.Core.Features.Notification.Commands.CancelarNotificacion;
using Notification.Core.Features.Notification.Commands.CreateNotificacion;
using Notification.Core.Features.Notification.Commands.EnviarNotificacion;
using Notification.Core.Features.Notification.Commands.MarcarNotificacionLeida;
using Notification.Core.Features.Notification.Queries.GetNotificacionById;
using Notification.Core.Features.Notification.Queries.GetNotificaciones;

namespace Notification.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacionesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificacionesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetNotificacionesQuery? query)
    {
        var result = await _mediator.Send(query ?? new GetNotificacionesQuery());
        return Ok(result);
    }

    [HttpGet("usuario/{usuarioId:long}")]
    public async Task<IActionResult> GetByUsuario(long usuarioId, [FromQuery] GetNotificacionesQuery? query)
    {
        var parametros = query ?? new GetNotificacionesQuery();
        parametros.UsuarioID = usuarioId;

        var result = await _mediator.Send(parametros);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetNotificacionByIdQuery(id));
        if (result is null)
        {
            return NotFound(new
            {
                errorCode = (int)Errores.NOTIFICACION_NO_ENCONTRADA,
                errorMessage = $"No existe la notificación {id}."
            });
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateNotificacionCommand command)
    {
        try
        {
            var notificacion = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = notificacion.NotificacionID }, notificacion);
        }
        catch (DomainException ex)
        {
            return BadRequest(new
            {
                errorCode = (int)ex.Codigo,
                errorMessage = ex.Message
            });
        }
    }

    [HttpPost("{id:int}/enviar")]
    public async Task<IActionResult> Enviar(
        int id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] EnviarNotificacionBody? body = null)
    {
        var command = new EnviarNotificacionCommand
        {
            NotificacionID = id,
            Exito = body?.Exito ?? true,
            Error = body?.Error
        };

        try
        {
            var notificacion = await _mediator.Send(command);
            return Ok(notificacion);
        }
        catch (DomainException ex)
        {
            return BadRequest(new
            {
                errorCode = (int)ex.Codigo,
                errorMessage = ex.Message
            });
        }
    }

    [HttpPost("{id:int}/leer")]
    public async Task<IActionResult> MarcarLeida(int id)
    {
        try
        {
            var notificacion = await _mediator.Send(new MarcarNotificacionLeidaCommand { NotificacionID = id });
            return Ok(notificacion);
        }
        catch (DomainException ex)
        {
            return BadRequest(new
            {
                errorCode = (int)ex.Codigo,
                errorMessage = ex.Message
            });
        }
    }

    [HttpPost("{id:int}/cancelar")]
    public async Task<IActionResult> Cancelar(int id, [FromBody] CancelarNotificacionBody? body)
    {
        try
        {
            var notificacion = await _mediator.Send(new CancelarNotificacionCommand
            {
                NotificacionID = id,
                Motivo = body?.Motivo
            });

            return Ok(notificacion);
        }
        catch (DomainException ex)
        {
            return BadRequest(new
            {
                errorCode = (int)ex.Codigo,
                errorMessage = ex.Message
            });
        }
    }
}

public class EnviarNotificacionBody
{
    public bool? Exito { get; set; }
    public string? Error { get; set; }
}

public class CancelarNotificacionBody
{
    public string? Motivo { get; set; }
}
