using Domain.Billing.Entities;
using Domain.Billing.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Billing.Context;

namespace Persistence.Billing.Repositories;

public class ServicioRepository : IServicioRepository
{
    private readonly BillingDbContext _context;

    public ServicioRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Servicio>> GetAllAsync()
    {
        return await _context.Servicios.ToListAsync();
    }

    public async Task<Servicio?> GetByIdAsync(int id)
    {
        return await _context.Servicios.FindAsync(id);
    }

    public async Task<Servicio> AddAsync(Servicio servicio)
    {
        _context.Servicios.Add(servicio);
        await _context.SaveChangesAsync();
        return servicio;
    }
}
