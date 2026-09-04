using Domain.Billing.Entities;
using Domain.Shared.Interfaces;

namespace Domain.Billing.Interfaces;

public interface IServicioRepository : IGenericRepository<Servicio, int>
{
    // Añadir métodos específicos de Servicio si son necesarios
}
