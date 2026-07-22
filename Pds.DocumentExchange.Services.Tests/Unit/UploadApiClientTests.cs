using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Pds.Core.ApiClient.Exceptions;
using Pds.Core.Logging;
using Pds.DocumentExchange.Services.Configuration;
using Pds.DocumentExchange.Services.Implementations;
using Pds.DocumentExchange.Services.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Tests.Unit
{
    [TestClass]
    public class UploadApiClientTests : BaseApiClientTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        private Mock<ILoggerAdapter<UploadApiClient>> _mockLogger;
        private UploadApiClient _apiClient;

        [TestMethod, TestCategory("Unit")]
        public async Task UploadDocument_ReturnsSuccess()
        {
            // Arrange
            Setup((HttpStatusCode)StatusCodes.Status200OK);

            // Act
            await _apiClient.UploadDocument(new UploadDocumentRequest());

            // Assert
            _mockHttpMessageHandler.Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(r =>
                        r.RequestUri.Equals("https://DummyUrl/api/upload/document")),
                    ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod, TestCategory("Unit")]
        public async Task UploadDocument_ReturnsHttpRequestException()
        {
            // Arrange
            Setup((HttpStatusCode)StatusCodes.Status500InternalServerError);

            // Act
            Func<Task> act = async () => await _apiClient.UploadDocument(new UploadDocumentRequest());

            // Assert
            await act.Should()
                .ThrowAsync<ApiGeneralException>()
                .WithMessage("Request failed with status code 500 InternalServerError*");
        }

        private void Setup(HttpStatusCode statusCode)
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode
                }).Verifiable();

            var client = new HttpClient(_mockHttpMessageHandler.Object);
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var configurationOptions = Options.Create(
                new DocumentExchangeServicesConfiguration
                {
                    DataApiClient = new DataApiClientConfiguration { ApiBaseAddress = "https://DummyUrl" }
                });

            _mockLogger = new Mock<ILoggerAdapter<UploadApiClient>>();

            _apiClient = new UploadApiClient(GetMockAuthenticationService<DataApiClientConfiguration>().Object, client, configurationOptions, _mockLogger.Object);
        }
    }
}