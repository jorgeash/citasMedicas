using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class PrescripcionRepository : Repository<Prescripcion>, IPrescripcionRepository
{
    public PrescripcionRepository(ClinicalDbContext context) : base(context) { }
}
