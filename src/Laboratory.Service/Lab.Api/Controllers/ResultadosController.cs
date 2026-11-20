using Lab.Core.Exceptions;
using Lab.Core.Features.Lab.ResultadosEstudio.Commands.RegistrarResultadoEstudio;
using Lab.Core.Features.Lab.ResultadosEstudio.Queries.GetResultados;
using Lab.Core.Features.Lab.ResultadosEstudio.Queries.GetResultadosBySolicitud;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResultadosController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResultadosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetResultadosQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("solicitud/{solicitudEstudioId:int}")]
    public async Task<IActionResult> GetBySolicitud(int solicitudEstudioId)
    {
        var result = await _mediator.Send(new GetResultadosBySolicitudQuery(solicitudEstudioId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] RegistrarResultadoEstudioCommand command)
    {
        try
        {
            var resultado = await _mediator.Send(command);
            return Ok(resultado);
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
