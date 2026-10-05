using Billing.Core.DTOs;
using Billing.Core.Features.Billing.Cobros.Commands.ProcesarCobro;
using Billing.Core.Features.Billing.Cobros.Queries.GetComprobante;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CobrosController : ControllerBase
{
    private readonly IMediator _mediator;

    public CobrosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Procesar([FromBody] ProcesarCobroCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result.Data);
    }

    [HttpGet("{facturaId:int}/comprobante")]
    public async Task<IActionResult> GetComprobante(int facturaId)
    {
        var pdf = await _mediator.Send(new GetComprobanteQuery(facturaId));
        return File(pdf, "application/pdf", $"comprobante-{facturaId}.pdf");
    }
}