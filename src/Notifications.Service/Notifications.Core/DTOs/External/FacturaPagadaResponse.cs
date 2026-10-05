using Newtonsoft.Json;

namespace Notifications.Core.DTOs.External;

/// <summary>
/// Factura pagada que devuelve el microservicio de Billing. Los atributos son [JsonProperty]
/// de Newtonsoft (no [JsonPropertyName] de System.Text.Json), porque la librería
/// NugetPackage_Rest serializa y deserializa con Newtonsoft.
/// </summary>
public class FacturaPagadaResponse
{
    [JsonProperty("facturaId")]
    public int FacturaId { get; set; }

    [JsonProperty("paciente")]
    public string Paciente { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("monto")]
    public decimal Monto { get; set; }

    [JsonProperty("moneda")]
    public string Moneda { get; set; } = "USD";

    [JsonProperty("estado")]
    public string Estado { get; set; } = string.Empty;

    [JsonProperty("transaccionId")]
    public string? TransaccionId { get; set; }
}
