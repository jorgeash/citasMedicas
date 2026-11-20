using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class AlergiaRepository : Repository<Alergia>, IAlergiaRepository
{
    public AlergiaRepository(ClinicalDbContext context) : base(context) { }
}
