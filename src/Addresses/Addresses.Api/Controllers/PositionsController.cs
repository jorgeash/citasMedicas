using Addresses.Core.DTOs;
using Addresses.Core.Features.Positions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Addresses.Api.Controllers;

/// <summary>
/// Catalogo de direcciones (tabla Addresses_Position). Es el microservicio que el de Customers
/// consulta con la libreria NugetPackage_Rest para resolver el LocationId de un cliente.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PositionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PositionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<HttpResponse<PagedDto<List<PositionDto>>>>> Get([FromQuery] GetPositionsQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HttpResponse<PositionDto>>> GetById(int id)
    {
        var response = await _mediator.Send(new GetPositionByIdQuery(id));
        if (response is null)
            return NotFound();
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<HttpResponse<PositionDto>>> Post([FromBody] CreatePositionCommand command)
    {
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = response.Data.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<HttpResponse<string>>> Put(int id, [FromBody] UpdatePositionCommand command)
    {
        command.Id = id;
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<HttpResponse<string>>> Delete(int id)
    {
        var response = await _mediator.Send(new DeletePositionCommand { Id = id });
        return Ok(response);
    }
}