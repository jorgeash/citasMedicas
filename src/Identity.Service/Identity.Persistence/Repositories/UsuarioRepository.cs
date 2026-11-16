using Identity.Core.Interfaces.Repositories;
using Identity.Domain.Entities;
using Identity.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Repositories;

namespace Identity.Persistence.Repositories;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(IdentityDbContext context) : base(context) { }

    public Task<Usuario?> GetCompletoPorNombreAsync(string nombreUsuario, CancellationToken cancellationToken = default)
    {
        // ToLower en BD: la colación de SQL Server ya es insensible, pero así el
        // mismo predicado también funciona en los tests en memoria.
        var normalizado = nombreUsuario.Trim().ToLower();

        return _dbSet
            .AsNoTracking()
            .Include(u => u.Rol)
            .ThenInclude(r => r!.Permisos)
            .FirstOrDefaultAsync(
                u => u.NombreUsuario.ToLower() == normalizado,
                cancellationToken);
    }

    public Task<Usuario?> GetCompletoPorIdAsync(int usuarioID, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .AsNoTracking()
            .Include(u => u.Rol)
            .ThenInclude(r => r!.Permisos)
            .FirstOrDefaultAsync(u => u.UsuarioID == usuarioID, cancellationToken);
    }

    public async Task RegistrarAccesoAsync(int usuarioID, DateTime fecha, CancellationToken cancellationToken = default)
    {
        var usuario = await _dbSet.FirstOrDefaultAsync(u => u.UsuarioID == usuarioID, cancellationToken);
        if (usuario is null)
            return;

        usuario.UltimoAcceso = fecha;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
