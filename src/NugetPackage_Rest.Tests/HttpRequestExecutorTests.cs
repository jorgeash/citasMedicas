using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Exceptions;
using NugetPackage_Rest.Tests.Fakes;
using Xunit;

namespace NugetPackage_Rest.Tests
{
    public class HttpRequestExecutorTests
    {
        [Fact]
        public async Task SendAsync_WhenHandlerThrowsHttpRequestException_ThrowsApiExceptionWithNetworkReason()
        {
            var networkError = new HttpRequestException("Connection refused");
            var client = new HttpClient(new FakeHttpMessageHandler(networkError));
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/orders");

            var exception = await Assert.ThrowsAsync<ApiException>(
                () => HttpRequestExecutor.SendAsync(client, request));

            Assert.Equal(ApiFailureReason.Network, exception.Reason);
            Assert.Same(networkError, exception.InnerException);
        }

        [Fact]
        public async Task SendAsync_WhenHandlerThrowsTaskCanceledException_ThrowsApiExceptionWithTimeoutReason()
        {
            var timeoutError = new TaskCanceledException("Timed out");
            var client = new HttpClient(new FakeHttpMessageHandler(timeoutError));
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/orders");

            var exception = await Assert.ThrowsAsync<ApiException>(
                () => HttpRequestExecutor.SendAsync(client, request));

            Assert.Equal(ApiFailureReason.Timeout, exception.Reason);
            Assert.Same(timeoutError, exception.InnerException);
        }

        [Fact]
        public async Task SendAsync_WhenHandlerReturnsResponse_ReturnsSameResponse()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var client = new HttpClient(new FakeHttpMessageHandler(_ => response));
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/orders");

            var result = await HttpRequestExecutor.SendAsync(client, request);

            Assert.Same(response, result);
        }

        [Fact]
        public async Task EnsureSuccessAsync_WhenStatusIsError_ThrowsApiExceptionWithStatusCodeAndBody()
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("{\"error\":\"not found\"}")
            };
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/orders/42");

            var exception = await Assert.ThrowsAsync<ApiException>(
                () => HttpRequestExecutor.EnsureSuccessAsync(response, request));

            Assert.Equal(ApiFailureReason.HttpError, exception.Reason);
            Assert.Equal(404, exception.StatusCode);
            Assert.Equal("{\"error\":\"not found\"}", exception.ResponseBody);
        }

        [Fact]
        public async Task EnsureSuccessAsync_WhenStatusIsSuccess_DoesNotThrow()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("ok")
            };
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/orders");

            await HttpRequestExecutor.EnsureSuccessAsync(response, request);
        }
    }
}