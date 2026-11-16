using MediatR;
using Microsoft.AspNetCore.Mvc;
using Scheduling.Core.Exceptions;
using Scheduling.Core.Features.Scheduling.Aseguradoras.Commands.CreateAseguradora;
using Scheduling.Core.Features.Scheduling.Aseguradoras.Queries.GetAseguradoraById;
using Scheduling.Core.Features.Scheduling.Aseguradoras.Queries.GetAseguradoras;

namespace Scheduling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AseguradorasController : ControllerBase
{
    private readonly IMediator _mediator;

    public AseguradorasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAseguradorasQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetAseguradoraByIdQuery(id));
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateAseguradoraCommand command)
    {
        try
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { aseguradoraID = id });
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
