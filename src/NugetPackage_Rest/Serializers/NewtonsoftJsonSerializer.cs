using NugetPackage_Rest.Interfaces.IServices;
using Newtonsoft.Json;

namespace NugetPackage_Rest.Serializers
{
    /// <summary>Implementación con Newtonsoft.Json. Es la opción por defecto porque respeta
    /// los atributos [JsonProperty] de los DTOs de los microservicios.</summary>
    public sealed class NewtonsoftJsonSerializer : IJsonSerializer
    {
        public static readonly NewtonsoftJsonSerializer Instance = new();

        public string Serialize(object? value) => JsonConvert.SerializeObject(value);

        public T? Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json);
    }
}