using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class AntecedenteMedicoRepository : Repository<AntecedenteMedico>, IAntecedenteMedicoRepository
{
    public AntecedenteMedicoRepository(ClinicalDbContext context) : base(context) { }
}
