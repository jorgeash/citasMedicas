using NugetPackage_Rest.Exceptions;
using Serilog;

namespace NugetPackage_Rest.Builders
{
    /// <summary>Centraliza el envío y el manejo de errores: todo fallo se convierte
    /// en ApiException con su ApiFailureReason y queda registrado en Serilog.</summary>
    internal static class HttpRequestExecutor
    {
        public static async Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            try
            {
                return await client.SendAsync(request, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                throw LogAndBuild(
                    $"Request {request.Method} {request.RequestUri} failed due to a network error: {ex.Message}",
                    ApiFailureReason.Network, request, ex);
            }
            catch (TaskCanceledException ex)
            {
                throw LogAndBuild(DescribeCancellation(request, cancellationToken), ApiFailureReason.Timeout, request, ex);
            }
            catch (OperationCanceledException ex)
            {
                throw LogAndBuild(DescribeCancellation(request, cancellationToken), ApiFailureReason.Timeout, request, ex);
            }
            catch (Exception ex)
            {
                throw LogAndBuild(
                    $"An unexpected error occurred while sending the request {request.Method} {request.RequestUri}: {ex.Message}",
                    ApiFailureReason.Unknown, request, ex);
            }
        }

        public static async Task EnsureSuccessAsync(HttpResponseMessage response, HttpRequestMessage request)
        {
            if (response.IsSuccessStatusCode) return;

            var statusCode = (int)response.StatusCode;
            var body = await ReadBodySafeAsync(response);

            throw LogAndBuild(
                $"Request {request.Method} {request.RequestUri} failed with status code {statusCode}. Response body: {body}",
                ApiFailureReason.HttpError, request, null, statusCode, body);
        }

        public static async Task<string> ReadContentAsStringAsync(HttpResponseMessage response, HttpRequestMessage request)
        {
            try
            {
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw LogAndBuild(
                    $"Failed to read the response content for {request.Method} {request.RequestUri}: {ex.Message}",
                    ApiFailureReason.Unknown, request, ex);
            }
        }

        public static async Task<byte[]> ReadContentAsByteArrayAsync(HttpResponseMessage response, HttpRequestMessage request)
        {
            try
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                throw LogAndBuild(
                    $"Failed to read the response content for {request.Method} {request.RequestUri}: {ex.Message}",
                    ApiFailureReason.Unknown, request, ex);
            }
        }

        public static ApiException LogAndBuild(
            string message,
            ApiFailureReason reason,
            HttpRequestMessage request,
            Exception? innerException = null,
            int? statusCode = null,
            string? responseBody = null)
        {
            Log.ForContext("Exception", innerException, destructureObjects: false)
               .ForContext("Method", request.Method)
               .ForContext("Url", request.RequestUri)
               .ForContext("StatusCode", statusCode)
               .ForContext("Reason", reason)
               .Error(message);

            return new ApiException(message, reason, innerException, statusCode, responseBody, request.Method, request.RequestUri);
        }

        /// <summary>Distingue una cancelación pedida por el consumidor de un timeout real:
        /// si el CancellationToken recibido ya fue cancelado, la cancelación la pidió el
        /// llamador; si no, se agotó el HttpClient.Timeout. En ambos casos el motivo es Timeout.</summary>
        private static string DescribeCancellation(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var causa = cancellationToken.IsCancellationRequested
                ? "was cancelled by the caller"
                : "timed out";

            return $"Request {request.Method} {request.RequestUri} {causa}";
        }

        private static async Task<string?> ReadBodySafeAsync(HttpResponseMessage response)
        {
            try
            {
                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
        }
    }

    internal static class HttpRequestMessageExtensions
    {
        /// <summary>Une la URI base, el endpoint y la query string en un solo Uri.
        /// Los parámetros se escapan con Uri.EscapeDataString y se unen con "&amp;" si el
        /// endpoint ya trae "?".</summary>
        public static Uri BuildUri(string uri, string endpoint, IReadOnlyDictionary<string, string>? query)
        {
            var baseUri = $"{uri}{endpoint ?? String.Empty}";

            if (query == null || query.Count == 0)
                return new Uri(baseUri);

            var separador = baseUri.Contains('?') ? "&" : "?";
            var queryString = string.Join("&", query.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));

            return new Uri($"{baseUri}{separador}{queryString}");
        }
    }
}