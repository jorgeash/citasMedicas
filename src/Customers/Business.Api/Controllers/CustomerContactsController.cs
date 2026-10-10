using Business.Core.DTOs;
using Business.Core.DTOs.Customers;
using Business.Core.Features.CustomerContacts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Business.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerContactsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerContactsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<HttpResponse<PagedDto<List<CustomerContactDto>>>>> Get([FromQuery] GetCustomerContactsQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<HttpResponse<CustomerContactDto>>> Post([FromBody] CreateCustomerContactCommand command)
    {
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { customerId = command.CustomerId }, response);
    }
}