using Business.Core.DTOs;
using Business.Core.DTOs.Customers;
using Business.Core.Features.Customers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Business.Api.Controllers;

/// <summary>
/// API de clientes. Los clientes se crean con POST /api/customers indicando LocationId, que es
/// el Id de la direccion en el microservicio de Addresses (validado por HTTP con IRest).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<HttpResponse<PagedDto<List<CustomerDto>>>>> Get([FromQuery] GetCustomersQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HttpResponse<CustomerDto>>> GetById(int id)
    {
        var response = await _mediator.Send(new GetCustomerByIdQuery(id));
        if (response is null)
            return NotFound();
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<HttpResponse<CustomerDto>>> Post([FromBody] CreateCustomerCommand command)
    {
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = response.Data.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<HttpResponse<string>>> Put(int id, [FromBody] UpdateCustomerCommand command)
    {
        command.Id = id;
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<HttpResponse<string>>> Delete(int id)
    {
        var response = await _mediator.Send(new DeleteCustomerCommand { Id = id });
        return Ok(response);
    }
}