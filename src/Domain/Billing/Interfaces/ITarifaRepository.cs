using Domain.Billing.Entities;
using Domain.Shared.Interfaces;

namespace Domain.Billing.Interfaces;

public interface ITarifaRepository : IGenericRepository<Tarifa, int>
{
    Task<IEnumerable<Tarifa>> GetByServicioIdAsync(int servicioId, System.Threading.CancellationToken cancellationToken = default);
}
