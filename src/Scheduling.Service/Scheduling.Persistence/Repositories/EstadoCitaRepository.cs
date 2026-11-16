using Scheduling.Core.Interfaces.Repositories;
using Scheduling.Domain.Entities;
using Scheduling.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Scheduling.Persistence.Repositories;

public class EstadoCitaRepository : Repository<EstadoCita>, IEstadoCitaRepository
{
    public EstadoCitaRepository(SchedulingDbContext context) : base(context) { }
}
