using Identity.Domain.Entities;
using Shared.Kernel.Interfaces;

namespace Identity.Core.Interfaces.Repositories;

/// <summary>
/// Bitácora de accesos (<c>dbo.Sesiones_Usuario</c>). Es solo auditoría: la
/// validación de tokens no pasa por aquí.
/// </summary>
public interface ISesionRepository : IRepository<SesionUsuario>
{
    /// <summary>Abre una fila de sesión y devuelve su id.</summary>
    Task<long> AbrirAsync(
        int usuarioID,
        string? tokenSesion,
        DateTime fechaInicio,
        string? direccionIP,
        string? dispositivo,
        CancellationToken cancellationToken = default);

    /// <summary>Cierra todas las sesiones abiertas del usuario. Devuelve cuántas cerró.</summary>
    Task<int> CerrarTodasAsync(int usuarioID, DateTime fechaFin, CancellationToken cancellationToken = default);
}
