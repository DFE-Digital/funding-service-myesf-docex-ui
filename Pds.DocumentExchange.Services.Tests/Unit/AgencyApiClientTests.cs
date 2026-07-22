using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.ApiClient.Exceptions;
using Pds.Core.Common.Organisation.Enums;
using Pds.Core.Common.Organisation.Models;
using Pds.Core.Logging;
using Pds.DocumentExchange.Services.Configuration;
using Pds.DocumentExchange.Services.Implementations;
using Pds.DocumentExchange.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Tests.Unit
{
    [TestClass]
    [TestCategory("Unit")]
    public class AgencyApiClientTests : BaseApiClientTests
    {
        private const string TestTeamName = "Team1";
        private const string TestFileName = "File1.doc";

        private static readonly string DownloadTeamDocumentRoute
            = $"api/agency/teams/{TestTeamName}/documents/{TestFileName}";

        private static readonly string GetTeamSummaryRoute
            = $"api/agency/teams/{TestTeamName}/summary";

        private static readonly string ListTeamDocumentsRoute
            = $"api/agency/teams/{TestTeamName}/documents";

        private static readonly string PublishTeamDocumentsRoute
            = $"api/agency/teams/{TestTeamName}/documents/publish";

        private static readonly string RemoveTeamDocumentsRoute
            = $"api/agency/teams/{TestTeamName}/documents/remove";

        private static readonly string GetPreviousDocumentReferencesRoute
            = $"api/agency/teams/{TestTeamName}/documents/previous-document-references";

        private readonly ILoggerAdapter<AgencyApiClient> _logger
            = Mock.Of<ILoggerAdapter<AgencyApiClient>>();

        [TestMethod]
        [DataRow("")]
        [DataRow("TEST STRING")]
        [DataRow("string 1")]
        [DataRow("another random string")]
        public async Task DownloadTeamDocument_OnSuccess_ReturnsTheByteArray(string data)
        {
            // Arrange
            var expectedBytes = Encoding.ASCII.GetBytes(data);

            SetupMessageHandler(HttpStatusCode.OK, expectedBytes);

            var client = GetAgencyApiClient();

            // Act
            var result = await client.DownloadTeamDocument(TestTeamName, TestFileName);

            // Assert
            result.Should().BeEquivalentTo(expectedBytes);
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + DownloadTeamDocumentRoute);
        }

        [TestMethod]
        public void DownloadTeamDocument_OnFailure_LogsTheException()
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetAgencyApiClient();

            // Act
            Func<Task> act = async () => await client.DownloadTeamDocument(TestTeamName, TestFileName);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + DownloadTeamDocumentRoute);

            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(10, 1)]
        [DataRow(100, 1)]
        [DataRow(1, 10)]
        [DataRow(10, 10)]
        [DataRow(100, 10)]
        [DataRow(1, 100)]
        [DataRow(10, 100)]
        [DataRow(100, 100)]
        public async Task GetTeamSummary_OnSuccess_ReturnsTheSummary(int invalidCount, int validCount)
        {
            // Arrange
            var expectedSummary = new FileShareSummary
            {
                InvalidCount = invalidCount,
                ValidCount = validCount
            };

            SetupMessageHandler(HttpStatusCode.OK, expectedSummary);

            var client = GetAgencyApiClient();

            // Act
            var result = await client.GetTeamSummary(TestTeamName);

            // Assert
            result.Should().BeEquivalentTo(expectedSummary);
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + GetTeamSummaryRoute);
        }

        [TestMethod]
        public void GetTeamSummary_OnFailure_LogsTheException()
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetAgencyApiClient();

            // Act
            Func<Task> act = async () => await client.GetTeamSummary(TestTeamName);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + GetTeamSummaryRoute);

            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public async Task ListTeamDocuments_OnSuccess_ReturnsTheSummary(int numberOfDocuments)
        {
            // Arrange
            var expectedResult = new ListResult<AgencyDocument>
            {
                Filters = GetTestFilters(),
                Items = numberOfDocuments == 0
                    ? Enumerable.Empty<AgencyDocument>()
                    : Enumerable.Range(1, numberOfDocuments)
                        .Select(
                            d => new AgencyDocument
                            {
                                FileName = $"file {d}",
                                IsValid = true,
                                Product = new Product
                                {
                                    Identifier = d,
                                    Name = $"product {d}",
                                    AgencyTeams = new[] { TestTeamName }
                                },
                                OrganisationInfo = new OrganisationInfo
                                {
                                    Name = $"school {d}",
                                    OrganisationIdentifier = new OrganisationIdentifier
                                    {
                                        Value = (10000000 + d).ToString(),
                                        Type = OrganisationIdentifierType.Ukprn
                                    }
                                }
                            })
            };

            SetupMessageHandler(HttpStatusCode.OK, expectedResult);

            var client = GetAgencyApiClient();

            // Act
            var result = await client.ListTeamDocuments(TestTeamName, null);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + ListTeamDocumentsRoute);
        }

        [TestMethod]
        public void ListTeamDocuments_OnFailure_LogsTheException()
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetAgencyApiClient();

            // Act
            Func<Task> act = async () => await client.ListTeamDocuments(TestTeamName, null);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + ListTeamDocumentsRoute);

            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [DataRow(1001, "product 1", 123)]
        [DataRow(1002, "product 2", 1234)]
        [DataRow(1003, "product 3", 12345)]
        public async Task PublishTeamDocuments_OnSuccess_ReturnsTheSummary(
            int productId,
            string productName,
            int count)
        {
            // Arrange
            var expectedResult = new KeyValuePair<Product, int>(
                new Product
                {
                    Identifier = productId,
                    Name = productName
                },
                count);

            SetupMessageHandler(HttpStatusCode.OK, expectedResult);

            var client = GetAgencyApiClient();

            // Act
            var result = await client.PublishTeamDocuments(TestTeamName, null);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + PublishTeamDocumentsRoute);
        }

        [TestMethod]
        public void PublishTeamDocuments_OnFailure_LogsTheException()
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetAgencyApiClient();

            // Act
            Func<Task> act = async () => await client.PublishTeamDocuments(TestTeamName, null);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + PublishTeamDocumentsRoute);

            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void RemoveTeamDocuments_OnSuccess_DoesNotThrow()
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.OK);

            var client = GetAgencyApiClient();

            // Act
            Func<Task> act = async () => await client.RemoveTeamDocuments(TestTeamName, null);

            // Assert
            act.Should().NotThrowAsync();
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + RemoveTeamDocumentsRoute);
        }

        [TestMethod]
        public void RemoveTeamDocuments_OnFailure_LogsTheException()
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetAgencyApiClient();

            // Act
            Func<Task> act = async () => await client.RemoveTeamDocuments(TestTeamName, null);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + RemoveTeamDocumentsRoute);

            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void GetPreviousDocumentReferences_OnSuccess_DoesNotThrow()
        {
            // Arrange
            var fakeDocumentReferences = new List<DocumentReference>
            {
                new DocumentReference { BatchIdentifier = "fake", FileName = "fake", ParentBatchIdentifier = "fake" }
            };

            SetupMessageHandler(HttpStatusCode.OK, fakeDocumentReferences);

            var client = GetAgencyApiClient();

            // Act
            Func<Task> act = async () => await client.GetPreviousDocumentVersionReferences(TestTeamName, fakeDocumentReferences);

            // Assert
            act.Should().NotThrowAsync();
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + GetPreviousDocumentReferencesRoute);
        }

        private AgencyApiClient GetAgencyApiClient()
        {
            return new AgencyApiClient(
                GetMockAuthenticationService<DataApiClientConfiguration>().Object,
                GetHttpClient(),
                Options.Create(GetServicesConfiguration()),
                _logger);
        }
    }
}