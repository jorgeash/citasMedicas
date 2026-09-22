using Billing.Core.DTOs.Billing;
using Billing.Core.Features.Billing.Servicios.Commands.CreateServicio;
using Billing.Core.Features.Billing.Servicios.Commands.CreateServiciosBulk;
using Billing.Core.Features.Billing.Servicios.Queries.GetServicios;
using Billing.Core.Features.Billing.Servicios.Queries.GetServicioById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Api.Controllers;

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
    public async Task<IActionResult> GetAll([FromQuery] GetServiciosQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetServicioByIdQuery(id));
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateServicioCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> PostBulk([FromBody] CreateServiciosBulkCommand command)
    {
        var count = await _mediator.Send(command);
        return Ok(new { message = $"{count} servicios creados correctamente" });
    }
}
