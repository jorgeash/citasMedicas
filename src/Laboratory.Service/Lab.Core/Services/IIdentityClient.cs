using Lab.Core.DTOs.External;

namespace Lab.Core.Services;

public interface IIdentityClient
{
    Task<MedicoExternoDto?> GetMedicoAsync(int medicoId, CancellationToken cancellationToken = default);
}
