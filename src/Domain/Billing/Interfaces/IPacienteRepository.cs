using Domain.Billing.Entities;
using Domain.Shared.Interfaces;

namespace Domain.Billing.Interfaces;

public interface IPacienteRepository : IGenericRepository<Paciente, long>
{
    // Añadir métodos específicos de Paciente si son necesarios
}
