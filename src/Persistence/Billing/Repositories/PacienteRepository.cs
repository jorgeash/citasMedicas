using Domain.Billing.Entities;
using Domain.Billing.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Billing.Context;

namespace Persistence.Billing.Repositories;

public class PacienteRepository : IPacienteRepository
{
    private readonly BillingDbContext _context;

    public PacienteRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Paciente>> GetAllAsync()
    {
        return await _context.Pacientes.ToListAsync();
    }

    public async Task<Paciente?> GetByIdAsync(long id)
    {
        return await _context.Pacientes.FindAsync(id);
    }

    public async Task<Paciente> CreateAsync(Paciente paciente)
    {
        await _context.Pacientes.AddAsync(paciente);
        await _context.SaveChangesAsync();

        return paciente;
    }

    public async Task<Paciente> UpdateAsync(Paciente paciente)
    {
        _context.Pacientes.Update(paciente);
        await _context.SaveChangesAsync();

        return paciente;
    }

    public async Task<bool> DeleteAsync(Paciente paciente)
    {
        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();

        return true;
    }
}