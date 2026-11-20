using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical.Atenciones.Commands.CreateAtencion;
using Clinical.Core.Features.Clinical.Atenciones.Queries.GetAtencionById;
using Clinical.Core.Features.Clinical.Atenciones.Queries.GetAtenciones;
using Clinical.Core.Features.Clinical.Diagnosticos.Commands.AsignarDiagnostico;
using Clinical.Core.Features.Clinical.Diagnosticos.Queries.GetDiagnosticosByAtencion;
using Clinical.Core.Features.Clinical.Indicaciones.Commands.CreateIndicacion;
using Clinical.Core.Features.Clinical.Indicaciones.Queries.GetIndicacionesByAtencion;
using Clinical.Core.Features.Clinical.NotasMedicas.Commands.AgregarNotaMedica;
using Clinical.Core.Features.Clinical.NotasMedicas.Queries.GetNotasByAtencion;
using Clinical.Core.Features.Clinical.Prescripciones.Commands.CreatePrescripcion;
using Clinical.Core.Features.Clinical.Prescripciones.Queries.GetPrescripcionesByAtencion;
using Clinical.Core.Features.Clinical.Procedimientos.Commands.RegistrarProcedimiento;
using Clinical.Core.Features.Clinical.Procedimientos.Queries.GetProcedimientosByAtencion;
using Clinical.Core.Features.Clinical.Referencias.Commands.CreateReferencia;
using Clinical.Core.Features.Clinical.Referencias.Queries.GetReferenciasByAtencion;
using Clinical.Core.Features.Clinical.SignosVitales.Commands.RegistrarSignosVitales;
using Clinical.Core.Features.Clinical.SignosVitales.Queries.GetSignosVitalesByAtencion;
using Clinical.Core.Features.Clinical.Sintomas.Commands.RegistrarSintoma;
using Clinical.Core.Features.Clinical.Sintomas.Queries.GetSintomasByAtencion;
using Clinical.Core.Features.Clinical.Tratamientos.Commands.CreateTratamiento;
using Clinical.Core.Features.Clinical.Tratamientos.Queries.GetTratamientosByAtencion;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AtencionesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AtencionesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAtencionesQuery query)
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

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var result = await _mediator.Send(new GetAtencionByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateAtencionCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.AtencionID }, result);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpPost("{atencionId:long}/signos-vitales")]
    public async Task<IActionResult> PostSignosVitales(long atencionId, [FromBody] RegistrarSignosVitalesCommand command)
    {
        try
        {
            command.AtencionID = atencionId;
            return Ok(await _mediator.Send(command));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpGet("{atencionId:long}/signos-vitales")]
    public async Task<IActionResult> GetSignosVitales(long atencionId)
    {
        try
        {
            return Ok(await _mediator.Send(new GetSignosVitalesByAtencionQuery(atencionId)));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpPost("{atencionId:long}/notas")]
    public async Task<IActionResult> PostNota(long atencionId, [FromBody] AgregarNotaMedicaCommand command)
    {
        try
        {
            command.AtencionID = atencionId;
            return Ok(await _mediator.Send(command));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpGet("{atencionId:long}/notas")]
    public async Task<IActionResult> GetNotas(long atencionId)
    {
        try
        {
            return Ok(await _mediator.Send(new GetNotasByAtencionQuery(atencionId)));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpPost("{atencionId:long}/diagnosticos")]
    public async Task<IActionResult> PostDiagnostico(long atencionId, [FromBody] AsignarDiagnosticoCommand command)
    {
        try
        {
            command.AtencionID = atencionId;
            return Ok(await _mediator.Send(command));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpGet("{atencionId:long}/diagnosticos")]
    public async Task<IActionResult> GetDiagnosticos(long atencionId)
    {
        try
        {
            return Ok(await _mediator.Send(new GetDiagnosticosByAtencionQuery(atencionId)));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpPost("{atencionId:long}/sintomas")]
    public async Task<IActionResult> PostSintoma(long atencionId, [FromBody] RegistrarSintomaCommand command)
    {
        try
        {
            command.AtencionID = atencionId;
            return Ok(await _mediator.Send(command));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpGet("{atencionId:long}/sintomas")]
    public async Task<IActionResult> GetSintomas(long atencionId)
    {
        try
        {
            return Ok(await _mediator.Send(new GetSintomasByAtencionQuery(atencionId)));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpPost("{atencionId:long}/prescripciones")]
    public async Task<IActionResult> PostPrescripcion(long atencionId, [FromBody] CreatePrescripcionCommand command)
    {
        try
        {
            command.AtencionID = atencionId;
            return Ok(await _mediator.Send(command));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpGet("{atencionId:long}/prescripciones")]
    public async Task<IActionResult> GetPrescripciones(long atencionId)
    {
        try
        {
            return Ok(await _mediator.Send(new GetPrescripcionesByAtencionQuery(atencionId)));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpPost("{atencionId:long}/tratamientos")]
    public async Task<IActionResult> PostTratamiento(long atencionId, [FromBody] CreateTratamientoCommand command)
    {
        try
        {
            command.AtencionID = atencionId;
            return Ok(await _mediator.Send(command));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpGet("{atencionId:long}/tratamientos")]
    public async Task<IActionResult> GetTratamientos(long atencionId)
    {
        try
        {
            return Ok(await _mediator.Send(new GetTratamientosByAtencionQuery(atencionId)));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpPost("{atencionId:long}/indicaciones")]
    public async Task<IActionResult> PostIndicacion(long atencionId, [FromBody] CreateIndicacionCommand command)
    {
        try
        {
            command.AtencionID = atencionId;
            return Ok(await _mediator.Send(command));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpGet("{atencionId:long}/indicaciones")]
    public async Task<IActionResult> GetIndicaciones(long atencionId)
    {
        try
        {
            return Ok(await _mediator.Send(new GetIndicacionesByAtencionQuery(atencionId)));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpPost("{atencionId:long}/referencias")]
    public async Task<IActionResult> PostReferencia(long atencionId, [FromBody] CreateReferenciaCommand command)
    {
        try
        {
            command.AtencionID = atencionId;
            return Ok(await _mediator.Send(command));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpGet("{atencionId:long}/referencias")]
    public async Task<IActionResult> GetReferencias(long atencionId)
    {
        try
        {
            return Ok(await _mediator.Send(new GetReferenciasByAtencionQuery(atencionId)));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpPost("{atencionId:long}/procedimientos")]
    public async Task<IActionResult> PostProcedimiento(long atencionId, [FromBody] RegistrarProcedimientoCommand command)
    {
        try
        {
            command.AtencionID = atencionId;
            return Ok(await _mediator.Send(command));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }

    [HttpGet("{atencionId:long}/procedimientos")]
    public async Task<IActionResult> GetProcedimientos(long atencionId)
    {
        try
        {
            return Ok(await _mediator.Send(new GetProcedimientosByAtencionQuery(atencionId)));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { errorCode = (int)ex.Codigo, errorMessage = ex.Message });
        }
    }
}
