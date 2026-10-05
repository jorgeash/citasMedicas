using NugetPackage_Rest.Interfaces.IFluents;

namespace NugetPackage_Rest.Interfaces.IRequests
{
    /// <summary>Primer eslabón de un request CON body (POST/PUT/PATCH/DELETE).</summary>
    public interface IWithContentRequest
    {
        IFluentAuth<IFluentFormat> WithoutAuth();
        IFluentAuth<IFluentFormat> WithBearer(string token);
        IFluentAuth<IFluentFormat> WithBasic(string user, string password);
    }
}