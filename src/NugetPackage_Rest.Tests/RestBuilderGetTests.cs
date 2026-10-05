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
    public class RestBuilderGetTests
    {
        private static RestBuilder CreateRestBuilder(HttpClient client)
        {
            return new RestBuilder(client, Options.Create(new RequestSettings { EnableRequestLogs = false }));
        }

        [Fact]
        public async Task Get_WithSuccessResponse_ReturnsContentAsString()
        {
            var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("hello world")
            }));

            var rest = CreateRestBuilder(client);

            var result = await rest.Get.WithoutAuth().WithUri("https://api.example.com", "/orders").GetContentAsStringAsync();

            Assert.Equal("hello world", result);
        }

        [Fact]
        public async Task Get_WithErrorStatusCode_ThrowsApiExceptionWithHttpErrorReason()
        {
            var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("{\"error\":\"not found\"}")
            }));
            var rest = CreateRestBuilder(client);

            var exception = await Assert.ThrowsAsync<ApiException>(
                () => rest.Get.WithoutAuth().WithUri("https://api.example.com", "/orders/42").GetContentAsStringAsync());

            Assert.Equal(ApiFailureReason.HttpError, exception.Reason);
            Assert.Equal(404, exception.StatusCode);
            Assert.Equal("{\"error\":\"not found\"}", exception.ResponseBody);
        }

        [Fact]
        public async Task Get_WithValidJson_DeserializesIntoTargetType()
        {
            var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\":42,\"name\":\"widget\"}")
            }));
            var rest = CreateRestBuilder(client);

            var result = await rest.Get.WithoutAuth().WithUri("https://api.example.com", "/orders/42").DeserializeWithAsync<TestOrder>();

            Assert.Equal(42, result.Id);
            Assert.Equal("widget", result.Name);
        }

        [Fact]
        public async Task Get_WithInvalidJson_ThrowsApiExceptionWithDeserializationReason()
        {
            var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("not-json")
            }));
            var rest = CreateRestBuilder(client);

            var exception = await Assert.ThrowsAsync<ApiException>(
                () => rest.Get.WithoutAuth().WithUri("https://api.example.com", "/orders/42").DeserializeWithAsync<TestOrder>());

            Assert.Equal(ApiFailureReason.Deserialization, exception.Reason);
            Assert.Equal("not-json", exception.ResponseBody);
            Assert.NotNull(exception.InnerException);
        }

        [Fact]
        public async Task Get_WithSuccessResponse_ReturnsContentAsByteArray()
        {
            var expectedBytes = Encoding.UTF8.GetBytes("binary-data");
            var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(expectedBytes)
            }));
            var rest = CreateRestBuilder(client);

            var result = await rest.Get.WithoutAuth().WithUri("https://api.example.com", "/orders/42/file").GetContentAsByteArrayAsync();

            Assert.Equal(expectedBytes, result);
        }
    }
}