using Domain.Billing.Entities;

namespace Domain.Billing.Interfaces;

public interface IServicioRepository
{
    Task<IEnumerable<Servicio>> GetAllAsync();
    Task<Servicio?> GetByIdAsync(int id);
}
