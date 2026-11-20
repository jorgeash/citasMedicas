using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical.Catalogos.Queries.GetMedicamentos;
using Clinical.Core.Features.Clinical.Catalogos.Queries.GetSintomasCatalogo;
using Clinical.Core.Features.Clinical.Diagnosticos.Queries.GetDiagnosticos;
using Clinical.Core.Features.Clinical.Hospitalizacion.Queries.GetCamas;
using Clinical.Core.Features.Clinical.Hospitalizacion.Queries.GetHabitaciones;
using Clinical.Core.Features.Clinical.Procedimientos.Queries.GetProcedimientos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogosController : ControllerBase
{
    private readonly IMediator _mediator;

    public CatalogosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("diagnosticos")]
    public async Task<IActionResult> GetDiagnosticos([FromQuery] GetDiagnosticosQuery query)
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

    [HttpGet("sintomas")]
    public async Task<IActionResult> GetSintomas([FromQuery] GetSintomasCatalogoQuery query)
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

    [HttpGet("medicamentos")]
    public async Task<IActionResult> GetMedicamentos([FromQuery] GetMedicamentosQuery query)
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

    [HttpGet("procedimientos")]
    public async Task<IActionResult> GetProcedimientos([FromQuery] GetProcedimientosQuery query)
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

    [HttpGet("habitaciones")]
    public async Task<IActionResult> GetHabitaciones([FromQuery] GetHabitacionesQuery query)
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

    [HttpGet("camas")]
    public async Task<IActionResult> GetCamas([FromQuery] GetCamasQuery query)
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
}
