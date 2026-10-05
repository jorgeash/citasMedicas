using NugetPackage_Rest.Configs;
using NugetPackage_Rest.Interfaces.IRequests;
using NugetPackage_Rest.Interfaces.IServices;
using Microsoft.Extensions.Options;

namespace NugetPackage_Rest.Builders
{
    /// <summary>Implementación pública de IRest. Cada propiedad crea un builder NUEVO,
    /// porque un HttpRequestMessage solo puede enviarse una vez.</summary>
    public class RestBuilder : IRest
    {
        private readonly HttpClient _httpClient;
        private readonly RequestSettings _requestSettings;

        public RestBuilder(HttpClient httpClient, IOptions<RequestSettings> options)
        {
            _httpClient = httpClient;
            _requestSettings = options.Value;
        }

        public INotContentRequest Get => new NotContentRequestBuilder(_httpClient, HttpMethod.Get, _requestSettings);

        public IWithContentRequest Post => new WithContentRequestBuilder(_httpClient, HttpMethod.Post, _requestSettings);

        public IWithContentRequest Put => new WithContentRequestBuilder(_httpClient, HttpMethod.Put, _requestSettings);

        public IWithContentRequest Delete => new WithContentRequestBuilder(_httpClient, HttpMethod.Delete, _requestSettings);

        public IWithContentRequest Patch => new WithContentRequestBuilder(_httpClient, HttpMethod.Patch, _requestSettings);
    }
}