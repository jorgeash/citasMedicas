namespace NugetPackage_Rest.Interfaces.IServices
{
    /// <summary>Serializador de JSON intercambiable. Permite usar Newtonsoft.Json (por defecto,
    /// porque los DTOs se anotan con [JsonProperty]) o System.Text.Json sin tocar los builders.
    /// Se elige con RequestSettings.JsonSerializer.</summary>
    public interface IJsonSerializer
    {
        string Serialize(object? value);

        T? Deserialize<T>(string json);
    }
}