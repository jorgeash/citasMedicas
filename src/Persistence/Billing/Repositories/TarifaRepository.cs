using Domain.Billing.Entities;
using Domain.Billing.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Billing.Context;
using Persistence.Shared.Repositories;

namespace Persistence.Billing.Repositories;

public class TarifaRepository : GenericRepository<Tarifa, int>, ITarifaRepository
{
    public TarifaRepository(BillingDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Tarifa>> GetAllAsync(System.Threading.CancellationToken cancellationToken = default)
    {
        return await _context.Set<Tarifa>()
            .Include(t => t.Servicio)
            .ToListAsync(cancellationToken);
    }

    public override async Task<Tarifa?> GetByIdAsync(int id, System.Threading.CancellationToken cancellationToken = default)
    {
        return await _context.Set<Tarifa>()
            .Include(t => t.Servicio)
            .FirstOrDefaultAsync(t => t.TarifaID == id, cancellationToken);
    }

    public async Task<IEnumerable<Tarifa>> GetByServicioIdAsync(int servicioId, System.Threading.CancellationToken cancellationToken = default)
    {
        return await _context.Set<Tarifa>()
            .Include(t => t.Servicio)
            .Where(t => t.ServicioID == servicioId)
            .ToListAsync(cancellationToken);
    }
}
