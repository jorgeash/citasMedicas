using Scheduling.Core.Interfaces.Repositories;
using Scheduling.Domain.Entities;

namespace Scheduling.Tests.Fakes;

public class FakePacienteRepository : InMemoryRepository<Paciente>, IPacienteRepository
{
    public FakePacienteRepository()
        : base(p => p.PacienteID, (p, id) => p.PacienteID = id)
    {
    }
}

public class FakeCitaRepository : InMemoryRepository<Cita>, ICitaRepository
{
    public FakeCitaRepository()
        : base(c => c.CitaID, (c, id) => c.CitaID = id)
    {
    }
}

public class FakeAseguradoraRepository : InMemoryRepository<Aseguradora>, IAseguradoraRepository
{
    public FakeAseguradoraRepository()
        : base(a => a.AseguradoraID, (a, id) => a.AseguradoraID = (int)id)
    {
    }
}

public class FakeMedicoRepository : InMemoryRepository<Medico>, IMedicoRepository
{
    public FakeMedicoRepository()
        : base(m => m.MedicoID, (m, id) => m.MedicoID = (int)id)
    {
    }
}

public class FakeEstadoCitaRepository : InMemoryRepository<EstadoCita>, IEstadoCitaRepository
{
    public FakeEstadoCitaRepository()
        : base(e => e.EstadoCitaID, (e, id) => e.EstadoCitaID = (int)id)
    {
    }
}
