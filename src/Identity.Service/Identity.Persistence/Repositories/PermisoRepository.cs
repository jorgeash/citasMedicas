using Identity.Core.Interfaces.Repositories;
using Identity.Domain.Entities;
using Identity.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Repositories;

namespace Identity.Persistence.Repositories;

public class PermisoRepository : Repository<Permiso>, IPermisoRepository
{
    public PermisoRepository(IdentityDbContext context) : base(context) { }

    public async Task<IReadOnlyCollection<string>> GetCodigosPorRolAsync(int rolID, CancellationToken cancellationToken = default)
    {
        // Sin JOIN cross-service: todo vive en ClinicaDB, es una consulta interna.
        // Set<RolPermiso>() porque el base expone DbContext, no el contexto concreto.
        return await _context.Set<RolPermiso>()
            .AsNoTracking()
            .Where(rp => rp.RolID == rolID)
            .Select(rp => rp.Permiso!.Codigo)
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);
    }
}
