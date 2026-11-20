using Clinical.Core.DTOs.External;

namespace Clinical.Core.Services;

public interface IIdentityClient
{
    Task<MedicoExternoDto?> GetMedicoAsync(int medicoId, CancellationToken cancellationToken = default);
}
