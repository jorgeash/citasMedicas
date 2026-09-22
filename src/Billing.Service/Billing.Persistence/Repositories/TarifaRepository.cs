using Billing.Core.Interfaces.Repositories;
using Billing.Domain.Entities;
using Billing.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Billing.Persistence.Repositories;

public class TarifaRepository : Repository<Tarifa>, ITarifaRepository
{
    public TarifaRepository(BillingDbContext context) : base(context) { }
}
