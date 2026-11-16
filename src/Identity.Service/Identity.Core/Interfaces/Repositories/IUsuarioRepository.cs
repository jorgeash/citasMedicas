using Identity.Domain.Entities;
using Shared.Kernel.Interfaces;

namespace Identity.Core.Interfaces.Repositories;

public interface IUsuarioRepository : IRepository<Usuario>
{
    /// <summary>Usuario con su rol y permisos cargados, por nombre de usuario.</summary>
    Task<Usuario?> GetCompletoPorNombreAsync(string nombreUsuario, CancellationToken cancellationToken = default);

    /// <summary>Usuario con su rol y permisos cargados, por id.</summary>
    Task<Usuario?> GetCompletoPorIdAsync(int usuarioID, CancellationToken cancellationToken = default);

    /// <summary>Registra el último acceso exitoso del usuario.</summary>
    Task RegistrarAccesoAsync(int usuarioID, DateTime fecha, CancellationToken cancellationToken = default);
}
