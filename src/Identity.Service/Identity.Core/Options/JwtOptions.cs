namespace Identity.Core.Options;

/// <summary>
/// Configuración JWT. La misma sección <c>Jwt</c> y la misma clave de firma se
/// replican en Billing.Api y Scheduling.Api para que los tres servicios validen
/// tokens emitidos por Identity sin llamarse entre sí.
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    /// <summary>Clave HMAC-SHA256 en base64. Debe medir al menos 32 bytes.</summary>
    public string SigningKey { get; set; } = string.Empty;

    /// <summary>Vida del token de acceso, en minutos.</summary>
    public int AccessTokenMinutes { get; set; } = 60;

    /// <summary>Vida del token de refresco, en días.</summary>
    public int RefreshTokenDays { get; set; } = 7;
}
