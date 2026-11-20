using Lab.Core.Interfaces.Repositories;
using Lab.Domain.Entities;
using Lab.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Lab.Persistence.Repositories;

public class SolicitudEstudioDetalleRepository : Repository<SolicitudEstudioDetalle>, ISolicitudEstudioDetalleRepository
{
    public SolicitudEstudioDetalleRepository(LabDbContext context) : base(context) { }
}
