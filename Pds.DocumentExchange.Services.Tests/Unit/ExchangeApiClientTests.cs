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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Tests.Unit
{
    [TestClass]
    public class ExchangeApiClientTests : BaseApiClientTests
    {
        private const string TestTeamName = "Team1";
        private const bool IsAdvancedUser = false;
        private const string TestFileName = "File1.doc";
        private const int TestProductId = 10001;

        private const string DownloadDocumentsRoute
            = "api/exchange/documents/download";

        private const string DeleteDocumentsRoute
            = "api/exchange/documents/delete";

        private const string DeleteDocumentRoute
           = "api/exchange/documents/delete-single";

        private const string GetOrganisationDocumentsRoute
            = "api/exchange/documents";

        private const string GetOrganisationUserSummaryRoute
            = "api/exchange/summary";

        private const string MIReportRoute
            = "api/exchange/mi-report";

        private static readonly string DownloadAgencyTeamDocumentsRoute
            = $"api/exchange/teams/{TestTeamName}/documents/download";

        private static readonly string GetAgencyTeamDocumentsRoute
            = $"api/exchange/teams/{TestTeamName}/documents";

        private static readonly string GetAgencyTeamSummaryRoute
            = $"api/exchange/teams/{TestTeamName}/team-summary";

        private static readonly string GetCurrentProductVersionForOrganisationRoute
            = $"api/exchange/current-product-version-for-organisation?productIdentifier={TestProductId}";

        private readonly ILoggerAdapter<ExchangeApiClient> _logger
            = Mock.Of<ILoggerAdapter<ExchangeApiClient>>();

        [TestMethod, TestCategory("Unit")]
        [DataRow("")]
        [DataRow("TEST STRING")]
        [DataRow("string 1")]
        [DataRow("another random string")]
        public async Task DownloadDocuments_OnSuccess_ReturnsTheByteArray(string data)
        {
            // Arrange
            var expectedBytes = Encoding.ASCII.GetBytes(data);

            SetupMessageHandler(HttpStatusCode.OK, expectedBytes);

            var client = GetExchangeApiClient();

            // Act
            var result = await client.DownloadDocuments(null);

            // Assert
            result.Should().BeEquivalentTo(expectedBytes);
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + DownloadDocumentsRoute);
        }

        [TestMethod, TestCategory("Unit")]
        public void DownloadDocuments_OnFailure_LogsTheException()
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetExchangeApiClient();

            // Act
            Func<Task> act = async () => await client.DownloadDocuments(null);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + DownloadDocumentsRoute);
            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow("")]
        [DataRow("TEST STRING")]
        [DataRow("string 1")]
        [DataRow("another random string")]
        public async Task DownloadAgencyTeamDocuments_OnSuccess_ReturnsTheByteArray(string data)
        {
            // Arrange
            var expectedBytes = Encoding.ASCII.GetBytes(data);

            SetupMessageHandler(HttpStatusCode.OK, expectedBytes);

            var client = GetExchangeApiClient();

            // Act
            var result = await client.DownloadAgencyTeamDocuments(TestTeamName, null);

            // Assert
            result.Should().BeEquivalentTo(expectedBytes);
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + DownloadAgencyTeamDocumentsRoute);
        }

        [TestMethod, TestCategory("Unit")]
        public void DownloadAgencyTeamDocuments_OnFailure_LogsTheException()
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetExchangeApiClient();

            // Act
            Func<Task> act = async () => await client.DownloadAgencyTeamDocuments(TestTeamName, null);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + DownloadAgencyTeamDocumentsRoute);
            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public async Task GetAgencyTeamDocuments_OnSuccess_ReturnsTheDocuments(int numberOfDocuments)
        {
            // Arrange
            var expectedResult = new ListResult<ExchangeDocument>
            {
                Filters = GetTestFilters(),
                Items = numberOfDocuments == 0
                        ? Enumerable.Empty<ExchangeDocument>()
                        : Enumerable.Range(1, numberOfDocuments)
                                    .Select(d => new ExchangeDocument
                                    {
                                        AgencyTeam = TestTeamName,
                                        DocumentReference = new DocumentReference
                                        {
                                            BatchIdentifier = Guid.NewGuid().ToString(),
                                            ParentBatchIdentifier = Guid.NewGuid().ToString(),
                                            FileName = TestFileName
                                        },
                                        Product = new Product
                                        {
                                            Identifier = d,
                                            Name = $"product {d}",
                                            AgencyTeams = new[] { TestTeamName }
                                        },
                                        ExchangeDirection = Enums.ExchangeDocumentDirection.SentByOrganisation,
                                        Year = 201920,
                                        OrganisationInfo = new OrganisationInfo
                                        {
                                            Name = $"school {d}",
                                            OrganisationIdentifier = new OrganisationIdentifier
                                            {
                                                Value = (10000000 + d).ToString(),
                                                Type = OrganisationIdentifierType.Ukprn
                                            }
                                        },
                                        Version = d
                                    })
                                    .ToList()
            };

            SetupMessageHandler(HttpStatusCode.OK, expectedResult);

            var client = GetExchangeApiClient();

            // Act
            var result = await client.GetAgencyTeamDocuments(TestTeamName, null);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + GetAgencyTeamDocumentsRoute);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetAgencyTeamDocuments_OnFailure_LogsTheException()
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetExchangeApiClient();

            // Act
            Func<Task> act = async () => await client.GetAgencyTeamDocuments(TestTeamName, null);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + GetAgencyTeamDocumentsRoute);
            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public async Task GetAgencyTeamSummary_OnSuccess_ReturnsTheSummary(int numberOfDocuments)
        {
            // Arrange
            var expectedResult = new Summary
            {
                CountOfNewDocuments = numberOfDocuments,
                DocumentExchangeEnabled = true
            };

            SetupMessageHandler(HttpStatusCode.OK, expectedResult);

            var client = GetExchangeApiClient();

            // Act
            var result = await client.GetAgencyTeamSummary(TestTeamName);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + GetAgencyTeamSummaryRoute);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetAgencyTeamSummary_OnFailure_LogsTheException()
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetExchangeApiClient();

            // Act
            Func<Task> act = async () => await client.GetAgencyTeamSummary(TestTeamName);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + GetAgencyTeamSummaryRoute);
            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(1000)]
        public async Task GetCurrentProductVersionForOrganisation_OnSuccess_ReturnsTheSummary(int expectedResult)
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.OK, expectedResult);

            var client = GetExchangeApiClient();

            // Act
            var result = await client.GetCurrentProductVersionForOrganisation(null, TestProductId.ToString());

            // Assert
            result.Should().Be(expectedResult);
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + GetCurrentProductVersionForOrganisationRoute);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetCurrentProductVersionForOrganisation_OnFailure_LogsTheException()
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetExchangeApiClient();

            // Act
            Func<Task> act = async () => await client.GetCurrentProductVersionForOrganisation(null, TestProductId.ToString());

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + GetCurrentProductVersionForOrganisationRoute);
            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public async Task GetOrganisationDocuments_OnSuccess_ReturnsTheDocuments(int numberOfDocuments)
        {
            // Arrange
            var expectedResult = new ListResult<ExchangeDocument>
            {
                Filters = GetTestFilters(),
                Items = numberOfDocuments == 0
                        ? Enumerable.Empty<ExchangeDocument>()
                        : Enumerable.Range(1, numberOfDocuments)
                                    .Select(d => new ExchangeDocument
                                    {
                                        AgencyTeam = TestTeamName,
                                        DocumentReference = new DocumentReference
                                        {
                                            BatchIdentifier = Guid.NewGuid().ToString(),
                                            ParentBatchIdentifier = Guid.NewGuid().ToString(),
                                            FileName = TestFileName
                                        },
                                        Product = new Product
                                        {
                                            Identifier = d,
                                            Name = $"product {d}",
                                            AgencyTeams = new[] { TestTeamName }
                                        },
                                        ExchangeDirection = Enums.ExchangeDocumentDirection.SentByOrganisation,
                                        Year = 201920,
                                        OrganisationInfo = new OrganisationInfo
                                        {
                                            Name = $"school {d}",
                                            OrganisationIdentifier = new OrganisationIdentifier
                                            {
                                                Value = (10000000 + d).ToString(),
                                                Type = OrganisationIdentifierType.Ukprn
                                            }
                                        },
                                        Version = d
                                    })
                                    .ToList()
            };

            SetupMessageHandler(HttpStatusCode.OK, expectedResult);

            var client = GetExchangeApiClient();

            // Act
            var result = await client.GetOrganisationDocuments(null);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + GetOrganisationDocumentsRoute);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetOrganisationDocuments_OnFailure_LogsTheException()
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetExchangeApiClient();

            // Act
            Func<Task> act = async () => await client.GetOrganisationDocuments(null);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + GetOrganisationDocumentsRoute);
            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public async Task GetOrganisationUserSummary_OnSuccess_ReturnsTheSummary(int numberOfDocuments)
        {
            // Arrange
            var expectedResult = new Summary
            {
                CountOfNewDocuments = numberOfDocuments,
                DocumentExchangeEnabled = true
            };

            SetupMessageHandler(HttpStatusCode.OK, expectedResult);

            var client = GetExchangeApiClient();

            // Act
            var result = await client.GetOrganisationUserSummary(null);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + GetOrganisationUserSummaryRoute);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetOrganisationUserSummary_OnFailure_LogsTheException()
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetExchangeApiClient();

            // Act
            Func<Task> act = async () => await client.GetOrganisationUserSummary(null);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + GetOrganisationUserSummaryRoute);
            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public async Task DeleteDocuments_OnSuccess_ReturnsTheResult(int numberOfDocuments)
        {
            // Arrange
            var expectedResult = Enumerable.Range(0, numberOfDocuments).Select(id => new ExchangeDocument
            {
                DocumentReference = new DocumentReference
                {
                    FileName = $"file-{id}"
                }
            });

            SetupMessageHandler(HttpStatusCode.OK, expectedResult);

            var client = GetExchangeApiClient();

            // Act
            var result = await client.DeleteDocuments(null);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + DeleteDocumentsRoute);
        }

        [TestMethod, TestCategory("Unit")]
        public void DeleteDocuments_OnFailure_LogsTheException()
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetExchangeApiClient();

            // Act
            Func<Task> act = async () => await client.DeleteDocuments(null);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + DeleteDocumentsRoute);
            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }


        [TestMethod, TestCategory("Unit")]
        public async Task DeleteDocument_OnSuccess_ReturnsTheResult()
        {
            // Arrange
            var expectedResult = new ExchangeDocument
            {
                DocumentReference = new DocumentReference
                {
                    FileName = $"file1"
                }
            };

            SetupMessageHandler(HttpStatusCode.OK, expectedResult);

            var client = GetExchangeApiClient();

            // Act
            var result = await client.DeleteDocument(null);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + DeleteDocumentRoute);
        }


        [TestMethod, TestCategory("Unit")]
        public void DeleteDocument_OnFailure_LogsTheException()
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetExchangeApiClient();

            // Act
            Func<Task> act = async () => await client.DeleteDocument(null);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + DeleteDocumentRoute);
            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task MIReport_OnSuccess_ReturnsTheReportData()
        {
            // Arrange
            var expectedResult = new MIReportData();

            SetupMessageHandler(HttpStatusCode.OK, expectedResult);

            var client = GetExchangeApiClient();

            // Act
            var result = await client.MIReport(null);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + MIReportRoute);
        }

        [TestMethod, TestCategory("Unit")]
        public void MIReport_OnFailure_LogsTheException()
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetExchangeApiClient();

            // Act
            Func<Task> act = async () => await client.MIReport(null);

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Post, TestBaseAddress + MIReportRoute);
            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        private ExchangeApiClient GetExchangeApiClient()
            => new ExchangeApiClient(GetMockAuthenticationService<DataApiClientConfiguration>().Object, GetHttpClient(), Options.Create(GetServicesConfiguration()), _logger);
    }
}