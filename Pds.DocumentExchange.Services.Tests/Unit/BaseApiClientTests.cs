using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Pds.Core.ApiClient;
using Pds.Core.ApiClient.Interfaces;
using Pds.DocumentExchange.Services.Configuration;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Models.Filters;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Tests.Unit
{
    public abstract class BaseApiClientTests
    {
        protected const string TestBaseAddress = "http://test-api-endpoint/";

        protected const string TestFakeAccessToken = "AccessToken";

        private readonly HttpMessageHandler _httpMessageHandler = Mock.Of<HttpMessageHandler>();

        protected HttpClient GetHttpClient()
            => new HttpClient(_httpMessageHandler);

        protected DocumentExchangeServicesConfiguration GetServicesConfiguration()
            => new DocumentExchangeServicesConfiguration
            {
                DataApiClient = new DataApiClientConfiguration
                {
                    ApiBaseAddress = TestBaseAddress,
                    Timeout = 180
                }
            };

        protected Mock<IAuthenticationService<T>> GetMockAuthenticationService<T>()
            where T : BaseApiClientConfiguration
        {
            var mockAuthenticationService = new Mock<IAuthenticationService<T>>(MockBehavior.Strict);
            mockAuthenticationService.Setup(x => x.GetAccessTokenForAAD()).Returns(Task.FromResult(TestFakeAccessToken));
            return mockAuthenticationService;
        }

        protected void SetupMessageHandler(HttpStatusCode statusCode, object responseObject = null)
        {
            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = statusCode
            };

            if (responseObject != null)
            {
                var responseContent = JsonConvert.SerializeObject(responseObject);
                expectedResponse.Content = new StringContent(
                responseContent,
                System.Text.Encoding.UTF8,
                "application/json");
            }

            Mock.Get(_httpMessageHandler)
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(expectedResponse);
        }

        protected void VerifyMessageHandler(HttpMethod httpMethod, string expectedUri)
        {
            Mock.Get(_httpMessageHandler)
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(), // we expected a single external request
                    ItExpr.Is<HttpRequestMessage>(
                        req => req.Method.Equals(httpMethod)
                        && req.RequestUri.Equals(new Uri(expectedUri))),
                    ItExpr.IsAny<CancellationToken>());
        }

        protected IFilter[] GetTestFilters()
            => new[]
                {
                    new ListFilter
                    {
                        Title = "Test Filter",
                        Key = "testfilter",
                        Type = FilterType.ListFilter.ToString(),
                        Values = new[]
                        {
                            new FilterValue
                            {
                                Title = "Test Filter Value",
                                Value = "testfiltervalue",
                                Selected = false,
                                Count = 2
                            }
                        }
                    }
                };
    }
}