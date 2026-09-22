using Billing.Core.Interfaces.Repositories;
using Billing.Domain.Entities;
using Billing.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Billing.Persistence.Repositories;

public class ServicioRepository : Repository<Servicio>, IServicioRepository
{
    public ServicioRepository(BillingDbContext context) : base(context) { }
}
