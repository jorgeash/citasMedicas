using Clinical.Core.DTOs.External;
using Clinical.Core.Services;

namespace Clinical.Infrastructure.Services;

internal class StubSchedulingClient : ISchedulingClient
{
    public Task<CitaExternaDto?> GetCitaAsync(long citaId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<CitaExternaDto?>(new CitaExternaDto
        {
            CitaID = citaId,
            PacienteID = 1,
            Fecha = DateTime.UtcNow,
            EstadoCodigo = "PROGRAMADA"
        });
    }
}
