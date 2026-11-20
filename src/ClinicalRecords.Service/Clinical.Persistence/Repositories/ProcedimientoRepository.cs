using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class ProcedimientoRepository : Repository<Procedimiento>, IProcedimientoRepository
{
    public ProcedimientoRepository(ClinicalDbContext context) : base(context) { }
}
