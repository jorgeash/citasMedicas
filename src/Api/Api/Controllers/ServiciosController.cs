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
    public async Task<IActionResult> GetAll()
    {
        var servicios = await _mediator.Send(
            new GetServiciosQuery());

        return Ok(servicios);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var servicio = await _mediator.Send(
            new GetServicioByIdQuery(id));

        if (servicio is null)
        {
            return NotFound();
        }

        return Ok(servicio);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateServicioCommand command)
    {
        var servicio = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id = servicio.ServicioID },
            servicio);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateServicioCommand command)
    {
        if (id != command.ServicioID)
        {
            return BadRequest("El ID de la URL no coincide con el ID del servicio.");
        }

        var servicio = await _mediator.Send(command);

        if (servicio is null)
        {
            return NotFound();
        }

        return Ok(servicio);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var eliminado = await _mediator.Send(
            new DeleteServicioCommand(id));

        if (!eliminado)
        {
            return NotFound();
        }

        return NoContent();
    }
}