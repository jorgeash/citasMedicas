using Identity.Core.Interfaces.Repositories;
using Identity.Domain.Entities;

namespace Identity.Tests.Fakes;

public class FakeUsuarioRepository : InMemoryRepository<Usuario>, IUsuarioRepository
{
    public FakeUsuarioRepository()
        : base(u => u.UsuarioID, (u, id) => u.UsuarioID = (int)id)
    {
    }

    // Misma semántica que el repo real: insensible a mayúsculas/minúsculas.
    public Task<Usuario?> GetCompletoPorNombreAsync(string nombreUsuario, CancellationToken cancellationToken = default)
    {
        var normalizado = nombreUsuario.Trim().ToLower();
        return Task.FromResult(Items.FirstOrDefault(u => u.NombreUsuario.ToLower() == normalizado));
    }

    public Task<Usuario?> GetCompletoPorIdAsync(int usuarioID, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.FirstOrDefault(u => u.UsuarioID == usuarioID));

    public Task RegistrarAccesoAsync(int usuarioID, DateTime fecha, CancellationToken cancellationToken = default)
    {
        var usuario = Items.FirstOrDefault(u => u.UsuarioID == usuarioID);
        if (usuario is not null)
            usuario.UltimoAcceso = fecha;

        return Task.CompletedTask;
    }
}

public class FakePermisoRepository : InMemoryRepository<Permiso>, IPermisoRepository
{
    private readonly List<RolPermiso> _relaciones = new();

    public FakePermisoRepository()
        : base(p => p.PermisoID, (p, id) => p.PermisoID = (int)id)
    {
    }

    /// <summary>Asigna códigos de permiso a un rol, como haría el seed.</summary>
    public void AsignarRol(int rolID, params string[] codigos)
    {
        foreach (var codigo in codigos)
        {
            var permiso = Items.FirstOrDefault(p => p.Codigo == codigo)
                ?? throw new InvalidOperationException($"El permiso '{codigo}' no está sembrado.");

            _relaciones.Add(new RolPermiso { RolID = rolID, PermisoID = permiso.PermisoID });
        }
    }

    public Task<IReadOnlyCollection<string>> GetCodigosPorRolAsync(int rolID, CancellationToken cancellationToken = default)
    {
        var ids = _relaciones.Where(r => r.RolID == rolID).Select(r => r.PermisoID).ToHashSet();

        IReadOnlyCollection<string> codigos = Items
            .Where(p => ids.Contains(p.PermisoID))
            .Select(p => p.Codigo)
            .OrderBy(c => c, StringComparer.Ordinal)
            .ToList();

        return Task.FromResult(codigos);
    }
}

public class FakeSesionRepository : InMemoryRepository<SesionUsuario>, ISesionRepository
{
    public FakeSesionRepository()
        : base(s => s.SesionID, (s, id) => s.SesionID = id)
    {
    }

    public Task<long> AbrirAsync(
        int usuarioID,
        string? tokenSesion,
        DateTime fechaInicio,
        string? direccionIP,
        string? dispositivo,
        CancellationToken cancellationToken = default)
    {
        var sesion = new SesionUsuario
        {
            SesionID = Items.Count == 0 ? 1 : Items.Max(s => s.SesionID) + 1,
            UsuarioID = usuarioID,
            TokenSesion = tokenSesion,
            FechaInicio = fechaInicio,
            DireccionIP = direccionIP,
            Dispositivo = dispositivo,
            Activa = true
        };

        Items.Add(sesion);
        return Task.FromResult(sesion.SesionID);
    }

    public Task<int> CerrarTodasAsync(int usuarioID, DateTime fechaFin, CancellationToken cancellationToken = default)
    {
        var abiertas = Items.Where(s => s.UsuarioID == usuarioID && s.Activa).ToList();

        foreach (var sesion in abiertas)
        {
            sesion.Activa = false;
            sesion.FechaFin = fechaFin;
        }

        return Task.FromResult(abiertas.Count);
    }
}
