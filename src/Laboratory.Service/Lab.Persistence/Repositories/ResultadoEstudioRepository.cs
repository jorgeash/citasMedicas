using Lab.Core.Interfaces.Repositories;
using Lab.Domain.Entities;
using Lab.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Lab.Persistence.Repositories;

public class ResultadoEstudioRepository : Repository<ResultadoEstudio>, IResultadoEstudioRepository
{
    public ResultadoEstudioRepository(LabDbContext context) : base(context) { }
}
