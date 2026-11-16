using MediatR;
using Microsoft.AspNetCore.Mvc;
using Scheduling.Core.Exceptions;
using Scheduling.Core.Features.Scheduling.Pacientes.Commands.CreatePaciente;
using Scheduling.Core.Features.Scheduling.Pacientes.Queries.GetPacienteById;
using Scheduling.Core.Features.Scheduling.Pacientes.Queries.GetPacientes;

namespace Scheduling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PacientesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetPacientesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _mediator.Send(new GetPacienteByIdQuery(id));
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreatePacienteCommand command)
    {
        try
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { pacienteID = id });
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
