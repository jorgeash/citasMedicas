using NugetPackage_Rest.Exceptions;
using Serilog;

namespace NugetPackage_Rest.Builders
{
    /// <summary>Centraliza el envío y el manejo de errores: todo fallo se convierte
    /// en ApiException con su ApiFailureReason y queda registrado en Serilog.</summary>
    internal static class HttpRequestExecutor
    {
        public static async Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage request)
        {
            try
            {
                return await client.SendAsync(request);
            }
            catch (HttpRequestException ex)
            {
                throw LogAndBuild(
                    $"Request {request.Method} {request.RequestUri} failed due to a network error: {ex.Message}",
                    ApiFailureReason.Network, request, ex);
            }
            catch (TaskCanceledException ex)
            {
                throw LogAndBuild(
                    $"Request {request.Method} {request.RequestUri} timed out",
                    ApiFailureReason.Timeout, request, ex);
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
}
