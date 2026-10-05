using System.Runtime.CompilerServices;

namespace NugetPackage_Rest.Interfaces.IFluents
{
    /// <summary>Último eslabón de la cadena: ejecuta el request y lee la respuesta.
    /// Todos los métodos terminales aceptan un CancellationToken opcional: al cancelarlo se
    /// lanza ApiException con Reason = Timeout (para distinguir una cancelación pedida por el
    /// usuario de un timeout real, revise Message o el CancellationToken del consumidor).</summary>
    public interface IFluentContent
    {
        Task<string> GetContentAsStringAsync(CancellationToken cancellationToken = default);
        Task<byte[]> GetContentAsByteArrayAsync(CancellationToken cancellationToken = default);
        Task<T> DeserializeWithAsync<T>(CancellationToken cancellationToken = default);
        TaskAwaiter<HttpResponseMessage> GetAwaiter();
    }
}