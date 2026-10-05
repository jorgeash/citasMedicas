using NugetPackage_Rest.Interfaces.IFluents;

namespace NugetPackage_Rest.Interfaces.IRequests
{
    /// <summary>Primer eslabón de un request SIN body (GET): elegir autenticación.</summary>
    public interface INotContentRequest
    {
        IFluentAuth<IFluentContent> WithoutAuth();
        IFluentAuth<IFluentContent> WithBearer(string token);
        IFluentAuth<IFluentContent> WithBasic(string user, string password);
    }
}