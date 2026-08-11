//using AutoMapper;
using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Common.Organisation.Enums;
using Pds.Core.Common.Organisation.Models;
using Pds.Core.Web.Components.Areas.Lists.Builders;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Implementations;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.Implementations.Converters;
using Pds.DocumentExchange.Web.Implementations.Coordinators;
using Pds.DocumentExchange.Web.Implementations.Helpers;
using Pds.DocumentExchange.Web.Models;
using Pds.DocumentExchange.Web.Models.SupportTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Tests.Integration
{
    [TestClass]
    [TestCategory("Unit")]
    public class SupportToolsControllerTests : BaseControllerIntegrationTests
    {
        private readonly SupportToolsController _controller;

        public SupportToolsControllerTests()
        {
            var dateTimeDisplayHelper = new DateTimeDisplayHelper(SystemProvider.DateTime);

            var documentConverter =
                new DocumentModelConverter(
                    dateTimeDisplayHelper,
                    new DocumentStatusProvider(),
                    null);

            var documentReferenceService = new DocumentReferenceService();

            var coordinator = new AgencyExchangeDeletionRequestCoordinator(
                    UserInfoProvider,
                    ExchangeApiClient,
                    documentConverter,
                    documentReferenceService,
                    SupportToolsApiClient);

            var testConfigurationOptions = Options.Create(TestConfiguration);

            var listHelper =
                new ListHelper(
                    new RouteValueDictionaryBuilder(),
                    Mapper,
                    SystemProvider,
                    testConfigurationOptions);

            _controller = new SupportToolsController(
                UserInfoProvider,
                SupportToolsApiClient,
                SettingsApiClient,
                DateTimeProvider,
                Options.Create(new DocumentExchangeConfiguration()),
                coordinator,
                listHelper);
        }

        [TestMethod]
        public async Task SupportTools_ReturnsExpectedViewAsync()
        {
            // Act
            SetupUserIdentity(TestAgencyAdvancedUser);

            var result = await _controller.SupportTools();

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<SupportToolsModel>();
        }

        [TestMethod]
        public async Task DocumentsPublishedByDfe_ReturnsExpectedView()
        {
            // Arrange
            SetupUserIdentity(TestAgencyAdvancedUser);

            var apiPublishedBatches = new[]
            {
                new PublishedBatch
                {
                    DateAndTime = new DateTime(2021, 11, 1),
                    ParentBatchIdentifier = Guid.Parse("1ee2cc42-b604-494c-8358-33ee793ae845"),
                    EmailAddress = "email.address.01@education.co.uk",
                    NumberOfDocuments = 1000,
                    NumberOfEmails = 2500
                },
                new PublishedBatch
                {
                    DateAndTime = new DateTime(2021, 12, 1),
                    ParentBatchIdentifier = Guid.Parse("55a75877-4e22-4126-b826-0fc3eda4b7f8"),
                    EmailAddress = "email.address.02@education.co.uk",
                    NumberOfDocuments = 2000,
                    NumberOfEmails = 3500
                }
            };

            var apiExpected = new ListResult<PublishedBatch>
            {
                TotalItems = apiPublishedBatches.Length,
                TotalPages = 1,
                Items = apiPublishedBatches
            };

            var expected = new[]
            {
                new PublishedBatchItem
                {
                    DateAndTime = apiPublishedBatches[0].DateAndTime,
                    ParentBatchIdentifier = apiPublishedBatches[0].ParentBatchIdentifier,
                    EmailAddress = apiPublishedBatches[0].EmailAddress,
                    NumberOfDocuments = apiPublishedBatches[0].NumberOfDocuments,
                    NumberOfEmails = apiPublishedBatches[0].NumberOfEmails
                },
                new PublishedBatchItem
                {
                    DateAndTime = apiPublishedBatches[1].DateAndTime,
                    ParentBatchIdentifier = apiPublishedBatches[1].ParentBatchIdentifier,
                    EmailAddress = apiPublishedBatches[1].EmailAddress,
                    NumberOfDocuments = apiPublishedBatches[1].NumberOfDocuments,
                    NumberOfEmails = apiPublishedBatches[1].NumberOfEmails
                }
            };

            int pageNumber = 1;
            int pageSize = 25;

            Mock.Get(SupportToolsApiClient)
                .Setup(a => a.GetDocumentsPublishedByDfE(pageNumber, pageSize))
                .ReturnsAsync(apiExpected);

            var listRequest = new Core.Web.Components.Areas.Lists.DTOs.ListRequest
            {
                Page = pageNumber
            };

            // Act
            var result = await _controller.DocumentsPublishedByDfe(listRequest);

            // Assert
            var listItems = result.Should().BeViewResult()
                .Model.Should().BeOfType<PublishedBatchesViewModel>()
                .Which.ListItems;

            listItems.Should().BeEquivalentTo(expected);

            Mock.VerifyAll(Mock.Get(SupportToolsApiClient));
        }

        [TestMethod]
        public async Task DownloadDocumentsCsv_ReturnsExpectedCsvFile()
        {
            // Arrange
            SetupUserIdentity(TestAgencyAdvancedUser);

            var parentBatchId = Guid.Parse("24132c20-0f40-4ddd-989d-3bed23f42aae");

            var fileContent = new byte[] { 1, 2, 3, 4, 5 };
            var now = new DateTime(2021, 11, 1);

            Mock.Get(SupportToolsApiClient)
                .Setup(a => a.DownloadDocumentsPublishedCsv(parentBatchId))
                .ReturnsAsync(fileContent);

            Mock.Get(DateTimeProvider)
                .Setup(d => d.Now())
                .Returns(now);

            // Act
            var result = await _controller.DownloadDocumentsCsv(parentBatchId);

            // Assert
            result.Should().BeFileContentResult()
                .WithContentType("text/csv")
                .WithFileDownloadName($"Documents_{now}.csv");

            var fileContentResult = result.As<FileContentResult>();
            fileContentResult.FileContents.Should().BeEquivalentTo(fileContent);

            Mock.VerifyAll(
                Mock.Get(SupportToolsApiClient),
                Mock.Get(DateTimeProvider));
        }

        [TestMethod]
        public async Task DownloadEmailsCsv_ReturnsExpectedCsvFile()
        {
            // Arrange
            SetupUserIdentity(TestAgencyAdvancedUser);

            var parentBatchId = Guid.Parse("24132c20-0f40-4ddd-989d-3bed23f42aae");

            var fileContent = new byte[] { 1, 2, 3, 4, 5 };
            var now = new DateTime(2021, 11, 1);

            Mock.Get(SupportToolsApiClient)
                .Setup(a => a.DownloadNotificationRecipientsCsv(parentBatchId))
                .ReturnsAsync(fileContent);

            Mock.Get(DateTimeProvider)
                .Setup(d => d.Now())
                .Returns(now);

            // Act
            var result = await _controller.DownloadEmailsCsv(parentBatchId);

            // Assert
            result.Should().BeFileContentResult()
                .WithContentType("text/csv")
                .WithFileDownloadName($"Emails_{now}.csv");

            var fileContentResult = result.As<FileContentResult>();
            fileContentResult.FileContents.Should().BeEquivalentTo(fileContent);

            Mock.VerifyAll(
                Mock.Get(SupportToolsApiClient),
                Mock.Get(DateTimeProvider));
        }

        [TestMethod]
        public async Task SupportTools_DeleteDocuments()
        {
            // Act
            SetupUserIdentity(TestAgencyAdvancedUser);

            var result = await _controller.DeleteDocuments(null);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<DeleteDocumentsViewModel>();
        }

        [TestMethod]
        public async Task DeletePublishedDocument_WhenDocumentReferencePassed_DeletesTheDocument()
        {
            // Arrange
            SetupUserIdentity(TestAgencyAdvancedUser);

            var testDocId = "file.pdf|batch-x|parent-y";
            var testDocRefData = new DocumentReference
            {
                FileName = testDocId.Split('|')[0],
                BatchIdentifier = testDocId.Split('|')[1],
                ParentBatchIdentifier = testDocId.Split('|')[2]
            };

            var expectedDocument = GetTestExchangeDocumentsPublishedByAgency(1).FirstOrDefault();

            var mockExchangeClient = Mock.Get(ExchangeApiClient);

            ExchangeDocumentDeleteRequest actualRequest = null;
            mockExchangeClient
                .Setup(e => e.DeleteDocument(It.IsAny<ExchangeDocumentDeleteRequest>()))
                .ReturnsAsync((ExchangeDocumentDeleteRequest request) =>
                {
                    actualRequest = request;
                    return expectedDocument;
                });

            var fakeDateTime = new DateTime(2020, 12, 25, 15, 37, 23, 123);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.ConvertToUKTime(It.IsAny<DateTime>()))
                .Returns((DateTime dt) => dt);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.Now())
                .Returns(fakeDateTime);

            var expectedViewModel = new DeleteDocumentVersionsConfirmation()
            {
                IsAllVersions = false,
                ProviderName = expectedDocument.OrganisationInfo.Name,
                ProductName = expectedDocument.Product.Name,
                ProviderUkprn = expectedDocument.OrganisationInfo.OrganisationIdentifier.Value,
                Version = "1"
            };

            // Act
            var result = await _controller.DeletePublishedDocument(new DeletePublishedDocumentAreYouSure
            {
                SelectedDocument = testDocId
            });

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedViewModel);
            actualRequest.Should().BeEquivalentTo(new ExchangeDocumentDeleteRequest
            {
                DocumentReferences = new List<DocumentReferenceWithPreviousVersions>
                    {
                        new DocumentReferenceWithPreviousVersions(testDocRefData)
                    }
            });
        }

        [DataRow(1, 1)]
        [DataRow(1, 2)]
        [DataRow(2, 1)]
        [DataRow(2, 2)]
        [DataRow(2, 3)]
        [TestMethod]
        public async Task DeleteSelectedDocumentVersion_WhenVersionSelected_DeletesTheVersion(int numberOfPreviousVersions, int versionNumberToDelete)
        {
            // Arrange
            var testDocument = GetTestExchangeDocumentPublishedByAgencyWithPreviousVersions(numberOfPreviousVersions);

            var expectedDocument = testDocument.Version == versionNumberToDelete ? testDocument : testDocument.PreviousVersions.FirstOrDefault(d => d.Version == versionNumberToDelete);

            var testDocId = GetDocumentIdFromReference(expectedDocument.DocumentReference);

            var mockExchangeClient = Mock.Get(ExchangeApiClient);

            ExchangeDocumentDeleteRequest actualRequest = null;
            mockExchangeClient
                .Setup(e => e.DeleteDocument(It.IsAny<ExchangeDocumentDeleteRequest>()))
                .ReturnsAsync((ExchangeDocumentDeleteRequest request) =>
                {
                    actualRequest = request;
                    return expectedDocument;
                });

            var fakeDateTime = new DateTime(2020, 12, 25, 15, 37, 23, 123);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.ConvertToUKTime(It.IsAny<DateTime>()))
                .Returns((DateTime dt) => dt);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.Now())
                .Returns(fakeDateTime);

            var inputViewModel = new DeleteDocumentsVersionSelect
            {
                SelectedVersion = testDocId
            };

            var expectedViewModel = new DeleteDocumentVersionsConfirmation
            {
                IsAllVersions = false,
                ProviderName = expectedDocument.OrganisationInfo.Name,
                ProductName = expectedDocument.Product.Name,
                ProviderUkprn = expectedDocument.OrganisationInfo.OrganisationIdentifier.Value,
                Version = expectedDocument.Version.ToString()
            };

            var listRequest = PublishedByAgencyListRequest();

            // Act
            var result = await _controller.DeleteSelectedDocumentVersion(inputViewModel);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedViewModel);

            actualRequest.Should().BeEquivalentTo(new ExchangeDocumentDeleteRequest
            {
                DocumentReferences = new List<DocumentReferenceWithPreviousVersions>
                    {
                        new DocumentReferenceWithPreviousVersions(expectedDocument.DocumentReference)
                    }
            });
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [TestMethod]
        public async Task DeleteSelectedDocumentVersion_WhenAllVersionSelected_DeletesAllVersions(int numberOfPreviousVersions)
        {
            // Arrange
            SetupUserIdentity(TestAgencyUser);

            var testDocument = GetTestExchangeDocumentPublishedByAgencyWithPreviousVersions(numberOfPreviousVersions);

            var expectedDocument = GetTestExchangeDocumentPublishedByAgencyWithPreviousVersions(numberOfPreviousVersions);

            var testDocId = GetDocumentIdFromReference(expectedDocument.DocumentReference);

            var mockExchangeClient = Mock.Get(ExchangeApiClient);

            ExchangeDocumentDeleteRequest actualRequest = null;
            mockExchangeClient
                .Setup(e => e.DeleteDocuments(It.IsAny<ExchangeDocumentDeleteRequest>()))
                .ReturnsAsync((ExchangeDocumentDeleteRequest request) =>
                {
                    actualRequest = request;
                    return new List<ExchangeDocument> { expectedDocument };
                });

            var fakeDateTime = new DateTime(2020, 12, 25, 15, 37, 23, 123);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.ConvertToUKTime(It.IsAny<DateTime>()))
                .Returns((DateTime dt) => dt);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.Now())
                .Returns(fakeDateTime);

            var inputViewModel = new DeleteDocumentsVersionSelect
            {
                SelectedVersion = "AllVersions",
                DocumentReferences = new List<string> { testDocId }
            };

            var expectedVersionString = string.Join(", ", expectedDocument.PreviousVersions.OrderBy(x => x.Version).Select(y => y.Version.ToString()));

            expectedVersionString += expectedDocument.PreviousVersions.Any() ? (", " + expectedDocument.Version.ToString()) : expectedDocument.Version.ToString();
            var expectedViewModel = new DeleteDocumentVersionsConfirmation
            {
                IsAllVersions = true,
                ProviderName = expectedDocument.OrganisationInfo.Name,
                ProductName = expectedDocument.Product.Name,
                ProviderUkprn = expectedDocument.OrganisationInfo.OrganisationIdentifier.Value,
                Version = expectedVersionString
            };

            var listRequest = PublishedByAgencyListRequest();

            // Act
            var result = await _controller.DeleteSelectedDocumentVersion(inputViewModel);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedViewModel);

            actualRequest.Should().BeEquivalentTo(new ExchangeDocumentDeleteRequest
            {
                DocumentReferences = new List<DocumentReferenceWithPreviousVersions>
                    {
                        new DocumentReferenceWithPreviousVersions(expectedDocument.DocumentReference)
                    }
            });
        }

        private IEnumerable<ExchangeDocument> GetTestExchangeDocumentsPublishedByAgency(int numberOfPages, int pageNumber = 1)
        {
            if (numberOfPages == 0)
            {
                return Enumerable.Empty<ExchangeDocument>();
            }

            var publishedByAgency = new ExchangeDocumentEvent
            {
                EventType = ExchangeDocumentEventType.PublishedByAgency,
                EventDateTime = new DateTime(2020, 6, 1),
                UserInfo = new UserInfo
                {
                    Principal = "Martin Riggs",
                    FullName = "Mr. Martin Riggs",
                    EmailAddress = "martin.riggs@education.gov.uk",
                    OrganisationInfo = new OrganisationInfo
                    {
                        OrganisationIdentifier = new OrganisationIdentifier
                        {
                            Type = OrganisationIdentifierType.Ukprn,
                            Value = "-999",
                        },
                        Name = "Education & Skills Funding Agency"
                    }
                }
            };

            var downloadedByReceiverEvent = new ExchangeDocumentEvent
            {
                EventType = ExchangeDocumentEventType.DownloadedByReceiver,
                EventDateTime = new DateTime(2020, 7, 1),
                UserInfo = new UserInfo
                {
                    Principal = "John McClane",
                    FullName = "Lieutenant John McClane",
                    EmailAddress = "john.mcclane@gmail.com",
                    OrganisationInfo = new OrganisationInfo
                    {
                        OrganisationIdentifier = new OrganisationIdentifier
                        {
                            Type = OrganisationIdentifierType.Ukprn,
                            Value = "99999",
                        },
                        Name = "New York City Police Department"
                    }
                }
            };

            var eventsForNewStatus = new[] { publishedByAgency };

            var eventsForDownloadedStatus = new[]
            {
                publishedByAgency,
                downloadedByReceiverEvent
            };

            return Enumerable
                .Range(1, numberOfPages)
                .Select(p => Enumerable
                    .Range(((p - 1) * TestConfiguration.ListPageSize) + 1, TestConfiguration.ListPageSize)
                    .Select(d => new ExchangeDocument
                    {
                        DocumentReference = new DocumentReference
                        {
                            FileName = $"file{d}",
                            BatchIdentifier = $"batch{d}",
                            ParentBatchIdentifier = $"parent-batch{d}"
                        },
                        Product = new Product
                        {
                            Identifier = 10000 + p,
                            Name = $"product{p}"
                        },
                        Year = 201920,
                        OrganisationInfo = new OrganisationInfo
                        {
                            OrganisationIdentifier = new OrganisationIdentifier
                            {
                                Type = OrganisationIdentifierType.Ukprn,
                                Value = $"{10000000 + d}"
                            },
                            Name = $"school {d}"
                        },
                        AgencyTeam = TestAgencyAdvancedUser.Roles.First(),
                        ExchangeDirection = ExchangeDocumentDirection.PublishedByAgency,
                        EventHistory = d % 2 == 0 ? eventsForNewStatus : eventsForDownloadedStatus,
                        Version = d
                    })).ElementAt(pageNumber - 1);
        }

        private ExchangeDocument GetTestExchangeDocumentPublishedByAgencyWithPreviousVersions(int numberOfPreviousVersions)
        {
            var mainDocument = GetTestExchangeDocumentPublishedByAgency(numberOfPreviousVersions + 1);

            List<ExchangeDocument> previousVersions = new List<ExchangeDocument>();

            for (int i = 1; i <= numberOfPreviousVersions; i++)
            {
                previousVersions.Add(GetTestExchangeDocumentPublishedByAgency(i));
            }

            mainDocument.PreviousVersions = previousVersions;

            return mainDocument;
        }


        private ExchangeDocument GetTestExchangeDocumentPublishedByAgency(int counter)
        {
            return new ExchangeDocument
            {
                DocumentReference = new DocumentReference
                {
                    FileName = $"file{counter}",
                    BatchIdentifier = "batch",
                    ParentBatchIdentifier = "parent-batch"
                },
                Product = new Product
                {
                    Identifier = 10000,
                    Name = "product"
                },
                OrganisationInfo = new OrganisationInfo
                {
                    OrganisationIdentifier = new OrganisationIdentifier
                    {
                        Type = OrganisationIdentifierType.Ukprn,
                        Value = "10000000"
                    },
                    Name = "school"
                },
                AgencyTeam = TestAgencyAdvancedUser.Roles.First(),
                ExchangeDirection = ExchangeDocumentDirection.PublishedByAgency,
                Version = counter
            };
        }

        private string GetDocumentIdFromReference(DocumentReference docReference)
           => $"{docReference.FileName}|{docReference.BatchIdentifier}|{docReference.ParentBatchIdentifier}";
    }
}