using Addresses.Core.DTOs;
using Addresses.Core.Features.Countries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Addresses.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CountriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<HttpResponse<PagedDto<List<CountryDto>>>>> Get([FromQuery] GetCountriesQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HttpResponse<CountryDto>>> GetById(int id)
    {
        var response = await _mediator.Send(new GetCountryByIdQuery(id));
        if (response is null)
            return NotFound();
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<HttpResponse<CountryDto>>> Post([FromBody] CreateCountryCommand command)
    {
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = response.Data.Id }, response);
    }
}