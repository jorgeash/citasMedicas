using MediatR;
using Microsoft.AspNetCore.Mvc;
using Scheduling.Core.Features.Scheduling.EstadosCita.Queries.GetEstadosCita;

namespace Scheduling.Api.Controllers;

[ApiController]
[Route("api/estados-cita")]
public class EstadosCitaController : ControllerBase
{
    private readonly IMediator _mediator;

    public EstadosCitaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetEstadosCitaQuery());
        return Ok(result.Data);
    }
}
