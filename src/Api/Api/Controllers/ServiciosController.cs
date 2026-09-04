using Microsoft.AspNetCore.Mvc;
using Core.Billing.Servicios.Queries.GetServicios;
using Core.Billing.Servicios.Queries.GetServicioById;
using Core.Billing.Servicios.Commands.CreateServicio;
using Core.Billing.Servicios.Commands.UpdateServicio;
using Core.Billing.Servicios.Commands.DeleteServicio;
using MediatR;

namespace Api.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiciosController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServiciosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var servicios = await _mediator.Send(
            new GetServiciosQuery(), cancellationToken);

        return Ok(servicios);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var servicio = await _mediator.Send(
            new GetServicioByIdQuery(id), cancellationToken);

        if (servicio is null)
        {
            return NotFound();
        }

        return Ok(servicio);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateServicioCommand command,
        CancellationToken cancellationToken)
    {
        var servicio = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = servicio.ServicioID },
            servicio);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateServicioCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.ServicioID)
        {
            return BadRequest("El ID de la URL no coincide con el ID del servicio.");
        }

        var servicio = await _mediator.Send(command, cancellationToken);

        if (servicio is null)
        {
            return NotFound();
        }

        return Ok(servicio);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var eliminado = await _mediator.Send(
            new DeleteServicioCommand(id), cancellationToken);

        if (!eliminado)
        {
            return NotFound();
        }

        return NoContent();
    }
}