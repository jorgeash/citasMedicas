using NugetPackage_Rest.Interfaces.IRequests;

namespace NugetPackage_Rest.Interfaces.IServices
{
    /// <summary>Punto de entrada público de la librería. Es lo único que se inyecta
    /// en los servicios del microservicio consumidor.</summary>
    public interface IRest
    {
        INotContentRequest Get { get; }
        IWithContentRequest Post { get; }
        IWithContentRequest Put { get; }
        IWithContentRequest Delete { get; }
        IWithContentRequest Patch { get; }
    }
}