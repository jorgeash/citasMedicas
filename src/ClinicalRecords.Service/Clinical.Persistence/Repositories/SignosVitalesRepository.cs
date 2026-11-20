using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class SignosVitalesRepository : Repository<SignosVitales>, ISignosVitalesRepository
{
    public SignosVitalesRepository(ClinicalDbContext context) : base(context) { }
}
