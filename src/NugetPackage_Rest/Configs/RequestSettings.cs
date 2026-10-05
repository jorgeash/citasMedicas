using NugetPackage_Rest.Interfaces.IServices;
using NugetPackage_Rest.Serializers;

namespace NugetPackage_Rest.Configs
{
    /// <summary>Serializador que usa la librería para el body y la respuesta.</summary>
    public enum JsonSerializerChoice
    {
        Newtonsoft = 0,
        SystemTextJson = 1
    }

    /// <summary>Configuración de la librería. Se lee de la sección "RestSettings"
    /// del appsettings.json del microservicio que la consume.</summary>
    public class RequestSettings
    {
        /// <summary>Si es true, cada request se registra en Serilog (método, URL y body).</summary>
        public bool EnableRequestLogs { get; set; }

        /// <summary>Serializador de JSON. Newtonsoft es el valor por defecto porque los DTOs
        /// se anotan con [JsonProperty]. Si se cambia a SystemTextJson, los DTOs deben usar
        /// [JsonPropertyName] en lugar de [JsonProperty].</summary>
        public JsonSerializerChoice JsonSerializer { get; set; } = JsonSerializerChoice.Newtonsoft;

        /// <summary>Instancia del serializador según <see cref="JsonSerializer"/>.
        /// Es de solo lectura: el binder de IOptions solo escribe en las propiedades con setter.</summary>
        public IJsonSerializer Serializer => JsonSerializer == JsonSerializerChoice.SystemTextJson
            ? SystemTextJsonSerializer.Instance
            : NewtonsoftJsonSerializer.Instance;
    }
}