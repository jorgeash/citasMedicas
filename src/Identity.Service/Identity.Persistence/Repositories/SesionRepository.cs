using Identity.Core.Interfaces.Repositories;
using Identity.Domain.Entities;
using Identity.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Repositories;

namespace Identity.Persistence.Repositories;

public class SesionRepository : Repository<SesionUsuario>, ISesionRepository
{
    public SesionRepository(IdentityDbContext context) : base(context) { }

    public async Task<long> AbrirAsync(
        int usuarioID,
        string? tokenSesion,
        DateTime fechaInicio,
        string? direccionIP,
        string? dispositivo,
        CancellationToken cancellationToken = default)
    {
        // TokenSesion es varchar(500) y Dispositivo nvarchar(600): se recortan
        // para que un User-Agent largo no reviente el INSERT.
        var sesion = new SesionUsuario
        {
            UsuarioID = usuarioID,
            TokenSesion = Recortar(tokenSesion, 500),
            FechaInicio = fechaInicio,
            DireccionIP = Recortar(direccionIP, 50),
            Dispositivo = Recortar(dispositivo, 600),
            Activa = true
        };

        await _dbSet.AddAsync(sesion, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sesion.SesionID;
    }

    public async Task<int> CerrarTodasAsync(int usuarioID, DateTime fechaFin, CancellationToken cancellationToken = default)
    {
        var abiertas = await _dbSet
            .Where(s => s.UsuarioID == usuarioID && s.Activa)
            .ToListAsync(cancellationToken);

        if (abiertas.Count == 0)
            return 0;

        foreach (var sesion in abiertas)
        {
            sesion.Activa = false;
            sesion.FechaFin = fechaFin;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return abiertas.Count;
    }

    private static string? Recortar(string? valor, int maximo)
        => valor is null ? null : valor.Length <= maximo ? valor : valor[..maximo];
}
