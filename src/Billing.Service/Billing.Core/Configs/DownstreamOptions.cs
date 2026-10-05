namespace Billing.Core.Configs;

/// <summary>
/// URLs base de los otros microservicios a los que llama Billing.
/// </summary>
public class DownstreamOptions
{
    /// <summary>Microservicio de Citas: verifica disponibilidad de un servicio antes de facturar.</summary>
    public string CitasBaseUrl { get; set; } = string.Empty;

    /// <summary>Microservicio de Pagos: registra y liquida la factura.</summary>
    public string PagosBaseUrl { get; set; } = string.Empty;

    /// <summary>Microservicio de Reportes: genera y almacena comprobantes.</summary>
    public string ReportesBaseUrl { get; set; } = string.Empty;
}