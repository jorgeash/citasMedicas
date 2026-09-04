using Domain.Billing.Entities;
using Domain.Billing.Interfaces;
using Persistence.Billing.Context;
using Persistence.Shared.Repositories;

namespace Persistence.Billing.Repositories;

public class ServicioRepository : GenericRepository<Servicio, int>, IServicioRepository
{
    public ServicioRepository(BillingDbContext context) : base(context)
    {
    }
}
