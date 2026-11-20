using Clinical.Core.DTOs.External;
using Clinical.Core.Services;

namespace Clinical.Tests.Fakes;

public class SchedulingStub : ISchedulingClient
{
    private readonly CitaExternaDto? _cita;

    public SchedulingStub(CitaExternaDto? cita = null)
    {
        _cita = cita;
    }

    public Task<CitaExternaDto?> GetCitaAsync(long citaId, CancellationToken cancellationToken = default)
        => Task.FromResult(_cita);
}

public class IdentityStub : IIdentityClient
{
    private readonly MedicoExternoDto? _medico;

    public IdentityStub(MedicoExternoDto? medico = null)
    {
        _medico = medico;
    }

    public Task<MedicoExternoDto?> GetMedicoAsync(int medicoId, CancellationToken cancellationToken = default)
        => Task.FromResult(_medico);
}
