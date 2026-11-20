using Lab.Core.Interfaces.Repositories;
using Lab.Domain.Entities;
using Lab.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Lab.Persistence.Repositories;

public class SolicitudEstudioRepository : Repository<SolicitudEstudio>, ISolicitudEstudioRepository
{
    public SolicitudEstudioRepository(LabDbContext context) : base(context) { }
}
