using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Services.Models.Filters;
using Pds.DocumentExchange.Web.Enums;
using Pds.DocumentExchange.Web.Implementations.Coordinators;
using Pds.DocumentExchange.Web.Interfaces.Converters;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models.Agency;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Tests.Unit.Coordinators
{
    [TestClass, TestCategory("Unit")]
    public sealed class AgencyExchangeDeletionRequestCoordinatorTests
    {
        private readonly IUserInformationProvider _userInfoProvider
            = Mock.Of<IUserInformationProvider>(MockBehavior.Strict);

        private readonly IExchangeApiClient _exchangeApiClient
            = Mock.Of<IExchangeApiClient>(MockBehavior.Strict);

        private readonly ISupportToolsApiClient _supportToolsApiClient
          = Mock.Of<ISupportToolsApiClient>(MockBehavior.Strict);

        private readonly IDocumentModelConverter _documentConverter
            = Mock.Of<IDocumentModelConverter>(MockBehavior.Strict);

        private readonly IDocumentReferenceService _docRefService
            = Mock.Of<IDocumentReferenceService>(MockBehavior.Strict);

        [TestMethod]
        [DataRow(1)]
        [DataRow(13)]
        [DataRow(37)]
        public async Task GetDocumentsToDelete_ReturnsExpectedResults(int numberOfReferences)
        {
            // Arrange
            var fakeTeams = "teams";
            var docRefStrings = Enumerable.Range(0, numberOfReferences).Select(id => $"ref{id}").ToList();

            var docRefs = Enumerable.Range(0, numberOfReferences).Select(id => new DocumentReference
            {
                BatchIdentifier = $"batch {id}",
                FileName = $"file {id}.doc",
                ParentBatchIdentifier = $"parent batch {id}"
            }).ToList();

            ExchangeListDocumentOptions actualOptions = null;

            var testDocuments = Enumerable.Range(0, numberOfReferences).Select(id => new ExchangeDocument
            {
                DocumentReference = docRefs.ElementAt(id)
            }).ToList();

            var testConvertedDocuments = Enumerable.Range(0, numberOfReferences).Select(id => new AgencyExchangeDocument
            {
                ParentBatchIdentifier = docRefs.ElementAt(id).ParentBatchIdentifier,
                BatchIdentifier = docRefs.ElementAt(id).BatchIdentifier,
                FileName = docRefs.ElementAt(id).FileName
            }).ToList();

            Mock.Get(_userInfoProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync(fakeTeams);

            Mock.Get(_docRefService)
                .Setup(dr => dr.CreateDocumentReferenceFromString(It.IsAny<string>()))
                .Returns((string docRefString) =>
                {
                    var index = docRefStrings.IndexOf(docRefString);
                    return docRefs.ElementAt(index);
                });

            Mock.Get(_exchangeApiClient)
                .Setup(e => e.GetAgencyTeamDocuments(
                    fakeTeams,
                    It.IsAny<ExchangeListDocumentOptions>()))
                .ReturnsAsync((string teams, ExchangeListDocumentOptions options) =>
                {
                    actualOptions = options;
                    return new ListResult<ExchangeDocument> { Items = testDocuments };
                });

            Mock.Get(_documentConverter)
                .Setup(dr => dr.CreateAgencyExchangeDocumentFromExchangeDocument(It.IsAny<ExchangeDocument>()))
                .Returns((ExchangeDocument inputDocument) =>
                {
                    var index = testDocuments.IndexOf(inputDocument);
                    return testConvertedDocuments.ElementAt(index);
                });

            var coordinator = GetTestCoordinator();

            // Act
            var actual = await coordinator.GetDocumentsToDelete(docRefStrings);

            // Assert
            actual.Should().BeEquivalentTo(testConvertedDocuments);

            actualOptions.Should().BeEquivalentTo(new ExchangeListDocumentOptions
            {
                DocumentReferences = docRefs,
                DocumentStatusOption = ExchangeDocumentDirection.SentByOrganisation,
                PageSize = int.MaxValue,
                FilterOptions = Enumerable.Empty<IFilterOption>()
            });

            Mock.VerifyAll(
                Mock.Get(_userInfoProvider),
                Mock.Get(_docRefService),
                Mock.Get(_exchangeApiClient),
                Mock.Get(_documentConverter));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(13)]
        [DataRow(37)]
        public async Task GetDocumentsToDelete_WhenTeamIsSelected_ReturnsExpectedResults(int numberOfReferences)
        {
            // Arrange
            var fakeTeams = "teams";
            var docRefStrings = Enumerable.Range(0, numberOfReferences).Select(id => $"ref{id}").ToList();

            var docRefs = Enumerable.Range(0, numberOfReferences).Select(id => new DocumentReference
            {
                BatchIdentifier = $"batch {id}",
                FileName = $"file {id}.doc",
                ParentBatchIdentifier = $"parent batch {id}"
            }).ToList();

            var teamFilter = new RadioFilterCategory
            {
                Key = FilterKey.Team.ToString(),
                Value = fakeTeams
            };

            var filters = new IFilterCategory[]
            {
                 teamFilter
            };

            var teamFilterOption = new RadioFilterOption
            {
                Type = FilterOptionType.RadioFilterOption.ToString(),
                Key = teamFilter.Key,
                Value = teamFilter.Value
            };

            var filterOptions = new IFilterOption[]
            {
                teamFilterOption
            };

            ExchangeListDocumentOptions actualOptions = null;

            var testDocuments = Enumerable.Range(0, numberOfReferences).Select(id => new ExchangeDocument
            {
                DocumentReference = docRefs.ElementAt(id)
            }).ToList();

            var testConvertedDocuments = Enumerable.Range(0, numberOfReferences).Select(id => new AgencyExchangeDocument
            {
                ParentBatchIdentifier = docRefs.ElementAt(id).ParentBatchIdentifier,
                BatchIdentifier = docRefs.ElementAt(id).BatchIdentifier,
                FileName = docRefs.ElementAt(id).FileName
            }).ToList();

            Mock.Get(_userInfoProvider)
                    .Setup(u => u.GetCurrentUserAgencyTeams())
                    .ReturnsAsync(fakeTeams);

            Mock.Get(_docRefService)
                    .Setup(dr => dr.CreateDocumentReferenceFromString(It.IsAny<string>()))
                    .Returns((string docRefString) =>
                    {
                        var index = docRefStrings.IndexOf(docRefString);
                        return docRefs.ElementAt(index);
                    });

            Mock.Get(_exchangeApiClient)
                .Setup(e => e.GetAgencyTeamDocuments(
                    fakeTeams,
                    It.IsAny<ExchangeListDocumentOptions>()))
                .ReturnsAsync((string teams, ExchangeListDocumentOptions options) =>
                {
                    actualOptions = options;
                    return new ListResult<ExchangeDocument> { Items = testDocuments };
                });

            Mock.Get(_documentConverter)
                .Setup(dr => dr.CreateAgencyExchangeDocumentFromExchangeDocument(It.IsAny<ExchangeDocument>()))
                .Returns((ExchangeDocument inputDocument) =>
                {
                    var index = testDocuments.IndexOf(inputDocument);
                    return testConvertedDocuments.ElementAt(index);
                });

            var coordinator = GetTestCoordinator();

            // Act
            var actual = await coordinator.GetDocumentsToDelete(docRefStrings, filters);

            // Assert
            actual.Should().BeEquivalentTo(testConvertedDocuments);

            actualOptions.Should().BeEquivalentTo(new ExchangeListDocumentOptions
            {
                DocumentReferences = docRefs,
                DocumentStatusOption = ExchangeDocumentDirection.SentByOrganisation,
                PageSize = int.MaxValue,
                FilterOptions = filterOptions
            });

            Mock.VerifyAll(
                Mock.Get(_userInfoProvider),
                Mock.Get(_docRefService),
                Mock.Get(_exchangeApiClient),
                Mock.Get(_documentConverter));
        }

        [TestMethod]
        public async Task GetDocumentsToDeleteWithDirection_WhenNoDocumentFound_ReturnsNull()
        {
            // Arrange
            ExchangeDocumentDirection direction = ExchangeDocumentDirection.PublishedByAgency;
            int ukprn = 12345678;
            string fileType = "filetype1";
            string year = "202021";


            Mock.Get(_supportToolsApiClient)
               .Setup(e => e.DeleteDocumentsSearch(direction, ukprn, fileType, year))
               .ReturnsAsync((ExchangeDocumentDirection exchangeDocumentDirection, int ukprn, string fileType, string year) =>
               {
                   return null;
               });


            var coordinator = GetTestCoordinator();

            // Act
            var actual = await coordinator.GetDocumentsToDelete(direction, ukprn, fileType, year);

            // Assert
            actual.Should().BeNull();

            Mock.VerifyAll(
                Mock.Get(_userInfoProvider),
                Mock.Get(_supportToolsApiClient));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(13)]
        [DataRow(37)]
        public async Task GetDocumentsToDeleteWithDirection_ReturnsExpectedResults(int numberOfReferences)
        {
            // Arrange
            ExchangeDocumentDirection direction = ExchangeDocumentDirection.PublishedByAgency;
            int ukprn = 12345678;
            string fileType = "filetype1";
            string year = "202021";

            var docRefStrings = Enumerable.Range(0, numberOfReferences).Select(id => $"ref{id}").ToList();

            var docRefs = Enumerable.Range(0, numberOfReferences).Select(id => new DocumentReference
            {
                BatchIdentifier = $"batch {id}",
                FileName = $"file {id}.doc",
                ParentBatchIdentifier = $"parent batch {id}"
            }).ToList();

            var testDocuments = Enumerable.Range(0, numberOfReferences).Select(id => new ExchangeDocument
            {
                DocumentReference = docRefs.ElementAt(id)
            }).ToList();

            var testConvertedDocuments = Enumerable.Range(0, numberOfReferences).Select(id => new AgencyExchangeDocument
            {
                ParentBatchIdentifier = docRefs.ElementAt(id).ParentBatchIdentifier,
                BatchIdentifier = docRefs.ElementAt(id).BatchIdentifier,
                FileName = docRefs.ElementAt(id).FileName
            }).ToList();

            Mock.Get(_supportToolsApiClient)
               .Setup(e => e.DeleteDocumentsSearch(direction, ukprn, fileType, year))
               .ReturnsAsync((ExchangeDocumentDirection exchangeDocumentDirection, int ukprn, string fileType, string year) =>
               {
                   return new ListResult<ExchangeDocument> { Items = testDocuments };
               });

            Mock.Get(_documentConverter)
                .Setup(dr => dr.CreateAgencyExchangeDocumentFromExchangeDocument(It.IsAny<ExchangeDocument>()))
                .Returns((ExchangeDocument inputDocument) =>
                {
                    var index = testDocuments.IndexOf(inputDocument);
                    return testConvertedDocuments.ElementAt(index);
                });

            var coordinator = GetTestCoordinator();

            // Act
            var actual = await coordinator.GetDocumentsToDelete(direction, ukprn, fileType, year);

            // Assert
            actual.Should().BeEquivalentTo(testConvertedDocuments);

            Mock.VerifyAll(
                Mock.Get(_userInfoProvider),
                Mock.Get(_supportToolsApiClient),
                Mock.Get(_documentConverter));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(13)]
        [DataRow(37)]
        public async Task DeleteDocuments_ReturnsExpectedResult(int numberOfReferences)
        {
            // Arrange
            var fakeUserInfo = new UserInfo
            {
                FullName = "fake user"
            };

            var docRefStrings = Enumerable.Range(0, numberOfReferences).Select(id => $"ref{id}").ToList();

            var docRefs = Enumerable.Range(0, numberOfReferences).Select(id => new DocumentReferenceWithPreviousVersions(new DocumentReference
            {
                BatchIdentifier = $"batch {id}",
                FileName = $"file {id}.doc",
                ParentBatchIdentifier = $"parent batch {id}"
            })).ToList();

            ExchangeDocumentDeleteRequest actualOptions = null;

            var testDocuments = Enumerable.Range(0, numberOfReferences).Select(id => new ExchangeDocument
            {
                DocumentReference = docRefs.ElementAt(id)
            }).ToList();

            var testConvertedDocuments = Enumerable.Range(0, numberOfReferences).Select(id => new AgencyExchangeDocument
            {
                ParentBatchIdentifier = docRefs.ElementAt(id).ParentBatchIdentifier,
                BatchIdentifier = docRefs.ElementAt(id).BatchIdentifier,
                FileName = docRefs.ElementAt(id).FileName
            }).ToList();

            var expectedAgencyExchangeDocuments = AddVersionsAndPublishedByToTestDownloadFileshareDocuments(testConvertedDocuments);

            foreach (AgencyExchangeDocument document in expectedAgencyExchangeDocuments)
            {
                document.IsDeleted = true;
            }

            Mock.Get(_userInfoProvider)
                .Setup(u => u.GetCurrentUserInfo())
                .ReturnsAsync(fakeUserInfo);

            Mock.Get(_docRefService)
                .Setup(dr => dr.CreateDocumentReferencesWithPreviousVersionsFromStrings(docRefStrings))
                .Returns(docRefs);

            Mock.Get(_exchangeApiClient)
                .Setup(e => e.DeleteDocuments(It.IsAny<ExchangeDocumentDeleteRequest>()))
                .ReturnsAsync((ExchangeDocumentDeleteRequest options) =>
                {
                    actualOptions = options;
                    return testDocuments;
                });

            Mock.Get(_documentConverter)
                .Setup(dr => dr.CreateAgencyExchangeDocumentFromExchangeDocument(It.IsAny<ExchangeDocument>()))
                .Returns((ExchangeDocument inputDocument) =>
                {
                    var index = testDocuments.IndexOf(inputDocument);
                    return expectedAgencyExchangeDocuments.ElementAt(index);
                });

            var coordinator = GetTestCoordinator();

            // Act
            var actual = await coordinator.DeleteDocuments(docRefStrings);

            // Assert
            actual.Should().BeEquivalentTo(expectedAgencyExchangeDocuments);

            actualOptions.Should().BeEquivalentTo(new ExchangeDocumentDeleteRequest
            {
                UserInfo = fakeUserInfo,
                DocumentReferences = docRefs
            });

            Mock.VerifyAll(
                Mock.Get(_userInfoProvider),
                Mock.Get(_docRefService),
                Mock.Get(_exchangeApiClient),
                Mock.Get(_documentConverter));
        }

        [TestMethod]
        public async Task DeleteDocumentVersion_ReturnsExpectedResults()
        {
            // Arrange
            var fakeUserInfo = new UserInfo
            {
                FullName = "fake user"
            };

            var docRefStrings = Enumerable.Range(0, 1).Select(id => $"ref{id}").ToList();

            var docRefs = Enumerable.Range(0, 1).Select(id => new DocumentReferenceWithPreviousVersions(new DocumentReference
            {
                BatchIdentifier = $"batch {id}",
                FileName = $"file {id}.doc",
                ParentBatchIdentifier = $"parent batch {id}"
            })).ToList();

            ExchangeDocumentDeleteRequest actualOptions = null;

            var testDocuments = Enumerable.Range(0, 1).Select(id => new ExchangeDocument
            {
                DocumentReference = docRefs.ElementAt(id)
            }).ToList();

            var testConvertedDocuments = Enumerable.Range(0, 1).Select(id => new AgencyExchangeDocument
            {
                ParentBatchIdentifier = docRefs.ElementAt(id).ParentBatchIdentifier,
                BatchIdentifier = docRefs.ElementAt(id).BatchIdentifier,
                FileName = docRefs.ElementAt(id).FileName
            }).ToList();

            Mock.Get(_userInfoProvider)
                .Setup(u => u.GetCurrentUserInfo())
                .ReturnsAsync(fakeUserInfo);

            Mock.Get(_docRefService)
                .Setup(dr => dr.CreateDocumentReferenceFromString(docRefStrings.FirstOrDefault()))
                .Returns(docRefs.FirstOrDefault());

            Mock.Get(_exchangeApiClient)
                .Setup(e => e.DeleteDocument(It.IsAny<ExchangeDocumentDeleteRequest>()))
                .ReturnsAsync((ExchangeDocumentDeleteRequest options) =>
                {
                    actualOptions = options;
                    return testDocuments.FirstOrDefault();
                });

            Mock.Get(_documentConverter)
                .Setup(dr => dr.CreateAgencyExchangeDocumentFromExchangeDocument(It.IsAny<ExchangeDocument>()))
                .Returns((ExchangeDocument inputDocument) =>
                {
                    var index = testDocuments.IndexOf(inputDocument);
                    return testConvertedDocuments.ElementAt(index);
                });

            var coordinator = GetTestCoordinator();

            // Act
            var actual = await coordinator.DeleteDocumentVersion(docRefStrings.FirstOrDefault());

            // Assert
            actual.Should().BeEquivalentTo(testConvertedDocuments.FirstOrDefault());

            actualOptions.Should().BeEquivalentTo(new ExchangeDocumentDeleteRequest
            {
                UserInfo = fakeUserInfo,
                DocumentReferences = docRefs
            });

            Mock.VerifyAll(
                Mock.Get(_userInfoProvider),
                Mock.Get(_docRefService),
                Mock.Get(_exchangeApiClient),
                Mock.Get(_documentConverter));
        }

        private AgencyExchangeDeletionRequestCoordinator GetTestCoordinator()
        {
            return new AgencyExchangeDeletionRequestCoordinator(
                _userInfoProvider,
                _exchangeApiClient,
                _documentConverter,
                _docRefService,
                _supportToolsApiClient);
        }

        private IEnumerable<AgencyExchangeDocument> AddVersionsAndPublishedByToTestDownloadFileshareDocuments(List<AgencyExchangeDocument> agencyExchangeDocuments, string publishedBy = null)
        {
            var amendedAgencyExchangeDocuments = agencyExchangeDocuments;
            foreach (AgencyExchangeDocument document in amendedAgencyExchangeDocuments)
            {
                document.PublishedBy = publishedBy;
                document.VersionNumber = amendedAgencyExchangeDocuments.IndexOf(document) + 1;

                for (int i = document.VersionNumber; i > 0; i--)
                {
                    if (document.VersionNumber > 1)
                    {
                        document.VersionsWithoutHyperlinks = string.Join(i.ToString(), ", ");
                        document.Versions = string.Join(i.ToString(), ", ");
                    }
                    else
                    {
                        document.VersionsWithoutHyperlinks = i.ToString();
                        document.Versions = i.ToString();
                    }
                }
            }

            return amendedAgencyExchangeDocuments;
        }
    }
}