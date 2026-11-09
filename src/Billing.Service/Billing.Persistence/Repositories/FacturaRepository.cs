using Billing.Core.Interfaces.Repositories;
using Billing.Domain.Entities;
using Billing.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Billing.Persistence.Repositories;

public class FacturaRepository : Repository<Factura>, IFacturaRepository
{
    public FacturaRepository(BillingDbContext context) : base(context) { }
}
