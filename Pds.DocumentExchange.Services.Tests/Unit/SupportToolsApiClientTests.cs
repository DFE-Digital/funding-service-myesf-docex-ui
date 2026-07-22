using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.ApiClient.Exceptions;
using Pds.Core.Logging;
using Pds.DocumentExchange.Services.Configuration;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Implementations;
using Pds.DocumentExchange.Services.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Tests.Unit
{
    [TestClass]
    [TestCategory("Unit")]
    public class SupportToolsApiClientTests : BaseApiClientTests
    {
        private static readonly string GetDocumentsPublishedByESFARoute
            = "api/support-tools/documents-published-by-esfa?pageNumber={0}&pageSize={1}";

        private static readonly string DownloadDocumentsPublishedCsvRoute
            = "api/support-tools/documents-published-csv/{0}";

        private static readonly string DownloadNotificationRecipientsCsvRoute
            = "api/support-tools/notification-recipients-csv/{0}";

        private static readonly string DeleteDocumentsSearch
            = "api/support-tools/delete-documents-search?direction={0}&ukprn={1}&fileType={2}&year={3}";

        private static readonly string DownloadMIReport
            = "api/support-tools/download-mireport?from={0}&to={1}";

        private readonly ILoggerAdapter<SupportToolsApiClient> _logger
            = Mock.Of<ILoggerAdapter<SupportToolsApiClient>>();

        [TestMethod]
        [DataRow(1, 25)]
        [DataRow(5, 25)]
        [DataRow(99, 5)]
        public async Task GetDocumentsPublishedByDfE_OnSuccess_Returns(int pageNumber, int pageSize)
        {
            // Arrange
            var publishedBatches = new[]
            {
                new PublishedBatch
                {
                    DateAndTime = new DateTime(2020, 1, 1),
                    ParentBatchIdentifier = Guid.Parse("78cface4-e78f-4c31-969f-6f6b2a7d49e1"),
                    EmailAddress = "email.address@education.co.uk",
                    NumberOfDocuments = 1000,
                    NumberOfEmails = 250
                }
            };

            var expected = new ListResult<PublishedBatch>
            {
                Items = publishedBatches,
                TotalItems = publishedBatches.Length,
                TotalPages = 1
            };

            SetupMessageHandler(HttpStatusCode.OK, expected);

            var client = GetSupportToolsApiClient();

            // Act
            var result = await client.GetDocumentsPublishedByDfE(pageNumber, pageSize);

            // Assert
            result.Should().BeEquivalentTo(expected);
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + string.Format(GetDocumentsPublishedByESFARoute, pageNumber, pageSize));
        }

        [TestMethod]
        [DataRow(1, 25)]
        [DataRow(5, 25)]
        [DataRow(99, 5)]
        public void GetDocumentsPublishedByDfE_OnFailure_LogsTheException(int pageNumber, int pageSize)
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetSupportToolsApiClient();

            // Act
            Func<Task> act = async () => await client.GetDocumentsPublishedByDfE(pageNumber, pageSize);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + string.Format(GetDocumentsPublishedByESFARoute, pageNumber, pageSize));

            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task DownloadDocumentsPublishedCsv_OnSuccess_Returns()
        {
            // Arrange
            var parentBatchId = Guid.Parse("2015ecf7-e2b3-4e1a-ab33-1d50750baf84");

            var expected = new byte[] { 1, 2, 3, 4, 5 };
            SetupMessageHandler(HttpStatusCode.OK, expected);

            var client = GetSupportToolsApiClient();

            // Act
            var result = await client.DownloadDocumentsPublishedCsv(parentBatchId);

            // Assert
            result.Should().BeEquivalentTo(expected);
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + string.Format(DownloadDocumentsPublishedCsvRoute, parentBatchId));
        }

        [TestMethod]
        public void DownloadDocumentsPublishedCsv_OnFailure_LogsTheException()
        {
            // Arrange
            var parentBatchId = Guid.Parse("2015ecf7-e2b3-4e1a-ab33-1d50750baf84");

            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetSupportToolsApiClient();

            // Act
            Func<Task> act = async () => await client.DownloadDocumentsPublishedCsv(parentBatchId);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + string.Format(DownloadDocumentsPublishedCsvRoute, parentBatchId));

            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task DownloadNotificationRecipientsCsv_OnSuccess_Returns()
        {
            // Arrange
            var parentBatchId = Guid.Parse("2015ecf7-e2b3-4e1a-ab33-1d50750baf84");

            var expected = new byte[] { 1, 2, 3, 4, 5 };
            SetupMessageHandler(HttpStatusCode.OK, expected);

            var client = GetSupportToolsApiClient();

            // Act
            var result = await client.DownloadNotificationRecipientsCsv(parentBatchId);

            // Assert
            result.Should().BeEquivalentTo(expected);
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + string.Format(DownloadNotificationRecipientsCsvRoute, parentBatchId));
        }

        [TestMethod]
        public void DownloadNotificationRecipientsCsv_OnFailure_LogsTheException()
        {
            // Arrange
            var parentBatchId = Guid.Parse("2015ecf7-e2b3-4e1a-ab33-1d50750baf84");

            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetSupportToolsApiClient();

            // Act
            Func<Task> act = async () => await client.DownloadNotificationRecipientsCsv(parentBatchId);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + string.Format(DownloadNotificationRecipientsCsvRoute, parentBatchId));

            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void DeleteDocuments_OnFailure_LogsTheException()
        {
            // Arrange
            ExchangeDocumentDirection direction = ExchangeDocumentDirection.PublishedByAgency;
            int ukprn = 12345678;
            string fileType = "filetype1";
            string year = "202021";

            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetSupportToolsApiClient();

            // Act
            Func<Task> act = async () => await client.DeleteDocumentsSearch(direction, ukprn, fileType, year);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + string.Format(DeleteDocumentsSearch, direction, ukprn, fileType, year));

            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteDocuments_OnSuccess_Returns()
        {
            // Arrange
            ExchangeDocumentDirection direction = ExchangeDocumentDirection.PublishedByAgency;
            int ukprn = 12345678;
            string fileType = "filetype1";
            string year = "202021";

            var expected = new ListResult<ExchangeDocument>();
            SetupMessageHandler(HttpStatusCode.OK, expected);

            var client = GetSupportToolsApiClient();

            // Act
            var result = await client.DeleteDocumentsSearch(direction, ukprn, fileType, year);

            // Assert
            result.Should().BeEquivalentTo(expected);
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + string.Format(DeleteDocumentsSearch, direction, ukprn, fileType, year));
        }

        [TestMethod]
        public async Task DownloadMIReport_OnSuccess_Returns()
        {
            // Arrange
            var to = DateTime.Now;
            var from = to.AddDays(-10);

            var expected = new byte[] { 1, 2, 3, 4, 5 };
            SetupMessageHandler(HttpStatusCode.OK, expected);

            var client = GetSupportToolsApiClient();

            // Act
            var result = await client.DownloadMIReport(from, to);

            // Assert
            result.Should().BeEquivalentTo(expected);
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + string.Format(DownloadMIReport, from.ToString("yyyy-MM-dd"), to.ToString("yyyy-MM-dd")));
        }

        [TestMethod]
        public void DownloadMIReport_OnFailure_LogsTheException()
        {
            // Arrange
            var to = DateTime.Now;
            var from = to.AddDays(-10);

            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetSupportToolsApiClient();

            // Act
            Func<Task> act = async () => await client.DownloadMIReport(from, to);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + string.Format(DownloadMIReport, from.ToString("yyyy-MM-dd"), to.ToString("yyyy-MM-dd")));

            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        private SupportToolsApiClient GetSupportToolsApiClient()
        {
            return new SupportToolsApiClient(
                GetMockAuthenticationService<DataApiClientConfiguration>().Object,
                GetHttpClient(),
                Options.Create(GetServicesConfiguration()),
                _logger);
        }
    }
}