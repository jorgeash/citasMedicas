using Billing.Core.Exceptions;
using Billing.Core.Features.Billing.Facturas.Commands.AnularFactura;
using Billing.Core.Features.Billing.Facturas.Commands.CreateFactura;
using Billing.Core.Features.Billing.Facturas.Queries.GetFacturaById;
using Billing.Core.Features.Billing.Facturas.Queries.GetFacturas;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Billing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FacturasController : ControllerBase
{
    private readonly IMediator _mediator;

    public FacturasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetFacturasQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetFacturaByIdQuery(id));
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateFacturaCommand command)
    {
        try
        {
            var factura = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = factura.FacturaID }, factura);
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

    [HttpPost("{id:int}/anular")]
    public async Task<IActionResult> Anular(
        int id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] AnularFacturaRequest? request)
    {
        try
        {
            var factura = await _mediator.Send(new AnularFacturaCommand
            {
                FacturaID = id,
                Motivo = request?.Motivo
            });

            return Ok(factura);
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

public class AnularFacturaRequest
{
    public string? Motivo { get; set; }
}
