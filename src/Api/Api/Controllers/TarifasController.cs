using Domain.Billing.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TarifasController : ControllerBase
{
    private readonly ITarifaRepository _tarifaRepository;

    public TarifasController(ITarifaRepository tarifaRepository)
    {
        _tarifaRepository = tarifaRepository;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var tarifas = await _tarifaRepository.GetAllAsync(cancellationToken);
        return Ok(tarifas);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var tarifa = await _tarifaRepository.GetByIdAsync(id, cancellationToken);
        if (tarifa is null)
            return NotFound();

        return Ok(tarifa);
    }

    [HttpGet("servicio/{servicioId:int}")]
    public async Task<IActionResult> GetByServicioId(int servicioId, CancellationToken cancellationToken)
    {
        var tarifas = await _tarifaRepository.GetByServicioIdAsync(servicioId, cancellationToken);
        return Ok(tarifas);
    }
}
