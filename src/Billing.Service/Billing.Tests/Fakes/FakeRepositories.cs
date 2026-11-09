using Billing.Core.Interfaces.Repositories;
using Billing.Domain.Entities;

namespace Billing.Tests.Fakes;

public class FakeFacturaRepository : InMemoryRepository<Factura>, IFacturaRepository
{
    public FakeFacturaRepository()
        : base(f => f.FacturaID, (f, id) => f.FacturaID = id)
    {
    }
}

public class FakeTarifaRepository : InMemoryRepository<Tarifa>, ITarifaRepository
{
    public FakeTarifaRepository()
        : base(t => t.TarifaID, (t, id) => t.TarifaID = id)
    {
    }
}
