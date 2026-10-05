using System;
using System.Net.Http;
using NugetPackage_Rest.Exceptions;
using Xunit;

namespace NugetPackage_Rest.Tests
{
    public class ApiExceptionTests
    {
        [Fact]
        public void Constructor_SetsAllProperties()
        {
            var inner = new InvalidOperationException("boom");
            var uri = new Uri("https://api.example.com/orders");

            var exception = new ApiException(
                "Request POST https://api.example.com/orders failed with status code 422. Response body: {\"error\":\"invalid\"}",
                ApiFailureReason.HttpError,
                inner,
                422,
                "{\"error\":\"invalid\"}",
                HttpMethod.Post,
                uri);

            Assert.Equal(ApiFailureReason.HttpError, exception.Reason);
            Assert.Equal(422, exception.StatusCode);
            Assert.Equal("{\"error\":\"invalid\"}", exception.ResponseBody);
            Assert.Equal(HttpMethod.Post, exception.RequestMethod);
            Assert.Equal(uri, exception.RequestUri);
            Assert.Same(inner, exception.InnerException);
            Assert.Contains("422", exception.Message);
        }

        [Fact]
        public void Constructor_AllowsOptionalParametersToBeOmitted()
        {
            var exception = new ApiException("Request GET https://api.example.com/orders timed out", ApiFailureReason.Timeout);

            Assert.Equal(ApiFailureReason.Timeout, exception.Reason);
            Assert.Null(exception.StatusCode);
            Assert.Null(exception.ResponseBody);
            Assert.Null(exception.RequestMethod);
            Assert.Null(exception.RequestUri);
            Assert.Null(exception.InnerException);
        }
    }
}