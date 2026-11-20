using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class DiagnosticoRepository : Repository<Diagnostico>, IDiagnosticoRepository
{
    public DiagnosticoRepository(ClinicalDbContext context) : base(context) { }
}
