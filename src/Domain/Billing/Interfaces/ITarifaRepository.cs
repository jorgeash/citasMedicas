using Domain.Billing.Entities;

namespace Domain.Billing.Interfaces;

public interface ITarifaRepository
{
    Task<IEnumerable<Tarifa>> GetAllAsync();
    Task<Tarifa?> GetByIdAsync(int id);
    Task<IEnumerable<Tarifa>> GetByServicioIdAsync(int servicioId);
    Task<Tarifa> AddAsync(Tarifa tarifa);
}
