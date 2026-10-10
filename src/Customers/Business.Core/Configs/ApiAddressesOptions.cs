namespace Business.Core.Configs;

/// <summary>
/// URLs base de los microservicios a los que llama Customers. La de Addresses es la que
/// usa el cliente IRest de la libreria NugetPackage_Rest para resolver el LocationId.
/// </summary>
public class ApiAddressesOptions
{
    /// <summary>Microservicio de direcciones (catalogo Addresses_Position).</summary>
    public string AddressesBaseUrl { get; set; } = string.Empty;
}