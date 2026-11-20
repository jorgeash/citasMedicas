using Lab.Core.DTOs.External;

namespace Lab.Core.Services;

public interface IClinicalRecordsClient
{
    Task<AtencionExternaDto?> GetAtencionAsync(long atencionId, CancellationToken cancellationToken = default);
}
