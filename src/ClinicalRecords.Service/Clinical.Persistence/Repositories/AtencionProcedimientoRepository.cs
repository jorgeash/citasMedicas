using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class AtencionProcedimientoRepository : Repository<AtencionProcedimiento>, IAtencionProcedimientoRepository
{
    public AtencionProcedimientoRepository(ClinicalDbContext context) : base(context) { }
}
