using Core.Billing.Pacientes.Commands.CreatePaciente;
using Core.Billing.Pacientes.Queries.GetPacienteById;
using Domain.Billing.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PacientesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Paciente>> GetById(long id)
    {
        var paciente = await _mediator.Send(
            new GetPacienteByIdQuery(id));

        if (paciente is null)
            return NotFound();

        return Ok(paciente);
    }

    [HttpPost]
    public async Task<ActionResult<Paciente>> Create(Paciente paciente)
    {
        var creado = await _mediator.Send(
            new CreatePacienteCommand(paciente));

        return Ok(creado);
    }
}
