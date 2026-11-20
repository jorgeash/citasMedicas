using Lab.Core.Exceptions;
using Lab.Core.Features.Lab.Estudios.Commands.CreateEstudio;
using Lab.Core.Features.Lab.Estudios.Queries.GetEstudioById;
using Lab.Core.Features.Lab.Estudios.Queries.GetEstudios;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstudiosController : ControllerBase
{
    private readonly IMediator _mediator;

    public EstudiosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetEstudiosQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetEstudioByIdQuery(id));
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateEstudioCommand command)
    {
        try
        {
            var estudio = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = estudio.EstudioID }, estudio);
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
