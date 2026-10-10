namespace Business.RestClient;

/// <summary>
/// Sobre de resposta de los microservicios. Se replica aqui, en el cliente, a proposito:
/// un consumidor externo no debe depender de los ensamblados internos de los servicios.
/// Los atributos [JsonProperty] son de Newtonsoft porque es el serializador de la libreria.
/// </summary>
public class ApiResponse<T>
{
    [Newtonsoft.Json.JsonProperty("succeeded")]
    public bool Succeeded { get; set; } = true;

    [Newtonsoft.Json.JsonProperty("data")]
    public T Data { get; set; } = default!;

    [Newtonsoft.Json.JsonProperty("errorCode")]
    public int ErrorCode { get; set; }

    [Newtonsoft.Json.JsonProperty("errorMessage")]
    public string? ErrorMessage { get; set; }
}

public class PagedData<T>
{
    public int TotalRecords { get; set; }
    public int TotalPage { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public List<T> Data { get; set; } = new();
}

/// <summary>Direccion del catalogo de Addresses (tabla Addresses_Position).</summary>
public class LocationContract
{
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int? CustomerId { get; set; }
    public bool IsActive { get; set; }
}

public class LocationSummary
{
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}

/// <summary>Cliente devuelto por POST/GET /api/customers (tabla Business_Customer).</summary>
public class CustomerContract
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int CustomerTypeId { get; set; }
    public string CustomerTypeName { get; set; } = string.Empty;
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public LocationSummary? Location { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>Body de POST /api/customers. LocationId es la referencia a la direccion.</summary>
public class NewCustomerRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int CustomerTypeId { get; set; }
    public int CompanyId { get; set; }
    public int LocationId { get; set; }
}