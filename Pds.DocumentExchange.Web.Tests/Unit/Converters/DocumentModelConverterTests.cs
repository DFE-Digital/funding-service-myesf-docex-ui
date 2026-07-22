using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Common.Organisation.Models;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.Implementations.Converters;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using Pds.DocumentExchange.Web.Interfaces.Renderer;
using Pds.DocumentExchange.Web.Models.Agency;
using System;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Tests.Unit.Converters
{
    [TestClass, TestCategory("Unit")]
    public sealed class DocumentModelConverterTests
    {
        private readonly IDateTimeDisplayHelper _dateTimeDisplayHelper
            = Mock.Of<IDateTimeDisplayHelper>(MockBehavior.Strict);

        private readonly IDocumentStatusProvider _documentStatusProvider
            = Mock.Of<IDocumentStatusProvider>(MockBehavior.Strict);

        private readonly IDocumentVersionRenderer _documentVersionRenderer
            = Mock.Of<IDocumentVersionRenderer>(MockBehavior.Strict);

        [TestMethod, DynamicData(nameof(CreateAgencyExchangeDocumentFromExchangeDocument_TestData))]
        public void CreateAgencyExchangeDocumentFromExchangeDocument_ReturnsExpected(
            ExchangeDocument inputDocument,
            AgencyExchangeDocument expectedResult)
        {
            // Arrange
            var fakeDocumentStatus = new DocumentStatusInfo
            {
                DownloadedTime = expectedResult.StatusDateTime.Value,
                DownloadedBy = expectedResult.DownloadedBy,
                Status = expectedResult.Status
            };

            Mock.Get(_documentStatusProvider)
                .Setup(dsp => dsp.GetDownloadStatus(inputDocument.EventHistory))
                .Returns(fakeDocumentStatus);

            Mock.Get(_dateTimeDisplayHelper)
                .Setup(dt => dt.ToTimeAndDateDisplayString(fakeDocumentStatus.DownloadedTime))
                .Returns(expectedResult.DisplayStatusDateTime);

            Mock.Get(_dateTimeDisplayHelper)
              .Setup(dt => dt.ToTimeAndDateDisplayString(DateTime.MinValue))
              .Returns(expectedResult.DisplayStatusDateTime);

            Mock.Get(_documentVersionRenderer)
                .Setup(dvr => dvr.GetHyperLinkedDocumentVersionLabels(It.IsAny<ExchangeDocument>()))
                .Returns(expectedResult.Versions);

            Mock.Get(_documentVersionRenderer)
               .Setup(dvr => dvr.GetHyperLinkedDocumentVersionLabelWithPrefix(It.IsAny<ExchangeDocument>(), It.IsAny<string>()))
               .Returns(expectedResult.Version);

            Mock.Get(_documentVersionRenderer)
               .Setup(dvr => dvr.GetDocumentVersionsWithoutHyperlinks(It.IsAny<ExchangeDocument>()))
               .Returns(expectedResult.VersionsWithoutHyperlinks);

            Mock.Get(_dateTimeDisplayHelper)
                .Setup(dt => dt.ToTimeAndDateDisplayString(expectedResult.UploadedDateTime))
                .Returns(expectedResult.DisplayUploadedDateTime);

            Mock.Get(_dateTimeDisplayHelper)
               .Setup(dt => dt.ToTimeAndDateDisplayString(expectedResult.PublishedDateTime))
               .Returns(expectedResult.DisplayPublishedDateTime);

            var converter = GetTestConverter();

            // Act
            var result = converter.CreateAgencyExchangeDocumentFromExchangeDocument(inputDocument);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);

            Mock.VerifyAll(
                Mock.Get(_documentStatusProvider),
                Mock.Get(_dateTimeDisplayHelper));
        }

        private static IEnumerable<object[]> CreateAgencyExchangeDocumentFromExchangeDocument_TestData
        {
            get
            {
                yield return new object[]
                {
                    new ExchangeDocument
                    {
                        DocumentReference = new DocumentReference
                        {
                            ParentBatchIdentifier = "parent-batch-id",
                            BatchIdentifier = "batch-id",
                            FileName = "filename.pdf"
                        },
                        Product = new Product { Name = "product-name" },
                        OrganisationInfo = new OrganisationInfo
                        {
                            Name = "org-name",
                            OrganisationIdentifier = new OrganisationIdentifier { Value = "org-ukprn" }
                        },
                        Version = 1
                    },
                    new AgencyExchangeDocument
                    {
                        FileName = "filename.pdf",
                        BatchIdentifier = "batch-id",
                        ParentBatchIdentifier = "parent-batch-id",
                        ProductName = "product-name",
                        Status = DownloadDocumentStatus.New,
                        StatusDateTime = new DateTime(2020, 12, 25, 1, 2, 3, 4),
                        DownloadedBy = "fake-user",
                        DisplayStatusDateTime = "fake-downloaded-at",
                        ProviderName = "org-name",
                        ProviderUkprn = "org-ukprn",
                        Versions = "1",
                        VersionsWithoutHyperlinks = "1",
                        VersionWithHyperlink = "1",
                        Version = "View version 1",
                        VersionNumber = 1,
                        UploadedDateTime = DateTime.MinValue,
                        DisplayUploadedDateTime = "fake-uploaded-at",
                        PublishedBy = string.Empty,
                        PublishedDateTime = DateTime.MinValue,
                        DisplayPublishedDateTime = "fake-uploaded-at"
                    }
                };
                yield return new object[]
                {
                    new ExchangeDocument
                    {
                        DocumentReference = new DocumentReference
                        {
                            ParentBatchIdentifier = "parent-batch-id",
                            BatchIdentifier = "batch-id",
                            FileName = "filename.pdf"
                        },
                        Product = new Product { Name = "product-name" },
                        OrganisationInfo = new OrganisationInfo
                        {
                            Name = "org-name",
                            OrganisationIdentifier = new OrganisationIdentifier { Value = "org-ukprn" }
                        },
                        Version = 1,
                        EventHistory = new List<ExchangeDocumentEvent>()
                    },
                    new AgencyExchangeDocument
                    {
                        FileName = "filename.pdf",
                        BatchIdentifier = "batch-id",
                        ParentBatchIdentifier = "parent-batch-id",
                        ProductName = "product-name",
                        Status = DownloadDocumentStatus.New,
                        StatusDateTime = new DateTime(2020, 12, 25, 1, 2, 3, 4),
                        DownloadedBy = "fake-user",
                        DisplayStatusDateTime = "fake-downloaded-at",
                        ProviderName = "org-name",
                        ProviderUkprn = "org-ukprn",
                        Versions = "1",
                        VersionsWithoutHyperlinks = "1",
                        VersionWithHyperlink = "1",
                        Version = "View version 1",
                        VersionNumber = 1,
                        UploadedDateTime = DateTime.MinValue,
                        DisplayUploadedDateTime = "fake-uploaded-at",
                        PublishedBy = string.Empty,
                        PublishedDateTime = DateTime.MinValue,
                        DisplayPublishedDateTime = "fake-uploaded-at"
                    }
                };
                yield return new object[]
                {
                    new ExchangeDocument
                    {
                        DocumentReference = new DocumentReference
                        {
                            ParentBatchIdentifier = "parent-batch-id",
                            BatchIdentifier = "batch-id",
                            FileName = "filename.pdf"
                        },
                        Product = new Product { Name = "product-name" },
                        OrganisationInfo = new OrganisationInfo
                        {
                            Name = "org-name",
                            OrganisationIdentifier = new OrganisationIdentifier { Value = "org-ukprn" }
                        },
                        Version = 1,
                        EventHistory = new List<ExchangeDocumentEvent>
                        {
                            new ExchangeDocumentEvent
                            {
                                EventType = ExchangeDocumentEventType.SentByOrganisation,
                                EventDateTime = new DateTime(2020, 12, 1, 1, 2, 3, 4)
                            }
                        }
                    },
                    new AgencyExchangeDocument
                    {
                        FileName = "filename.pdf",
                        BatchIdentifier = "batch-id",
                        ParentBatchIdentifier = "parent-batch-id",
                        ProductName = "product-name",
                        Status = DownloadDocumentStatus.New,
                        StatusDateTime = new DateTime(2020, 12, 25, 1, 2, 3, 4),
                        DownloadedBy = "fake-user",
                        DisplayStatusDateTime = "fake-downloaded-at",
                        ProviderName = "org-name",
                        ProviderUkprn = "org-ukprn",
                        Versions = "1",
                        VersionsWithoutHyperlinks = "1",
                        VersionWithHyperlink = "1",
                        Version = "View version 1",
                        VersionNumber = 1,
                        UploadedDateTime = new DateTime(2020, 12, 1, 1, 2, 3, 4),
                        DisplayUploadedDateTime = "fake-uploaded-at",
                        PublishedBy = string.Empty,
                        PublishedDateTime = DateTime.MinValue,
                        DisplayPublishedDateTime = "fake-downloaded-at"
                    }
                };
                yield return new object[]
               {
                    new ExchangeDocument
                    {
                        DocumentReference = new DocumentReference
                        {
                            ParentBatchIdentifier = "parent-batch-id",
                            BatchIdentifier = "batch-id",
                            FileName = "filename.pdf"
                        },
                        Product = new Product { Name = "product-name" },
                        OrganisationInfo = new OrganisationInfo
                        {
                            Name = "org-name",
                            OrganisationIdentifier = new OrganisationIdentifier { Value = "org-ukprn" }
                        },
                        Version = 1,
                        EventHistory = new List<ExchangeDocumentEvent>
                        {
                            new ExchangeDocumentEvent
                            {
                                EventType = ExchangeDocumentEventType.PublishedByAgency,
                                UserInfo = new UserInfo { FullName = "publisherUser" },
                                EventDateTime = new DateTime(2020, 12, 1, 1, 2, 3, 4)
                            }
                        }
                    },
                    new AgencyExchangeDocument
                    {
                        FileName = "filename.pdf",
                        BatchIdentifier = "batch-id",
                        ParentBatchIdentifier = "parent-batch-id",
                        ProductName = "product-name",
                        Status = DownloadDocumentStatus.New,
                        StatusDateTime = new DateTime(2020, 12, 25, 1, 2, 3, 4),
                        DownloadedBy = "fake-user",
                        DisplayStatusDateTime = "fake-downloaded-at",
                        ProviderName = "org-name",
                        ProviderUkprn = "org-ukprn",
                        Versions = "1",
                        VersionsWithoutHyperlinks = "1",
                        VersionWithHyperlink = "1",
                        Version = "View version 1",
                        VersionNumber = 1,
                        UploadedDateTime = DateTime.MinValue,
                        DisplayUploadedDateTime = "fake-uploaded-at",
                        PublishedBy = "publisherUser",
                        PublishedDateTime = new DateTime(2020, 12, 1, 1, 2, 3, 4),
                        DisplayPublishedDateTime = "fake-uploaded-at"
                    }
               };
                yield return new object[]
                {
                    new ExchangeDocument
                    {
                        DocumentReference = new DocumentReference
                        {
                            ParentBatchIdentifier = "parent-batch-id-3",
                            BatchIdentifier = "batch-id-3",
                            FileName = "filename-3.pdf"
                        },
                        Product = new Product { Name = "product-name-3" },
                        OrganisationInfo = new OrganisationInfo
                        {
                            Name = "org-name-3",
                            OrganisationIdentifier = new OrganisationIdentifier { Value = "org-ukprn-3" }
                        },
                        Version = 3,
                        PreviousVersions = new List<ExchangeDocument>
                        {
                            new ExchangeDocument
                            {
                                DocumentReference = new DocumentReference
                                {
                                    ParentBatchIdentifier = "parent-batch-id-1",
                                    BatchIdentifier = "batch-id-1",
                                    FileName = "filename-1.pdf"
                                },
                                Product = new Product { Name = "product-name-1" },
                                OrganisationInfo = new OrganisationInfo
                                {
                                    Name = "org-name-1",
                                    OrganisationIdentifier = new OrganisationIdentifier { Value = "org-ukprn-1" }
                                },
                                Version = 1
                            },
                            new ExchangeDocument
                            {
                                DocumentReference = new DocumentReference
                                {
                                    ParentBatchIdentifier = "parent-batch-id-2",
                                    BatchIdentifier = "batch-id-2",
                                    FileName = "filename-2.pdf"
                                },
                                Product = new Product { Name = "product-name-2" },
                                OrganisationInfo = new OrganisationInfo
                                {
                                    Name = "org-name-2",
                                    OrganisationIdentifier = new OrganisationIdentifier { Value = "org-ukprn-2" }
                                },
                                Version = 2
                            }
                        }
                    },
                    new AgencyExchangeDocument
                    {
                        FileName = "filename-3.pdf",
                        BatchIdentifier = "batch-id-3",
                        ParentBatchIdentifier = "parent-batch-id-3",
                        ProductName = "product-name-3",
                        Status = DownloadDocumentStatus.New,
                        StatusDateTime = new DateTime(2020, 12, 25, 1, 2, 3, 4),
                        DownloadedBy = "fake-user",
                        DisplayStatusDateTime = "fake-downloaded-at",
                        ProviderName = "org-name-3",
                        ProviderUkprn = "org-ukprn-3",
                        Versions = "3, 2, 1",
                        VersionsWithoutHyperlinks = "3, 2, 1",
                        VersionWithHyperlink = "1",
                        Version = "View version 1",
                        VersionNumber = 3,
                        UploadedDateTime = DateTime.MinValue,
                        DisplayUploadedDateTime = "fake-uploaded-at",
                        PublishedBy = string.Empty,
                        PublishedDateTime = DateTime.MinValue,
                        DisplayPublishedDateTime = "fake-uploaded-at",
                        PreviousVersions = new List<AgencyExchangeDocument>
                        {
                            new AgencyExchangeDocument
                            {
                                FileName = "filename-1.pdf",
                                BatchIdentifier = "batch-id-1",
                                ParentBatchIdentifier = "parent-batch-id-1",
                                ProductName = "product-name-1",
                                Status = DownloadDocumentStatus.New,
                                StatusDateTime = new DateTime(2020, 12, 25, 1, 2, 3, 4),
                                DownloadedBy = "fake-user",
                                DisplayStatusDateTime = "fake-downloaded-at",
                                ProviderName = "org-name-1",
                                ProviderUkprn = "org-ukprn-1",
                                Versions = "3, 2, 1",
                                VersionsWithoutHyperlinks = "3, 2, 1",
                                VersionWithHyperlink = "1",
                                Version = "View version 1",
                                VersionNumber = 1,
                                UploadedDateTime = DateTime.MinValue,
                                DisplayUploadedDateTime = "fake-uploaded-at",
                                PublishedBy = string.Empty,
                                PublishedDateTime = DateTime.MinValue,
                                DisplayPublishedDateTime = "fake-uploaded-at"
                            },
                            new AgencyExchangeDocument
                            {
                                FileName = "filename-2.pdf",
                                BatchIdentifier = "batch-id-2",
                                ParentBatchIdentifier = "parent-batch-id-2",
                                ProductName = "product-name-2",
                                Status = DownloadDocumentStatus.New,
                                StatusDateTime = new DateTime(2020, 12, 25, 1, 2, 3, 4),
                                DownloadedBy = "fake-user",
                                DisplayStatusDateTime = "fake-downloaded-at",
                                ProviderName = "org-name-2",
                                ProviderUkprn = "org-ukprn-2",
                                Versions = "3, 2, 1",
                                VersionsWithoutHyperlinks = "3, 2, 1",
                                VersionWithHyperlink = "1",
                                Version = "View version 1",
                                VersionNumber = 2,
                                UploadedDateTime = DateTime.MinValue,
                                DisplayUploadedDateTime = "fake-uploaded-at",
                                PublishedBy = string.Empty,
                                PublishedDateTime = DateTime.MinValue,
                                DisplayPublishedDateTime = "fake-uploaded-at"
                            }
                        }
                    }
                };
            }
        }

        private DocumentModelConverter GetTestConverter()
            => new DocumentModelConverter(
                _dateTimeDisplayHelper,
                _documentStatusProvider,
                _documentVersionRenderer);
    }
}