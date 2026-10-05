using NugetPackage_Rest.Interfaces.IServices;
using System.Text;

namespace NugetPackage_Rest.Extensions
{
    /// <summary>Helpers que llenan el HttpRequestMessage (headers y los 3 tipos de body).</summary>
    public static class RequestExtensions
    {
        public static void AddHeaders(this HttpRequestMessage request, IDictionary<string, string> headers)
        {
            if (headers == null) return;
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        /// <summary>Serializa con Newtonsoft.Json. Se conserva por compatibilidad con versiones
        /// anteriores del paquete; los builders usan la sobrecarga con IJsonSerializer.</summary>
        public static void AddContent(this HttpRequestMessage request, object body)
        {
            request.AddContent(body, Serializers.NewtonsoftJsonSerializer.Instance);
        }

        /// <summary>Serializa el body con el serializador elegido en RequestSettings y fija
        /// application/json; charset=utf-8.</summary>
        public static void AddContent(this HttpRequestMessage request, object? body, IJsonSerializer serializer)
        {
            if (body == null) return;
            var jsonContent = serializer.Serialize(body);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        }

        public static void AddFormDataContent(this HttpRequestMessage request, MultipartFormDataContent content)
        {
            if (content == null) return;
            request.Content = content;
        }

        public static void AddFormUrlEncodedContent(this HttpRequestMessage request, IDictionary<string, string> data)
        {
            if (data == null) return;
            request.Content = new FormUrlEncodedContent(data);
        }
    }
}