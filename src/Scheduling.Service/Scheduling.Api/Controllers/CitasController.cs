using MediatR;
using Microsoft.AspNetCore.Mvc;
using Scheduling.Core.Exceptions;
using Scheduling.Core.Features.Scheduling.Citas.Commands.CancelarCita;
using Scheduling.Core.Features.Scheduling.Citas.Commands.CreateCita;
using Scheduling.Core.Features.Scheduling.Citas.Queries.GetCitaById;
using Scheduling.Core.Features.Scheduling.Citas.Queries.GetCitas;

namespace Scheduling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitasController : ControllerBase
{
    private readonly IMediator _mediator;

    public CitasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetCitasQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _mediator.Send(new GetCitaByIdQuery(id));
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCitaCommand command)
    {
        try
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { citaID = id });
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

    [HttpPost("{id:long}/cancelar")]
    public async Task<IActionResult> Cancelar(long id)
    {
        try
        {
            var cita = await _mediator.Send(new CancelarCitaCommand { CitaID = id });
            return Ok(cita);
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
