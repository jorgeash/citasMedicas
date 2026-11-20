using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class ArchivoClinicoRepository : Repository<ArchivoClinico>, IArchivoClinicoRepository
{
    public ArchivoClinicoRepository(ClinicalDbContext context) : base(context) { }
}
