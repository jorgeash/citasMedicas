using Newtonsoft.Json;

namespace Notifications.Core.DTOs;

/// <summary>
/// Sobre estándar con el que responden todos los microservicios del sistema, tanto en
/// éxito como en error. Los atributos [JsonProperty] son de Newtonsoft porque es el
/// serializador que usa la librería NugetPackage_Rest.
/// </summary>
public class HttpResponse<T>
{
    [JsonProperty("succeeded")]
    public bool Succeeded { get; set; } = true;

    [JsonProperty("data")]
    public T Data { get; set; } = default!;

    [JsonProperty("errorCode")]
    public int ErrorCode { get; set; }

    [JsonProperty("errorMessage")]
    public string? ErrorMessage { get; set; }

    /// <summary>Constructor para deserializar la respuesta de otro microservicio.</summary>
    public HttpResponse()
    {
    }

    public HttpResponse(T data)
    {
        Data = data;
    }

    /// <summary>Sobres de error, para que el middleware no arme un tipo anónimo aparte.</summary>
    public static HttpResponse<T> Failure(int errorCode, string? errorMessage) => new()
    {
        Succeeded = false,
        ErrorCode = errorCode,
        ErrorMessage = errorMessage
    };
}
