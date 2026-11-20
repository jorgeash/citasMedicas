using Lab.Core.DTOs.External;
using Lab.Core.Services;

namespace Lab.Infrastructure.Services;

internal class StubIdentityClient : IIdentityClient
{
    public Task<MedicoExternoDto?> GetMedicoAsync(
        int medicoId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<MedicoExternoDto?>(new MedicoExternoDto
        {
            MedicoID = medicoId,
            NombreCompleto = $"Médico {medicoId}",
            Activo = true
        });
    }
}
