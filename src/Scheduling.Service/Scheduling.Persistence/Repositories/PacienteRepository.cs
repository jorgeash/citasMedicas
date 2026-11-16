using Scheduling.Core.Interfaces.Repositories;
using Scheduling.Domain.Entities;
using Scheduling.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Scheduling.Persistence.Repositories;

public class PacienteRepository : Repository<Paciente>, IPacienteRepository
{
    public PacienteRepository(SchedulingDbContext context) : base(context) { }
}
