using Billing.Core.DTOs.External;
using Billing.Core.Services;

namespace Billing.Infrastructure.Services;

/// <summary>Implementación provisional mientras Scheduling no esté disponible.
/// Se activa cuando no hay Services:Scheduling:BaseUrl configurado.</summary>
internal class StubSchedulingClient : ISchedulingClient
{
    public Task<PacienteExternoDto?> GetPacienteAsync(int pacienteId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<PacienteExternoDto?>(new PacienteExternoDto
        {
            PacienteID = pacienteId,
            Nombre = $"Paciente {pacienteId}"
        });
    }

    public Task<CitaExternaDto?> GetCitaAsync(int citaId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<CitaExternaDto?>(new CitaExternaDto
        {
            CitaID = citaId,
            Fecha = DateTime.UtcNow
        });
    }
}
