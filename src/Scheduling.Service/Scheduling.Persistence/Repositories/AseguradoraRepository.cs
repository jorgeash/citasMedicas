using Scheduling.Core.Interfaces.Repositories;
using Scheduling.Domain.Entities;
using Scheduling.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Scheduling.Persistence.Repositories;

public class AseguradoraRepository : Repository<Aseguradora>, IAseguradoraRepository
{
    public AseguradoraRepository(SchedulingDbContext context) : base(context) { }
}
