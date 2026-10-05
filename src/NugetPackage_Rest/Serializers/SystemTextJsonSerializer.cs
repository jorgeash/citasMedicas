using NugetPackage_Rest.Interfaces.IServices;
using System.Text.Json;

namespace NugetPackage_Rest.Serializers
{
    /// <summary>Implementación con System.Text.Json. Ojo: ignora los atributos [JsonProperty]
    /// de Newtonsoft; con esta opción los DTOs deben anotarse con [JsonPropertyName].</summary>
    public sealed class SystemTextJsonSerializer : IJsonSerializer
    {
        public static readonly SystemTextJsonSerializer Instance = new();

        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

        public string Serialize(object? value) => JsonSerializer.Serialize(value, value?.GetType() ?? typeof(object), Options);

        public T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);
    }
}