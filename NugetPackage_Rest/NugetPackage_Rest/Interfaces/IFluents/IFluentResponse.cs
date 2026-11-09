using System.Runtime.CompilerServices;

namespace NugetPackage_Rest.Interfaces.IFluents
{
    /// <summary>Contrato reservado con la misma forma que IFluentContent. Ningún builder
    /// lo implementa hoy; se conserva por compatibilidad con versiones anteriores del paquete.</summary>
    public interface IFluentResponse
    {
        Task<string> GetContentAsStringAsync();
        Task<byte[]> GetContentAsByteArrayAsync();
        Task<T> DeserializeWithAsync<T>();
        TaskAwaiter<HttpResponseMessage> GetAwaiter();
    }
}
