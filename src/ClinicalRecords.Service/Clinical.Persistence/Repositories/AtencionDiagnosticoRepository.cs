using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class AtencionDiagnosticoRepository : Repository<AtencionDiagnostico>, IAtencionDiagnosticoRepository
{
    public AtencionDiagnosticoRepository(ClinicalDbContext context) : base(context) { }
}
