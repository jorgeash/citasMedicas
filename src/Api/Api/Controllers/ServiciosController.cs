using Domain.Billing.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiciosController : ControllerBase
{
    private readonly IServicioRepository _servicioRepository;

    public ServiciosController(IServicioRepository servicioRepository)
    {
        _servicioRepository = servicioRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var servicios = await _servicioRepository.GetAllAsync();
        return Ok(servicios);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var servicio = await _servicioRepository.GetByIdAsync(id);
        if (servicio is null)
            return NotFound();

        return Ok(servicio);
    }
}
