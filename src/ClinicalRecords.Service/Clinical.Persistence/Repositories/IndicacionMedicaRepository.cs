using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class IndicacionMedicaRepository : Repository<IndicacionMedica>, IIndicacionMedicaRepository
{
    public IndicacionMedicaRepository(ClinicalDbContext context) : base(context) { }
}
