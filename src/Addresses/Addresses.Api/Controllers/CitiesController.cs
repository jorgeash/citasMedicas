using Addresses.Core.DTOs;
using Addresses.Core.Features.Cities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Addresses.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<HttpResponse<PagedDto<List<CityDto>>>>> Get([FromQuery] GetCitiesQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HttpResponse<CityDto>>> GetById(int id)
    {
        var response = await _mediator.Send(new GetCityByIdQuery(id));
        if (response is null)
            return NotFound();
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<HttpResponse<CityDto>>> Post([FromBody] CreateCityCommand command)
    {
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = response.Data.Id }, response);
    }
}