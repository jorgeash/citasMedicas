using System.Diagnostics.CodeAnalysis;

namespace NugetPackage_Rest.Interfaces.IFluents
{
    /// <summary>Eslabón exclusivo de POST/PUT/PATCH/DELETE: define el body.</summary>
    public interface IFluentFormat
    {
        IFluentContent WithoutBody();
        IFluentContent WithBody([NotNull] object body);
        IFluentContent WithFormData([NotNull] MultipartFormDataContent content);

        /// <summary>Envía el body como application/x-www-form-urlencoded — distinto de
        /// <see cref="WithFormData"/>, que envía multipart/form-data. Algunas APIs (p. ej.
        /// endpoints de token estilo OAuth) exigen urlencoded y rechazan multipart.</summary>
        IFluentContent WithFormUrlEncoded([NotNull] IDictionary<string, string> data);
    }
}