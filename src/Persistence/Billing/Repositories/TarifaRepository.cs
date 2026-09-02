using Domain.Billing.Entities;
using Domain.Billing.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Billing.Context;

namespace Persistence.Billing.Repositories;

public class TarifaRepository : ITarifaRepository
{
    private readonly BillingDbContext _context;

    public TarifaRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tarifa>> GetAllAsync()
    {
        return await _context.Tarifas
            .Include(t => t.Servicio)
            .ToListAsync();
    }

    public async Task<Tarifa?> GetByIdAsync(int id)
    {
        return await _context.Tarifas
            .Include(t => t.Servicio)
            .FirstOrDefaultAsync(t => t.TarifaID == id);
    }

    public async Task<IEnumerable<Tarifa>> GetByServicioIdAsync(int servicioId)
    {
        return await _context.Tarifas
            .Include(t => t.Servicio)
            .Where(t => t.ServicioID == servicioId)
            .ToListAsync();
    }

    public async Task<Tarifa> AddAsync(Tarifa tarifa)
    {
        _context.Tarifas.Add(tarifa);
        await _context.SaveChangesAsync();
        return tarifa;
    }
}
