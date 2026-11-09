using Billing.Core.DTOs;
using Billing.Core.Exceptions;
using Billing.Core.Features.Billing.Pagos.Commands.ProcesarPago;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PagosController : ControllerBase
{
    private readonly IMediator _mediator;

    public PagosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("procesar")]
    public async Task<IActionResult> Procesar([FromBody] ProcesarPagoCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
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
