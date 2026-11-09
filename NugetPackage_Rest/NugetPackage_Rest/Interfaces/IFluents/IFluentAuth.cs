using System.Diagnostics.CodeAnalysis;

namespace NugetPackage_Rest.Interfaces.IFluents
{
    /// <summary>Eslabón posterior a la autenticación: headers opcionales y URI obligatoria.
    /// TNext es el siguiente eslabón: IFluentContent (GET) o IFluentFormat (con body).</summary>
    public interface IFluentAuth<TNext>
    {
        TNext WithUri([NotNull] string uri, string endpoint = "");
        IFluentAuth<TNext> WithHeaders([NotNull] Dictionary<string, string> keyValues);
    }
}
