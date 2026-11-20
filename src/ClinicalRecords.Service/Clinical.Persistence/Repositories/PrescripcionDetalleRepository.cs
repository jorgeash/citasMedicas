using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class PrescripcionDetalleRepository : Repository<PrescripcionDetalle>, IPrescripcionDetalleRepository
{
    public PrescripcionDetalleRepository(ClinicalDbContext context) : base(context) { }
}
