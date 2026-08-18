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
    public async Task<IActionResult> GetAll()
    {
        var tarifas = await _tarifaRepository.GetAllAsync();
        return Ok(tarifas);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var tarifa = await _tarifaRepository.GetByIdAsync(id);
        if (tarifa is null)
            return NotFound();

        return Ok(tarifa);
    }

    [HttpGet("servicio/{servicioId:int}")]
    public async Task<IActionResult> GetByServicioId(int servicioId)
    {
        var tarifas = await _tarifaRepository.GetByServicioIdAsync(servicioId);
        return Ok(tarifas);
    }
}
