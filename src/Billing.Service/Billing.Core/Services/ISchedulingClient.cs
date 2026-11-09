using Billing.Core.DTOs.External;

namespace Billing.Core.Services;

/// <summary>Cliente del microservicio Scheduling. La implementación real vive
/// en Infrastructure; Core solo conoce el contrato.</summary>
public interface ISchedulingClient
{
    Task<PacienteExternoDto?> GetPacienteAsync(int pacienteId, CancellationToken cancellationToken = default);

    Task<CitaExternaDto?> GetCitaAsync(int citaId, CancellationToken cancellationToken = default);
}
