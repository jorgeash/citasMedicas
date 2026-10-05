using Newtonsoft.Json;

namespace Billing.Core.DTOs.External;

/// <summary>
/// Contratos de la pasarela de pagos externa. La librería serializa y deserializa con
/// Newtonsoft.Json, por eso los atributos son [JsonProperty] y no [JsonPropertyName].
/// </summary>
public class CobroRequest
{
    [JsonProperty("referencia")]
    public string Referencia { get; set; } = string.Empty;

    [JsonProperty("monto")]
    public decimal Monto { get; set; }

    [JsonProperty("moneda")]
    public string Moneda { get; set; } = "USD";
}

public class CobroResponse
{
    [JsonProperty("estado")]
    public string Estado { get; set; } = string.Empty;

    [JsonProperty("transaccionId")]
    public string? TransaccionId { get; set; }

    [JsonProperty("mensajes")]
    public List<string>? Mensajes { get; set; }
}

public class TokenResponse
{
    [JsonProperty("access_token")]
    public string? AccessToken { get; set; }

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
}