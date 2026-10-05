namespace NugetPackage_Rest.Exceptions
{
    /// <summary>Única excepción que lanza la librería. Lleva el motivo, el código
    /// HTTP y el body de la respuesta para que el consumidor decida qué hacer.</summary>
    public class ApiException : Exception
    {
        public ApiFailureReason Reason { get; }
        public int? StatusCode { get; }
        public string? ResponseBody { get; }
        public HttpMethod? RequestMethod { get; }
        public Uri? RequestUri { get; }

        public ApiException(
            string message,
            ApiFailureReason reason,
            Exception? innerException = null,
            int? statusCode = null,
            string? responseBody = null,
            HttpMethod? requestMethod = null,
            Uri? requestUri = null)
            : base(message, innerException)
        {
            Reason = reason;
            StatusCode = statusCode;
            ResponseBody = responseBody;
            RequestMethod = requestMethod;
            RequestUri = requestUri;
        }
    }
}