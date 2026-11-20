using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class AtencionSintomaRepository : Repository<AtencionSintoma>, IAtencionSintomaRepository
{
    public AtencionSintomaRepository(ClinicalDbContext context) : base(context) { }
}
