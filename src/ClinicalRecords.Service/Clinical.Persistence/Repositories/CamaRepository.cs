using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class CamaRepository : Repository<Cama>, ICamaRepository
{
    public CamaRepository(ClinicalDbContext context) : base(context) { }
}
