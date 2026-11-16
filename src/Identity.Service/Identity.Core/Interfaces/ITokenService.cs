using Identity.Core.DTOs.Identity;
using Identity.Domain.Entities;

namespace Identity.Core.Interfaces;

/// <summary>Emisión y validación de tokens JWT (stateless, sin estado en BD).</summary>
public interface ITokenService
{
    /// <summary>Emite el JWT de acceso con los claims del usuario y su rol.</summary>
    (string Token, int ExpiresIn) CreateAccessToken(Usuario usuario, IReadOnlyCollection<string> permisos);

    /// <summary>Emite el JWT de refresco (claim <c>typ=refresh</c>), con vida más larga.</summary>
    string CreateRefreshToken(Usuario usuario);

    /// <summary>
    /// Valida la firma y la vigencia de un refresh token. Devuelve el usuario que
    /// lo emitió o <c>null</c> si es inválido, está vencido o no es de tipo refresh.
    /// </summary>
    Usuario? ValidateRefreshToken(string token);
}
