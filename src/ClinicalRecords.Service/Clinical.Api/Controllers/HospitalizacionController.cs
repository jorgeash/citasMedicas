using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical.Hospitalizacion.Commands.EgresarPaciente;
using Clinical.Core.Features.Clinical.Hospitalizacion.Commands.IngresarPaciente;
using Clinical.Core.Features.Clinical.Hospitalizacion.Queries.GetHospitalizaciones;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Clinical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HospitalizacionController : ControllerBase
{
    private readonly IMediator _mediator;

    public HospitalizacionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetHospitalizacionesQuery query)
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

    [HttpPost("ingresar")]
    public async Task<IActionResult> Ingresar([FromBody] IngresarPacienteCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpPost("{id:long}/egresar")]
    public async Task<IActionResult> Egresar(
        long id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] EgresarPacienteRequest? request)
    {
        try
        {
            var result = await _mediator.Send(new EgresarPacienteCommand
            {
                HospitalizacionID = id,
                FechaEgreso = request?.FechaEgreso ?? default,
                Observaciones = request?.Observaciones
            });

            return Ok(result);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }
}

public class EgresarPacienteRequest
{
    public DateTime FechaEgreso { get; set; }
    public string? Observaciones { get; set; }
}
