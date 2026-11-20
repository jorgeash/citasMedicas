using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class MedicamentoRepository : Repository<Medicamento>, IMedicamentoRepository
{
    public MedicamentoRepository(ClinicalDbContext context) : base(context) { }
}
