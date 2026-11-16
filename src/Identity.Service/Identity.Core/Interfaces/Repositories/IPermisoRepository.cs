using Identity.Domain.Entities;
using Shared.Kernel.Interfaces;

namespace Identity.Core.Interfaces.Repositories;

public interface IPermisoRepository : IRepository<Permiso>
{
    /// <summary>Códigos de permiso asignados a un rol.</summary>
    Task<IReadOnlyCollection<string>> GetCodigosPorRolAsync(int rolID, CancellationToken cancellationToken = default);
}
