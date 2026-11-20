using Lab.Core.Exceptions;
using Lab.Core.Features.Lab.SolicitudesEstudio.Commands.CancelarSolicitudEstudio;
using Lab.Core.Features.Lab.SolicitudesEstudio.Commands.CreateSolicitudEstudio;
using Lab.Core.Features.Lab.SolicitudesEstudio.Queries.GetSolicitudEstudioById;
using Lab.Core.Features.Lab.SolicitudesEstudio.Queries.GetSolicitudesEstudio;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolicitudesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SolicitudesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetSolicitudesEstudioQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetSolicitudEstudioByIdQuery(id));
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateSolicitudEstudioCommand command)
    {
        try
        {
            var solicitud = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = solicitud.SolicitudEstudioID }, solicitud);
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
    public async Task<IActionResult> Cancelar(
        int id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] CancelarSolicitudRequest? request)
    {
        try
        {
            var solicitud = await _mediator.Send(new CancelarSolicitudEstudioCommand
            {
                SolicitudEstudioID = id,
                Motivo = request?.Motivo
            });

            return Ok(solicitud);
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

public class CancelarSolicitudRequest
{
    public string? Motivo { get; set; }
}
