using Billing.Core.DTOs.Billing;
using Billing.Core.Features.Billing.Tarifas.Commands.CreateTarifa;
using Billing.Core.Features.Billing.Tarifas.Queries.GetTarifas;
using Billing.Core.Features.Billing.Tarifas.Queries.GetTarifaById;
using Billing.Core.Features.Billing.Tarifas.Queries.GetTarifasByServicio;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TarifasController : ControllerBase
{
    private readonly IMediator _mediator;

    public TarifasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IEnumerable<TarifaDto>> GetAll()
    {
        return await _mediator.Send(new GetTarifasQuery());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetTarifaByIdQuery(id));
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("servicio/{servicioId:int}")]
    public async Task<IActionResult> GetByServicioId(int servicioId)
    {
        var result = await _mediator.Send(new GetTarifasByServicioQuery(servicioId));
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateTarifaCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }
}
