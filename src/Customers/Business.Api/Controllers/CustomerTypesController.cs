using Business.Core.DTOs;
using Business.Core.DTOs.Customers;
using Business.Core.Features.CustomerTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Business.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerTypesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerTypesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<HttpResponse<PagedDto<List<CustomerTypeDto>>>>> Get([FromQuery] GetCustomerTypesQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<HttpResponse<CustomerTypeDto>>> Post([FromBody] CreateCustomerTypeCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }
}