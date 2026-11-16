using MediatR;
using Microsoft.AspNetCore.Mvc;
using Scheduling.Core.Features.Scheduling.Medicos.Queries.GetMedicos;

namespace Scheduling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicosController : ControllerBase
{
    private readonly IMediator _mediator;

    public MedicosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetMedicosQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }
}
