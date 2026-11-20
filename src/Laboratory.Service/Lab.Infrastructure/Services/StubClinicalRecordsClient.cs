using Lab.Core.DTOs.External;
using Lab.Core.Services;

namespace Lab.Infrastructure.Services;

internal class StubClinicalRecordsClient : IClinicalRecordsClient
{
    public Task<AtencionExternaDto?> GetAtencionAsync(
        long atencionId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<AtencionExternaDto?>(new AtencionExternaDto
        {
            AtencionID = atencionId,
            PacienteID = atencionId,
            FechaAtencion = DateTime.UtcNow,
            Estado = "ATENDIDA"
        });
    }
}
