using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class TratamientoDetalleRepository : Repository<TratamientoDetalle>, ITratamientoDetalleRepository
{
    public TratamientoDetalleRepository(ClinicalDbContext context) : base(context) { }
}
