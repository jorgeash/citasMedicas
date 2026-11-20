using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class AntecedenteFamiliarRepository : Repository<AntecedenteFamiliar>, IAntecedenteFamiliarRepository
{
    public AntecedenteFamiliarRepository(ClinicalDbContext context) : base(context) { }
}
