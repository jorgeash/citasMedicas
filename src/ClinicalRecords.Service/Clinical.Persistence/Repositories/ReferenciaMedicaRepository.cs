using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class ReferenciaMedicaRepository : Repository<ReferenciaMedica>, IReferenciaMedicaRepository
{
    public ReferenciaMedicaRepository(ClinicalDbContext context) : base(context) { }
}
