namespace Billing.Core.Configs;

/// <summary>
/// Datos de conexión a la pasarela de pagos externa (API con OAuth 2.0).
/// </summary>
public class PasarelaOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public int MaxReintentos { get; set; } = 2;
}