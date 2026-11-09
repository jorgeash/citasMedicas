namespace NugetPackage_Rest.Configs
{
    /// <summary>Configuración de la librería. Se lee de la sección "RestSettings"
    /// del appsettings.json del microservicio que la consume.</summary>
    public class RequestSettings
    {
        /// <summary>Si es true, cada request se registra en Serilog (método, URL y body).</summary>
        public bool EnableRequestLogs { get; set; }
    }
}
