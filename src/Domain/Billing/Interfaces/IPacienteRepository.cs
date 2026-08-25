using Domain.Billing.Entities;

namespace Domain.Billing.Interfaces;

public interface IPacienteRepository
{
    Task<IEnumerable<Paciente>> GetAllAsync();

    Task<Paciente?> GetByIdAsync(long id);

    Task<Paciente> CreateAsync(Paciente paciente);

    Task<Paciente> UpdateAsync(Paciente paciente);

    Task<bool> DeleteAsync(Paciente paciente);
}