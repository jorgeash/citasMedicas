using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class AtencionRepository : Repository<Atencion>, IAtencionRepository
{
    public AtencionRepository(ClinicalDbContext context) : base(context) { }
}
