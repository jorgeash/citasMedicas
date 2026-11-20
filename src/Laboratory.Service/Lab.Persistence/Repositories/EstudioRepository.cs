using Lab.Core.Interfaces.Repositories;
using Lab.Domain.Entities;
using Lab.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Lab.Persistence.Repositories;

public class EstudioRepository : Repository<Estudio>, IEstudioRepository
{
    public EstudioRepository(LabDbContext context) : base(context) { }
}
