using Lab.Core.DTOs.External;
using Lab.Core.Services;

namespace Lab.Tests.Fakes;

public class FakeClinicalRecordsClient : IClinicalRecordsClient
{
    private readonly Func<long, AtencionExternaDto?> _resolve;

    public FakeClinicalRecordsClient(Func<long, AtencionExternaDto?>? resolve = null)
    {
        _resolve = resolve ?? (id => new AtencionExternaDto
        {
            AtencionID = id,
            PacienteID = id,
            FechaAtencion = DateTime.UtcNow,
            Estado = "ATENDIDA"
        });
    }

    public Task<AtencionExternaDto?> GetAtencionAsync(
        long atencionId,
        CancellationToken cancellationToken = default)
        => Task.FromResult(_resolve(atencionId));
}

public class FakeIdentityClient : IIdentityClient
{
    private readonly Func<int, MedicoExternoDto?> _resolve;

    public FakeIdentityClient(Func<int, MedicoExternoDto?>? resolve = null)
    {
        _resolve = resolve ?? (id => new MedicoExternoDto
        {
            MedicoID = id,
            NombreCompleto = $"Médico {id}",
            Activo = true
        });
    }

    public Task<MedicoExternoDto?> GetMedicoAsync(
        int medicoId,
        CancellationToken cancellationToken = default)
        => Task.FromResult(_resolve(medicoId));
}
