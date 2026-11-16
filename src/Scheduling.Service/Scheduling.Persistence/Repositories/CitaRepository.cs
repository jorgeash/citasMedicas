using Scheduling.Core.Interfaces.Repositories;
using Scheduling.Domain.Entities;
using Scheduling.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Scheduling.Persistence.Repositories;

public class CitaRepository : Repository<Cita>, ICitaRepository
{
    public CitaRepository(SchedulingDbContext context) : base(context) { }
}
