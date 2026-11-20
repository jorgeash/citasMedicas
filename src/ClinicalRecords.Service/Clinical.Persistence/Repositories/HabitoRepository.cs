using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class HabitoRepository : Repository<Habito>, IHabitoRepository
{
    public HabitoRepository(ClinicalDbContext context) : base(context) { }
}
