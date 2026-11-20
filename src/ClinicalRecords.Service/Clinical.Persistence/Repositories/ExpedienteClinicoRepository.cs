using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class ExpedienteClinicoRepository : Repository<ExpedienteClinico>, IExpedienteClinicoRepository
{
    public ExpedienteClinicoRepository(ClinicalDbContext context) : base(context) { }
}
