using Addresses.Core.DTOs;
using Addresses.Core.Features.States;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Addresses.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<HttpResponse<PagedDto<List<StateDto>>>>> Get([FromQuery] GetStatesQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HttpResponse<StateDto>>> GetById(int id)
    {
        var response = await _mediator.Send(new GetStateByIdQuery(id));
        if (response is null)
            return NotFound();
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<HttpResponse<StateDto>>> Post([FromBody] CreateStateCommand command)
    {
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = response.Data.Id }, response);
    }
}