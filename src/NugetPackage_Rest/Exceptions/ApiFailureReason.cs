namespace NugetPackage_Rest.Exceptions
{
    /// <summary>Clasifica por qué falló una llamada HTTP.</summary>
    public enum ApiFailureReason
    {
        Unknown,
        Network,
        Timeout,
        HttpError,
        Deserialization
    }
}