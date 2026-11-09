using System.Runtime.CompilerServices;

namespace NugetPackage_Rest.Interfaces.IFluents
{
    /// <summary>Último eslabón de la cadena: ejecuta el request y lee la respuesta.</summary>
    public interface IFluentContent
    {
        Task<string> GetContentAsStringAsync();
        Task<byte[]> GetContentAsByteArrayAsync();
        Task<T> DeserializeWithAsync<T>();
        TaskAwaiter<HttpResponseMessage> GetAwaiter();
    }
}
