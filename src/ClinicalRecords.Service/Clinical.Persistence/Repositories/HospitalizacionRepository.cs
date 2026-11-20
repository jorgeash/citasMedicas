using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class HospitalizacionRepository : Repository<Hospitalizacion>, IHospitalizacionRepository
{
    public HospitalizacionRepository(ClinicalDbContext context) : base(context) { }
}
