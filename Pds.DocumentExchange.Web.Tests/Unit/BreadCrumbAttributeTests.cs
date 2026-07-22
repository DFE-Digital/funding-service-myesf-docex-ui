using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.Core.Web.Models.Hyperlinks;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Agency;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Organisation;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Shared;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs.SupportTools;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.Models.Agency;
using Pds.DocumentExchange.Web.Models.Organisation;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Tests.Unit
{
    [TestClass]
    [TestCategory("Unit")]
    public class BreadCrumbAttributeTests
    {
        [TestMethod]
        public void HomeBreadCrumbAttribute_GetPath_ReturnsEmptyList()
        {
            // Arrange
            var breadCrumb = new HomeBreadCrumbAttribute();

            // Act
            var breadCrumbs = breadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEmpty();
        }

        [TestMethod]
        public void LandingBreadCrumbAttribute_GetPath_WhenNotUsingServiceStartPage_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = landingBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());

            landingBreadCrumb.ViewModel.Link.Should().BeEquivalentTo(new MvcActionLinkViewModel
            {
                ActionName = nameof(DocumentExchangeController.Landing),
                ControllerName = NameOf<DocumentExchangeController>(),
                LinkText = ServiceConstants.ServiceName
            });
        }

        [TestMethod]
        public void LandingBreadCrumbAttribute_GetPath_WhenUsingServiceStartPage_ReturnsEmptyList()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(true, true);

            // Act
            var breadCrumbs = landingBreadCrumb.GetPath(new DTOs.BreadCrumbData { UsingServiceStartPage = true });

            // Assert
            breadCrumbs.Should().BeEmpty();

            landingBreadCrumb.ViewModel.Link.Should().BeEquivalentTo(new MvcActionLinkViewModel
            {
                ActionName = nameof(DocumentExchangeController.Landing),
                ControllerName = NameOf<DocumentExchangeController>(),
                LinkText = "Home"
            });
        }

        [TestMethod]
        public void TermsAndConditionsBreadCrumbAttribute_GetPath_WhenNotUsingServiceStartPage_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var tncBreadCrumb = new TermsAndConditionsBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = tncBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    tncBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void TermsAndConditionsBreadCrumbAttribute_GetPath_WhenUsingServiceStartPage_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false, true);
            var tncBreadCrumb = new TermsAndConditionsBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = tncBreadCrumb.GetPath(new DTOs.BreadCrumbData
            {
                UsingServiceStartPage = true
            });

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    landingBreadCrumb.ViewModel,
                    tncBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void Agency_FileShareBreadCrumbAttribute_GetPath_WhenNotUsingServiceStartPage_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var fileShareBreadCrumb = new FileShareBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = fileShareBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    fileShareBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void Agency_FileShareBreadCrumbAttribute_GetPath_WhenUsingServiceStartPage_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false, true);
            var fileShareBreadCrumb = new FileShareBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = fileShareBreadCrumb.GetPath(new DTOs.BreadCrumbData
            {
                UsingServiceStartPage = true
            });

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    landingBreadCrumb.ViewModel,
                    fileShareBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void Agency_DocumentsToPublishBreadCrumbAttribute_GetPath_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var fileShareBreadCrumb = new FileShareBreadCrumbAttribute(false);
            var documentsToPublishBreadCrumb = new DocumentsToPublishBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = documentsToPublishBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    fileShareBreadCrumb.ViewModel,
                    documentsToPublishBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void Agency_DocumentsToReviewBreadCrumbAttribute_GetPath_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var fileShareBreadCrumb = new FileShareBreadCrumbAttribute(false);
            var documentsToReviewBreadCrumb = new DocumentsToReviewBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = documentsToReviewBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    fileShareBreadCrumb.ViewModel,
                    documentsToReviewBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void Agency_DownloadDocumentsBreadCrumbAttribute_GetPath_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var downloadDocumentsBreadCrumb = new DownloadDocumentsBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = downloadDocumentsBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    downloadDocumentsBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void Agency_DeleteDocumentsAreYouSureBreadCrumbAttribute_GetPath_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var downloadDocumentsBreadCrumb = new DownloadDocumentsBreadCrumbAttribute(false);
            var deleteDocumentsAreYouSureBreadCrumb = new DeleteDocumentsAreYouSureBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = deleteDocumentsAreYouSureBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    downloadDocumentsBreadCrumb.ViewModel,
                    deleteDocumentsAreYouSureBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void Agency_RemoveDocumentsAreYouSureBreadCrumbAttribute_GetPath_ViaPublishPage_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var fileShareBreadCrumb = new FileShareBreadCrumbAttribute(false);
            var documentsToPublishBreadCrumb = new DocumentsToPublishBreadCrumbAttribute(false);
            var removeDocumentsAreYouSureBreadCrumb = new RemoveDocumentsAreYouSureBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = removeDocumentsAreYouSureBreadCrumb.GetPath(
                new DTOs.BreadCrumbData
                {
                    PageViewModel = new RemoveDocuments
                    {
                        EntryAction = nameof(AgencyController.DocumentsToPublish)
                    }
                });

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    fileShareBreadCrumb.ViewModel,
                    documentsToPublishBreadCrumb.ViewModel,
                    removeDocumentsAreYouSureBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void Agency_RemoveDocumentsAreYouSureBreadCrumbAttribute_GetPath_ViaReviewPage_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var fileShareBreadCrumb = new FileShareBreadCrumbAttribute(false);
            var documentsToReviewBreadCrumb = new DocumentsToReviewBreadCrumbAttribute(false);
            var removeDocumentsAreYouSureBreadCrumb = new RemoveDocumentsAreYouSureBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = removeDocumentsAreYouSureBreadCrumb.GetPath(
                new DTOs.BreadCrumbData
                {
                    PageViewModel = new RemoveDocuments
                    {
                        EntryAction = nameof(AgencyController.DocumentsToReview)
                    }
                });

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    fileShareBreadCrumb.ViewModel,
                    documentsToReviewBreadCrumb.ViewModel,
                    removeDocumentsAreYouSureBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void Agency_PublishDocumentsAreYouSureBreadCrumbAttribute_GetPath_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var fileShareBreadCrumb = new FileShareBreadCrumbAttribute(false);
            var documentsToPublishBreadCrumb = new DocumentsToPublishBreadCrumbAttribute(false);
            var publishDocumentsAreYouSureBreadCrumb = new PublishDocumentsAreYouSureBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = publishDocumentsAreYouSureBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    fileShareBreadCrumb.ViewModel,
                    documentsToPublishBreadCrumb.ViewModel,
                    publishDocumentsAreYouSureBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void Organisation_SelectAnOrganisationBreadCrumbAttribute_GetPath_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var selectAnOrganisationBreadCrumb = new SelectAnOrganisationBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = selectAnOrganisationBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    selectAnOrganisationBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void Organisation_SendYourDocumentBreadCrumbAttribute_GetPath_ViaLandingPage_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var sendYourDocumentBreadCrumb = new SendYourDocumentBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = sendYourDocumentBreadCrumb.GetPath(
                new DTOs.BreadCrumbData
                {
                    PageViewModel = new SendYourDocument
                    {
                        ShowParentView = false
                    }
                });

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    sendYourDocumentBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void Organisation_SendYourDocumentBreadCrumbAttribute_GetPath_ViaSelectAnOrganisationPage_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var selectAnOrganisationBreadCrumb = new SelectAnOrganisationBreadCrumbAttribute(false);
            var sendYourDocumentBreadCrumb = new SendYourDocumentBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = sendYourDocumentBreadCrumb.GetPath(
                new DTOs.BreadCrumbData
                {
                    PageViewModel = new SendYourDocument
                    {
                        ShowParentView = true
                    }
                });

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    selectAnOrganisationBreadCrumb.ViewModel,
                    sendYourDocumentBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void Organisation_SentDocumentsBreadCrumbAttribute_GetPath_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var sentDocumentsBreadCrumb = new SentDocumentsBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = sentDocumentsBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    sentDocumentsBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void Organisation_ReceivedDocumentsBreadCrumbAttribute_GetPath_WhenNotUsingServiceStartPage_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var receivedDocumentsBreadCrumb = new ReceivedDocumentsBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = receivedDocumentsBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    receivedDocumentsBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void Organisation_ReceivedDocumentsBreadCrumbAttribute_GetPath_WhenUsingServiceStartPage_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false, true);
            var receivedDocumentsBreadCrumb = new ReceivedDocumentsBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = receivedDocumentsBreadCrumb.GetPath(new DTOs.BreadCrumbData
            {
                UsingServiceStartPage = true
            });

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    landingBreadCrumb.ViewModel,
                    receivedDocumentsBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void DfE_DocumentsPublishedByDfEBreadCrumbAttribute_GetPath_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var supportToolsBreadCrum = new SupportToolsBreadCrumbAttribute(false);
            var documentsPublishedByDfeBreadCrumb = new DocumentsPublishedByDfeBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = documentsPublishedByDfeBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    supportToolsBreadCrum.ViewModel,
                    documentsPublishedByDfeBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void SupportToolsBreadCrumbAttribute_GetPath_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var supportToolsBreadCrumb = new SupportToolsBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = supportToolsBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    supportToolsBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void SupportTools_DocumentsPublishedByDfEBreadCrumbAttribute_GetPath_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var supportToolsBreadCrum = new SupportToolsBreadCrumbAttribute(false);
            var documentsPublishedByDfeBreadCrumb = new DocumentsPublishedByDfeBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = documentsPublishedByDfeBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    supportToolsBreadCrum.ViewModel,
                    documentsPublishedByDfeBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void SupportTools_DeleteDocumentsBreadCrumbAttribute_GetPath_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var supportToolsBreadCrumb = new SupportToolsBreadCrumbAttribute(false);
            var deleteDocumentsBreadCrumb = new DeleteDocumentBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = deleteDocumentsBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    supportToolsBreadCrumb.ViewModel,
                    deleteDocumentsBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void Agency_DeleteDocumentsSelectVersionBreadCrumb_GetPath_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var supportToolsBreadCrumb = new SupportToolsBreadCrumbAttribute(false);
            var deleteDocumentsBreadCrumb = new DeleteDocumentBreadCrumbAttribute(false);
            var deleteDocumentsSelectVersionBreadCrumb = new DeleteDocumentsSelectVersionBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = deleteDocumentsSelectVersionBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    supportToolsBreadCrumb.ViewModel,
                    deleteDocumentsBreadCrumb.ViewModel,
                    deleteDocumentsSelectVersionBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void Agency_DeletePublishedDocumentAreYouSureBreadCrumb_GetPath_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var supportToolsBreadCrumb = new SupportToolsBreadCrumbAttribute(false);
            var deleteDocumentsBreadCrumb = new DeleteDocumentBreadCrumbAttribute(false);
            var deletePublishedDocumentAreYouSureBreadCrumb = new DeletePublishedDocumentAreYouSureBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = deletePublishedDocumentAreYouSureBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    supportToolsBreadCrumb.ViewModel,
                    deleteDocumentsBreadCrumb.ViewModel,
                    deletePublishedDocumentAreYouSureBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void SupportTools_ReportsBreadCrumbAttribute_GetPath_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var supportToolsBreadCrumb = new SupportToolsBreadCrumbAttribute(false);
            var reportsBreadCrumb = new ReportsBreadCrumbAttribute(true);

            // Act
            var breadCrumbs = reportsBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    supportToolsBreadCrumb.ViewModel,
                    reportsBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }

        [TestMethod]
        public void SupportTools_ManagementInformationReportBreadCrumbAttribute_GetPath_ReturnsExpectedBreadCrumbs()
        {
            // Arrange
            var homeBreadCrumb = new HomeBreadCrumbAttribute();
            var landingBreadCrumb = new LandingBreadCrumbAttribute(false);
            var supportToolsBreadCrumb = new SupportToolsBreadCrumbAttribute(false);
            var reportsBreadCrumb = new ReportsBreadCrumbAttribute(false);
            var managementInformationReportBreadCrumb = new ManagementInformationReportAttribute(true);

            // Act
            var breadCrumbs = managementInformationReportBreadCrumb.GetPath(new DTOs.BreadCrumbData());

            // Assert
            breadCrumbs.Should().BeEquivalentTo(
                new[]
                {
                    homeBreadCrumb.ViewModel,
                    landingBreadCrumb.ViewModel,
                    supportToolsBreadCrumb.ViewModel,
                    reportsBreadCrumb.ViewModel,
                    managementInformationReportBreadCrumb.ViewModel
                },
                opt => opt.WithStrictOrdering());
        }
    }
}