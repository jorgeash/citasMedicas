using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Configs;
using NugetPackage_Rest.Exceptions;
using NugetPackage_Rest.Tests.Fakes;
using NugetPackage_Rest.Tests.Models;
using Microsoft.Extensions.Options;
using Xunit;

namespace NugetPackage_Rest.Tests
{
    public class RestBuilderPostTests
    {
        private static RestBuilder CreateRestBuilder(HttpClient client)
        {
            return new RestBuilder(client, Options.Create(new RequestSettings { EnableRequestLogs = false }));
        }

        [Fact]
        public async Task Post_WithSuccessResponse_ReturnsContentAsString()
        {
            var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("created")
            }));
            var rest = CreateRestBuilder(client);

            var result = await rest.Post.WithoutAuth().WithUri("https://api.example.com", "/orders")
                .WithBody(new { Name = "widget" })
                .GetContentAsStringAsync();

            Assert.Equal("created", result);
        }

        [Fact]
        public async Task Post_WithUnprocessableEntityStatus_ThrowsApiExceptionWithHttpErrorReasonAndBody()
        {
            var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage((HttpStatusCode)422)
            {
                Content = new StringContent("{\"error\":\"name is required\"}")
            }));
            var rest = CreateRestBuilder(client);

            var exception = await Assert.ThrowsAsync<ApiException>(
                () => rest.Post.WithoutAuth().WithUri("https://api.example.com", "/orders")
                    .WithBody(new { Name = "" })
                    .GetContentAsStringAsync());

            Assert.Equal(ApiFailureReason.HttpError, exception.Reason);
            Assert.Equal(422, exception.StatusCode);
            Assert.Equal("{\"error\":\"name is required\"}", exception.ResponseBody);
        }

        [Fact]
        public async Task Post_WithValidJsonResponse_DeserializesIntoTargetType()
        {
            var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\":7,\"name\":\"widget\"}")
            }));
            var rest = CreateRestBuilder(client);

            var result = await rest.Post.WithoutAuth().WithUri("https://api.example.com", "/orders")
                .WithBody(new { Name = "widget" })
                .DeserializeWithAsync<TestOrder>();

            Assert.Equal(7, result.Id);
            Assert.Equal("widget", result.Name);
        }

        [Fact]
        public async Task Post_WithoutBody_SendsRequestSuccessfully()
        {
            var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("ok")
            }));
            var rest = CreateRestBuilder(client);

            var result = await rest.Post.WithoutAuth().WithUri("https://api.example.com", "/orders/sync")
                .WithoutBody()
                .GetContentAsStringAsync();

            Assert.Equal("ok", result);
        }

        [Fact]
        public async Task Post_WithFormUrlEncoded_SendsUrlEncodedContentType()
        {
            HttpRequestMessage? capturedRequest = null;
            var client = new HttpClient(new FakeHttpMessageHandler(req =>
            {
                capturedRequest = req;
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") };
            }));
            var rest = CreateRestBuilder(client);

            await rest.Post.WithoutAuth().WithUri("https://api.example.com", "/auth")
                .WithFormUrlEncoded(new Dictionary<string, string> { ["user"] = "usuario01", ["pwd"] = "secret" })
                .GetContentAsStringAsync();

            Assert.NotNull(capturedRequest);
            Assert.Equal("application/x-www-form-urlencoded", capturedRequest!.Content!.Headers.ContentType!.MediaType);
            var body = await capturedRequest.Content.ReadAsStringAsync();
            Assert.Equal("user=usuario01&pwd=secret", body);
        }

        [Fact]
        public async Task Post_WithFormData_SendsMultipartConElArchivo()
        {
            HttpRequestMessage? capturedRequest = null;
            string? body = null;
            var client = new HttpClient(new FakeHttpMessageHandler(req =>
            {
                capturedRequest = req;
                body = req.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") };
            }));
            var rest = CreateRestBuilder(client);

            using var form = new MultipartFormDataContent();
            form.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("contenido-del-archivo")), "archivo", "producto.png");
            form.Add(new StringContent("producto-99"), "productoId");

            await rest.Post.WithoutAuth().WithUri("https://api.example.com", "/api/Productos/99/imagen")
                .WithFormData(form)
                .GetContentAsStringAsync();

            Assert.NotNull(capturedRequest);
            Assert.Equal("multipart/form-data", capturedRequest!.Content!.Headers.ContentType!.MediaType);
            Assert.Contains("name=archivo", body);
            Assert.Contains("filename=producto.png", body);
            Assert.Contains("contenido-del-archivo", body);
            Assert.Contains("name=productoId", body);
            Assert.Contains("producto-99", body);
        }
    }
}