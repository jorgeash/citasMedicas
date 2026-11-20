using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical.Archivos.Commands.AdjuntarArchivoClinico;
using Clinical.Core.Features.Clinical.Archivos.Queries.GetArchivosByExpediente;
using Clinical.Core.Features.Clinical.Expedientes.Commands.CreateExpediente;
using Clinical.Core.Features.Clinical.Expedientes.Queries.GetExpedienteById;
using Clinical.Core.Features.Clinical.Expedientes.Queries.GetExpedienteByPaciente;
using Clinical.Core.Features.Clinical.Expedientes.Queries.GetExpedientes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpedientesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExpedientesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetExpedientesQuery query)
    {
        try
        {
            return Ok(await _mediator.Send(query));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _mediator.Send(new GetExpedienteByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpGet("paciente/{pacienteId:long}")]
    public async Task<IActionResult> GetByPaciente(long pacienteId)
    {
        try
        {
            var result = await _mediator.Send(new GetExpedienteByPacienteQuery(pacienteId));
            return result is null ? NotFound() : Ok(result);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateExpedienteCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.ExpedienteClinicoID }, result);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpGet("{id:int}/archivos")]
    public async Task<IActionResult> GetArchivos(int id)
    {
        try
        {
            return Ok(await _mediator.Send(new GetArchivosByExpedienteQuery(id)));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpPost("{id:int}/archivos")]
    public async Task<IActionResult> PostArchivo(int id, [FromBody] AdjuntarArchivoClinicoCommand command)
    {
        try
        {
            command.ExpedienteClinicoID = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }
}
