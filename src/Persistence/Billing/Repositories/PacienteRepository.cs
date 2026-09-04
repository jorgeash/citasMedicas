using Domain.Billing.Entities;
using Domain.Billing.Interfaces;
using Persistence.Billing.Context;
using Persistence.Shared.Repositories;

namespace Persistence.Billing.Repositories;

public class PacienteRepository : GenericRepository<Paciente, long>, IPacienteRepository
{
    public PacienteRepository(BillingDbContext context) : base(context)
    {
    }
}
