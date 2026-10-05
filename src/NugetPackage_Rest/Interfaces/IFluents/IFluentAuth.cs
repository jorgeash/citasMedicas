using System.Diagnostics.CodeAnalysis;

namespace NugetPackage_Rest.Interfaces.IFluents
{
    /// <summary>Eslabón posterior a la autenticación: headers y query opcionales, URI obligatoria.
    /// TNext es el siguiente eslabón: IFluentContent (GET) o IFluentFormat (con body).</summary>
    public interface IFluentAuth<TNext>
    {
        TNext WithUri([NotNull] string uri, string endpoint = "");
        IFluentAuth<TNext> WithHeaders([NotNull] Dictionary<string, string> keyValues);

        /// <summary>Agrega parámetros a la query string de la URI, escapando cada valor con
        /// Uri.EscapeDataString. Se puede llamar varias veces antes de WithUri y combinarse
        /// con un endpoint que ya traiga "?": los parámetros se unen con "&amp;".</summary>
        IFluentAuth<TNext> WithQuery([NotNull] IDictionary<string, string> parametros);
    }
}