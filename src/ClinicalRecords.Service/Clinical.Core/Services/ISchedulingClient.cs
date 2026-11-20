using Clinical.Core.DTOs.External;

namespace Clinical.Core.Services;

public interface ISchedulingClient
{
    Task<CitaExternaDto?> GetCitaAsync(long citaId, CancellationToken cancellationToken = default);
}
