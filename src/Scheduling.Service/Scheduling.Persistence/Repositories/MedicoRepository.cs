using Scheduling.Core.Interfaces.Repositories;
using Scheduling.Domain.Entities;
using Scheduling.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Scheduling.Persistence.Repositories;

public class MedicoRepository : Repository<Medico>, IMedicoRepository
{
    public MedicoRepository(SchedulingDbContext context) : base(context) { }
}
