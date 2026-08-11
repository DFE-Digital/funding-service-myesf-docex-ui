using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Extensions;
using Pds.DocumentExchange.Web.Interfaces.Coordinators;
using Pds.DocumentExchange.Web.Models;
using Pds.DocumentExchange.Web.Models.Agency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;
using Product = Pds.DocumentExchange.Services.Models.Product;

namespace Pds.DocumentExchange.Web.Tests.Unit.Controllers
{
    [TestClass]
    [TestCategory("Unit")]
    public class AgencyControllerTests : BaseControllerUnitTests
    {
        private const string FakeMimeTypeString = "application/fake";

        private readonly IAgencySummaryRequestCoordinator _agencySummaryCoordinator
            = Mock.Of<IAgencySummaryRequestCoordinator>(MockBehavior.Strict);

        private readonly IAgencyFileShareListRequestCoordinator _agencyFileShareListRequestCoordinator
            = Mock.Of<IAgencyFileShareListRequestCoordinator>(MockBehavior.Strict);

        private readonly IAgencyExchangeListRequestCoordinator _agencyExchangeListRequestCoordinator
            = Mock.Of<IAgencyExchangeListRequestCoordinator>(MockBehavior.Strict);

        private readonly IAgencyExchangeDeletionRequestCoordinator _agencyDeletionRequestCoordinator
            = Mock.Of<IAgencyExchangeDeletionRequestCoordinator>(MockBehavior.Strict);

        private readonly IDocumentReferenceService _documentReferenceService
            = Mock.Of<IDocumentReferenceService>(MockBehavior.Strict);

        #region AgencyHome

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 0)]
        [DataRow(0, 1)]
        [DataRow(1, 1)]
        [DataRow(12, 24)]
        [DataRow(145, 87)]
        [DataRow(245, 142)]
        [DataRow(24, 26)]
        public async Task AgencyHome_ForTeamsUser_ReturnsExpectedTiles(
            int fileShareCount, int newDocuments)
        {
            // Arrange
            var expected = new AgencyHomePageData
            {
                CountOfNewDocuments = newDocuments,
                TotalCountOfDocumentsInFileShare = fileShareCount,
                ShowDocumentOptions = true
            };

            Mock.Get(_agencySummaryCoordinator)
                .Setup(a => a.GetHomePageData())
                .ReturnsAsync(expected);

            var controller = GetAgencyController();

            // Act
            var result = await controller.AgencyHome();

            // Assert
            var tiles = result.Should().BeViewResult()
                            .Model.Should().BeOfType<AgencyHome>()
                            .Which.Tiles;

            tiles.Should().HaveCount(2);

            tiles.First().Title.Should().Contain("Publish");
            tiles.First().AlertText.Should().Contain($"{fileShareCount}");

            tiles.Last().Title.Should().Contain("Download");
            tiles.Last().AlertText.Should().Contain($"{newDocuments}");

            Mock.VerifyAll(
                Mock.Get(_agencySummaryCoordinator));
        }

        [TestMethod]
        [DataRow(false, false, false)]
        [DataRow(false, true, false)]
        [DataRow(true, false, false)]
        [DataRow(true, true, true)]
        public async Task AgencyHome_ForViewAsProvider_ReturnsExpectedTiles(
            bool userHasViewAsProvider, bool configuredForServiceStartPage, bool expectedShouldShowTile)
        {
            // Arrange
            var coordinatorResult = new AgencyHomePageData
            {
                ShowViewAsOrganisation = userHasViewAsProvider
            };

            Mock.Get(_agencySummaryCoordinator)
                .Setup(a => a.GetHomePageData())
                .ReturnsAsync(coordinatorResult);

            var config = new DocumentExchangeConfiguration
            {
                ShowServiceStartPage = configuredForServiceStartPage
            };

            var controller = GetAgencyController(config);

            // Act
            var result = await controller.AgencyHome();

            // Assert
            var tiles = result.Should().BeViewResult()
                             .Model.Should().BeOfType<AgencyHome>()
                             .Which.Tiles;

            tiles.Should().HaveCount(expectedShouldShowTile ? 1 : 0);

            if (expectedShouldShowTile)
            {
                tiles.First().Title.Should().Contain("View as an organisation");
            }

            Mock.VerifyAll(
                Mock.Get(_agencySummaryCoordinator));
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task AgencyHome_ForAdmin_ReturnsExpectedTiles(bool userIsAdmin)
        {
            // Arrange
            var coordinatorResult = new AgencyHomePageData
            {
                ShowSettingsOption = userIsAdmin
            };

            Mock.Get(_agencySummaryCoordinator)
                .Setup(a => a.GetHomePageData())
                .ReturnsAsync(coordinatorResult);

            var controller = GetAgencyController();

            // Act
            var result = await controller.AgencyHome();

            // Assert
            var tiles = result.Should().BeViewResult()
                            .Model.Should().BeOfType<AgencyHome>()
                            .Which.Tiles;

            tiles.Should().HaveCount(userIsAdmin ? 1 : 0);

            if (userIsAdmin)
            {
                tiles.First().Title.Should().Contain("settings");
            }

            Mock.VerifyAll(
                Mock.Get(_agencySummaryCoordinator));
        }

        [TestMethod]
        public async Task AgencyHome_ForTeamsUserWithAdminAndViewAsProvider_ReturnsExpectedTiles()
        {
            // Arrange
            var expected = new AgencyHomePageData
            {
                TotalCountOfDocumentsInFileShare = 123,
                CountOfNewDocuments = 456,
                ShowDocumentOptions = true,
                ShowSettingsOption = true,
                ShowViewAsOrganisation = true,
                ShowToolsOption = false
            };

            Mock.Get(_agencySummaryCoordinator)
                .Setup(a => a.GetHomePageData())
                .ReturnsAsync(expected);

            var controller = GetAgencyController(new DocumentExchangeConfiguration
            {
                ShowServiceStartPage = true
            });

            // Act
            var result = await controller.AgencyHome();

            // Assert
            var tiles = result.Should().BeViewResult()
                            .Model.Should().BeOfType<AgencyHome>()
                            .Which.Tiles;

            tiles.Should().HaveCount(4);

            var tilesArr = tiles.ToArray();

            tilesArr[0].Title.Should().Contain("Publish");
            tilesArr[0].AlertText.Should().Contain("123");

            tilesArr[1].Title.Should().Contain("Download");
            tilesArr[1].AlertText.Should().Contain("456");

            tilesArr[2].Title.Should().Contain("settings");

            tilesArr[3].Title.Should().Contain("View as an organisation");

            Mock.VerifyAll(
                Mock.Get(_agencySummaryCoordinator));
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task AgencyHome_ForShowingToolOptions_ReturnsExpectedTiles(bool showToolOptions)
        {
            // Arrange
            var expected = new AgencyHomePageData
            {
                ShowToolsOption = showToolOptions
            };

            Mock.Get(_agencySummaryCoordinator)
                .Setup(a => a.GetHomePageData())
                .ReturnsAsync(expected);

            var controller = GetAgencyController(new DocumentExchangeConfiguration
            {
                ShowServiceStartPage = true
            });

            // Act
            var result = await controller.AgencyHome();

            // Assert
            var tiles = result.Should().BeViewResult()
                            .Model.Should().BeOfType<AgencyHome>()
                            .Which.Tiles;

            tiles.Should().HaveCount(showToolOptions ? 1 : 0);

            if (showToolOptions)
            {
                tiles.First().Title.Should().Contain("support");
            }

            Mock.VerifyAll(
                Mock.Get(_agencySummaryCoordinator));
        }
        #endregion


        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 291)]
        [DataRow(145, 53)]
        [DataRow(235, 64)]
        [DataRow(234, 46)]
        public async Task FileShare_MakesExpectedApiCallsAndReturnsExpectedView(int invalidCount, int validCount)
        {
            // Arrange
            var expected = new FileShare
            {
                TotalCountOfDocuments = invalidCount + validCount,
                CountOfInvalidDocuments = invalidCount,
                CountOfValidDocuments = validCount
            };

            Mock.Get(_agencySummaryCoordinator)
                .Setup(a => a.GetFileSharePage())
                .ReturnsAsync(expected);

            var controller = GetAgencyController();

            // Act
            var result = await controller.FileShare();

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<FileShare>()
                .Which.Should().Be(expected);

            Mock.VerifyAll(
                Mock.Get(_agencySummaryCoordinator));
        }

        #region DownloadDocument Tests

        [TestMethod]
        public async Task DownloadDocument_ForOtherTeam_IsUnauthorized()
        {
            // Arrange
            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync("my-team");

            var controller = GetAgencyController();

            // Act
            var result = await controller.DownloadDocument("not-my-team", FakeString);

            // Assert
            result.Should().BeUnauthorizedResult();

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        [TestMethod]
        [DataRow("test content 1")]
        [DataRow("test content 2")]
        [DataRow("test content 3")]
        public async Task DownloadDocument_ReturnsExpectedFileResult(string fileContent)
        {
            // Arrange
            var fileContentBytes = Encoding.UTF8.GetBytes(fileContent);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync("my-team");

            Mock.Get(AgencyApiClient)
                .Setup(a => a.DownloadTeamDocument(
                    "my-team",
                    FakeString))
                .ReturnsAsync(fileContentBytes);

            Mock.Get(MimeMappingService)
                .Setup(m => m.GetContentType(FakeString))
                .Returns(FakeMimeTypeString);

            var controller = GetAgencyController();

            // Act
            var result = await controller.DownloadDocument("my-team", FakeString);

            // Assert
            result.Should().BeFileContentResult()
                .WithFileDownloadName(FakeString)
                .WithContentType(FakeMimeTypeString);

            ((FileContentResult)result).FileContents.Should().BeEquivalentTo(fileContentBytes);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(AgencyApiClient),
                Mock.Get(MimeMappingService));
        }

        #endregion


        #region DownloadExchangeDocument Tests

        [TestMethod]
        [DataRow("test content 1")]
        [DataRow("test content 2")]
        [DataRow("test content 3")]
        public async Task DownloadExchangeDocument_ReturnsExpectedFileResult(string fileContent)
        {
            // Arrange
            var userInfo = TestAgencyUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserInfo())
                .ReturnsAsync(userInfo);

            var fakeFileName = "fakeFileName.pdf";
            var fakeDocumentReference = $"{fakeFileName}|fakeBatchIdentifier|fakeParentBatchIdentifier";

            var fileContentBytes = Encoding.UTF8.GetBytes(fileContent);

            var file = new DownloadedFile
            {
                Name = fakeFileName,
                ContentType = FakeMimeTypeString,
                Content = fileContentBytes
            };

            Mock.Get(ExchangeDocumentDownloadService)
                .Setup(a => a.DownloadExchangeDocumentByReference(fakeDocumentReference, userInfo))
                .ReturnsAsync(file);

            var controller = GetAgencyController();

            // Act
            var result = await controller.DownloadExchangeDocument(fakeDocumentReference);

            // Assert
            result.Should().BeFileContentResult()
                .WithFileDownloadName(fakeFileName)
                .WithContentType(FakeMimeTypeString);

            ((FileContentResult)result).FileContents.Should().BeEquivalentTo(fileContentBytes);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(ExchangeDocumentDownloadService));
        }

        [TestMethod]
        public async Task DownloadExchangeDocumentList_WhenDocumentReferenceListContainsItems_DownloadsFile()
        {
            // Arrange
            var userInfo = TestAgencyUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserInfo())
                .ReturnsAsync(userInfo);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync("team");

            var fileName = "fileName.pdf";
            var documentReference = $"{fileName}|batchIdentifier|parentBatchIdentifier";
            var documentReferences = new[] { documentReference };

            var documentReferenceList = new DocumentReferenceList
            {
                DocumentReferences = documentReferences
            };

            var fileContentBytes = new byte[] { 1, 2, 3, 4, 5 };

            var file = new DownloadedFile
            {
                Name = fileName,
                ContentType = FakeMimeTypeString,
                Content = fileContentBytes
            };

            Mock.Get(ExchangeDocumentDownloadService)
                .Setup(a => a.DownloadExchangeDocumentsByReferences(documentReferences, userInfo))
                .ReturnsAsync(file);

            Mock.Get(_documentReferenceService)
                .Setup(service => service.CreateDocumentReferenceFromString(It.IsAny<string>()))
                .Returns(new DocumentReference { FileName = fileName, BatchIdentifier = "batchIdentifier", ParentBatchIdentifier = "parentBatchIdentifier" });

            Mock.Get(_documentReferenceService)
                .Setup(service => service.CreateDocumentReferencesWithPreviousVersionsFromStrings(It.IsAny<IEnumerable<string>>()))
                .Returns(Enumerable.Empty<DocumentReferenceWithPreviousVersions>());

            Mock.Get(AgencyApiClient)
                .Setup(apiClient => apiClient.GetPreviousDocumentVersionReferences(It.IsAny<string>(), It.IsAny<IEnumerable<DocumentReference>>()))
                .ReturnsAsync(Enumerable.Empty<DocumentReference>());

            var controller = GetAgencyController();

            // Act
            var result = await controller.DownloadExchangeDocumentList(documentReferenceList);

            // Assert
            result.Should().BeFileContentResult()
                .WithFileDownloadName(fileName)
                .WithContentType(FakeMimeTypeString);

            ((FileContentResult)result).FileContents.Should().BeEquivalentTo(fileContentBytes);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(ExchangeDocumentDownloadService));
        }

        #endregion


        [TestMethod]
        public async Task DownloadExchangeDocumentList_WhenNoDocumentReferencesPassed_RedirectsToDownloadDocumentsPage()
        {
            // Arrange
            var controller = GetAgencyController();

            // Act
            var result = await controller.DownloadExchangeDocumentList(new DocumentReferenceList());

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(AgencyController.DownloadDocuments))
                .WithControllerName(NameOf<AgencyController>())
                .WithRouteValue("error", true)
                .WithRouteValue("errorAction", nameof(AgencyController.DownloadExchangeDocumentList));
        }

        [TestMethod]
        public async Task DownloadExchangeDocumentsByFilters_ReturnsFileBuiltFromCoordinatorResult()
        {
            // Arrange
            var listRequest = new ListRequest();

            var fileName = "fileName.pdf";
            var fileContentBytes = new byte[] { 1, 2, 3, 4, 5 };

            var file = new DownloadedFile
            {
                Name = fileName,
                ContentType = FakeMimeTypeString,
                Content = fileContentBytes
            };

            var mockCoordinator = Mock.Get(_agencyExchangeListRequestCoordinator);

            mockCoordinator
                .Setup(a => a.DownloadDocuments(listRequest))
                .ReturnsAsync(file);

            var controller = GetAgencyController();

            // Act
            var result = await controller.DownloadExchangeDocumentsByFilters(listRequest);

            // Assert
            result.Should().BeFileContentResult()
                .WithFileDownloadName(fileName)
                .WithContentType(FakeMimeTypeString);

            ((FileContentResult)result).FileContents.Should().BeEquivalentTo(fileContentBytes);

            Mock.VerifyAll(mockCoordinator);
        }

        #region Publish Documents Journey Tests

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 91)]
        [DataRow(15, 43)]
        [DataRow(23, 66)]
        [DataRow(34, 42)]
        public async Task DocumentsToPublish_ForNullParameter_MakesExpectedApiCallsAndReturnsExpectedView(
          int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var expectedResult = new DocumentsToPublish
            {
                ListItems = GetTestFileShareDocuments(numberOfPages),
                FilterCategories = GetTestViewModelFilterCategories(numberOfFilters).ToList(),
                Pagination = new PaginationViewModel
                {
                    TotalItems = expectedNumberOfItems,
                    TotalPages = numberOfPages,
                    Page = 1,
                    PageSize = TestConfiguration.ListPageSize
                },
                AnyDocumentsAvailable = expectedNumberOfItems > 0
            };

            Mock.Get(_agencyFileShareListRequestCoordinator)
                .Setup(c => c.GetDocumentsToPublish(null))
                .ReturnsAsync(expectedResult);

            var controller = GetAgencyController();

            // Act
            var result = await controller.DocumentsToPublish(null);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<DocumentsToPublish>()
                .Which.Should().BeEquivalentTo(
                    expectedResult,
                    options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(_agencyFileShareListRequestCoordinator));
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 29)]
        public async Task DocumentsToPublish_ForParameter_MakesExpectedApiCallsAndReturnsExpectedView(
            int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var expectedResult = new DocumentsToPublish
            {
                ListItems = GetTestFileShareDocuments(numberOfPages),
                FilterCategories = GetTestViewModelFilterCategories(numberOfFilters).ToList(),
                Pagination = new PaginationViewModel
                {
                    TotalItems = expectedNumberOfItems,
                    TotalPages = numberOfPages,
                    Page = 1,
                    PageSize = TestConfiguration.ListPageSize
                },
                AnyDocumentsAvailable = expectedNumberOfItems > 0
            };

            var request = new DocumentsRequest
            {
                Error = true,
                ErrorAction = FakeString
            };

            Mock.Get(_agencyFileShareListRequestCoordinator)
                .Setup(c => c.GetDocumentsToPublish(request))
                .ReturnsAsync(expectedResult);

            var controller = GetAgencyController();

            // Act
            var result = await controller.DocumentsToPublish(request);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<DocumentsToPublish>()
                .Which.Should().BeEquivalentTo(
                    expectedResult,
                    options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(_agencyFileShareListRequestCoordinator));
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 91)]
        [DataRow(15, 43)]
        [DataRow(23, 66)]
        [DataRow(34, 42)]
        public async Task DocumentsToPublishData_ForNullParameter_MakesExpectedApiCallsAndReturnsExpectedView(
          int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var expectedResult = new DocumentListUpdateData
            {
                ListItems = GetTestFileShareDocuments(numberOfPages),
                Pagination = GetExpectedPaginationUpdateData(expectedNumberOfItems, numberOfPages)
            };

            Mock.Get(_agencyFileShareListRequestCoordinator)
                .Setup(c => c.GetDocumentsToPublishData(null))
                .ReturnsAsync(expectedResult);

            var controller = GetAgencyController();

            // Act
            var result = await controller.DocumentsToPublishData(null);

            // Assert
            result.Should().BeJsonResult()
                .Value.Should().BeOfType<DocumentListUpdateData>()
                .Which.Should().BeEquivalentTo(expectedResult);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(_agencyFileShareListRequestCoordinator));
        }

        [TestMethod]
        [DataRow(1, 1001, "product 1", "team 1")]
        [DataRow(2, 1002, "product 2", "team 2")]
        [DataRow(5, 1003, "product 3", "team 3")]
        [DataRow(10, 1004, "product 4", "team 4")]
        [DataRow(100, 1005, "product 5", "team 5")]
        public async Task PublishDocumentsAreYouSure_WhenDocumentsExist_MakesExpectedApiCallAndReturnsExpectedView(
            int count, int productId, string productName, string team)
        {
            // Arrange
            var documentsToPublish = new DocumentsToPublish
            {
                Pagination = new PaginationViewModel
                {
                    TotalItems = count
                },
                SelectedTeam = team,
                SelectedProduct = new Models.Shared.Product
                {
                    Identifier = productId,
                    Name = productName
                }
            };

            Mock.Get(_agencyFileShareListRequestCoordinator)
                .Setup(c => c.GetDocumentsToPublish(
                    It.IsAny<DocumentsRequest>()))
                .ReturnsAsync(documentsToPublish);

            var controller = GetAgencyController();

            // Act
            var result = await controller.PublishDocumentsAreYouSure(null);

            // Assert
            result.Should()
                .BeViewResult()
                .Model.Should()
                .BeOfType<PublishDocumentsAreYouSure>()
                .Which.Should()
                .BeEquivalentTo(
                    new PublishDocumentsAreYouSure
                    {
                        Count = count,
                        Product = documentsToPublish.SelectedProduct,
                        SelectedProductId = productId,
                        SelectedTeam = team
                    });

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(_agencyFileShareListRequestCoordinator));
        }

        [TestMethod]
        public async Task PublishDocumentsAreYouSure_WhenNoDocumentsExist_RedirectsToListPage()
        {
            // Arrange
            var documentsToPublish = new DocumentsToPublish
            {
                Pagination = new PaginationViewModel
                {
                    TotalItems = 0
                }
            };

            Mock.Get(_agencyFileShareListRequestCoordinator)
                .Setup(c => c.GetDocumentsToPublish(
                    It.IsAny<DocumentsRequest>()))
                .ReturnsAsync(documentsToPublish);

            var controller = GetAgencyController();

            // Act
            var result = await controller.PublishDocumentsAreYouSure(null);

            // Assert
            result.Should()
                .BeRedirectToActionResult()
                .WithActionName(nameof(AgencyController.DocumentsToPublish));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(_agencyFileShareListRequestCoordinator));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task PublishDocumentsConfirmation_WhenConfirmationNotChosen_RedirectsToAreYouSurePage(bool nullParameter)
        {
            // Arrange
            var controller = GetAgencyController();

            // Act
            var result = await controller.PublishDocumentsConfirmation(
                nullParameter
                    ? null
                    : new PublishDocumentsConfirmation
                    {
                        PublishConfirmed = null
                    });

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithPreserveMethod(true)
                .WithActionName(nameof(AgencyController.PublishDocumentsAreYouSure))
                .WithControllerName(NameOf<AgencyController>())
                .WithRouteValue("error", true);
        }

        [TestMethod]
        public async Task PublishDocumentsConfirmation_WhenChosenToAbortPublish_RedirectsToDocumentsToPublishWithoutPublishingFiles()
        {
            // Arrange
            var controller = GetAgencyController();

            // Act
            var result = await controller.PublishDocumentsConfirmation(new PublishDocumentsConfirmation
            {
                PublishConfirmed = false,
                SelectedTeam = "team",
                SelectedProductId = 123
            });

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(AgencyController.DocumentsToPublish));
        }

        [TestMethod]
        public async Task PublishDocumentsConfirmation_WhenTeamNotSpecified_RedirectsToDocumentsToPublishWithoutPublishingFiles()
        {
            // Arrange
            var controller = GetAgencyController();

            // Act
            var result = await controller.PublishDocumentsConfirmation(new PublishDocumentsConfirmation
            {
                PublishConfirmed = true,
                SelectedTeam = string.Empty,
                SelectedProductId = 123
            });

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(AgencyController.DocumentsToPublish));
        }

        [TestMethod]
        public async Task PublishDocumentsConfirmation_WhenProductNotSpecified_RedirectsToDocumentsToPublishWithoutPublishingFiles()
        {
            // Arrange
            var controller = GetAgencyController();

            // Act
            var result = await controller.PublishDocumentsConfirmation(new PublishDocumentsConfirmation
            {
                PublishConfirmed = true,
                SelectedTeam = "team",
                SelectedProductId = default
            });

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(AgencyController.DocumentsToPublish));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(12)]
        [DataRow(125)]
        [DataRow(234)]
        public async Task PublishDocumentsConfirmation_ForOtherTeam_IsUnauthorized(int numberOfFileNames)
        {
            // Arrange
            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync("my-team");

            var controller = GetAgencyController();

            // Act
            var result = await controller.PublishDocumentsConfirmation(new PublishDocumentsConfirmation
            {
                PublishConfirmed = true,
                SelectedTeam = "not-my-team",
                SelectedProductId = 123
            });

            // Assert
            result.Should().BeUnauthorizedResult();
        }

        [TestMethod]
        [DataRow(1001, "product 1", 1)]
        [DataRow(1002, "product 2", 12)]
        [DataRow(1003, "product 3", 125)]
        [DataRow(1004, "product 4", 234)]
        public async Task PublishDocumentsConfirmation_WhenValidDataPassedAndChosenToConfirmPublish_PublishesFilesAndReturnsView(int productId, string productName, int count)
        {
            // Arrange
            var viewModel = new PublishDocumentsConfirmation
            {
                PublishConfirmed = true,
                SelectedTeam = "my-team",
                SelectedProductId = productId
            };

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync(viewModel.SelectedTeam);

            var testAgencyUserInfo = TestAgencyUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserInfo())
                .ReturnsAsync(testAgencyUserInfo);

            var product = new Product
            {
                Identifier = productId,
                Name = productName
            };

            Mock.Get(AgencyApiClient)
                .Setup(
                    client => client.PublishTeamDocuments(
                        viewModel.SelectedTeam,
                        It.Is<AgencyPublishRequest>(
                            req => req.ProductId == productId &&
                                   req.UserInfo == testAgencyUserInfo)))
                .ReturnsAsync(new KeyValuePair<Product, int>(product, count));

            var mappedProduct = new Models.Shared.Product
            {
                Identifier = productId,
                Name = productName
            };

            //Mock.Get(Mapper)
            //    .Setup(mapper => mapper.Map<Models.Shared.Product>(product))
            //    .Returns(mappedProduct);
            var controller = GetAgencyController();

            // Act
            var result = await controller.PublishDocumentsConfirmation(viewModel);

            // Assert
            var actualViewModel = result.Should()
                .BeViewResult()
                .Model.Should()
                .BeOfType<PublishDocumentsConfirmation>()
                .Which;

            actualViewModel.Count.Should().Be(count);
            actualViewModel.Product.Should().BeEquivalentTo(mappedProduct);
            actualViewModel.Should().BeEquivalentTo(viewModel);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(AgencyApiClient));
        }

        #endregion


        #region Review Documents Journey Tests

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 21)]
        [DataRow(25, 53)]
        [DataRow(35, 66)]
        [DataRow(34, 82)]
        public async Task DocumentsToReview_ForNullParameter_MakesExpectedApiCallsAndReturnsExpectedView(
            int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var expectedResult = new DocumentsToReview
            {
                ListItems = GetTestInvalidFileShareDocuments(numberOfPages),
                FilterCategories = GetTestViewModelFilterCategories(numberOfFilters).ToList(),
                Pagination = new PaginationViewModel
                {
                    TotalItems = expectedNumberOfItems,
                    TotalPages = numberOfPages,
                    Page = 1,
                    PageSize = TestConfiguration.ListPageSize
                },
                AnyDocumentsAvailable = expectedNumberOfItems > 0
            };

            Mock.Get(_agencyFileShareListRequestCoordinator)
                .Setup(c => c.GetDocumentsToReview(null))
                .ReturnsAsync(expectedResult);

            var controller = GetAgencyController();

            // Act
            var result = await controller.DocumentsToReview(null);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<DocumentsToReview>()
                .Which.Should().BeEquivalentTo(
                    expectedResult,
                    options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(_agencyFileShareListRequestCoordinator));
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 29)]
        [DataRow(123, 54)]
        [DataRow(235, 46)]
        [DataRow(234, 85)]
        public async Task DocumentsToReview_ForParameterWithValidationError_MakesExpectedApiCallsAndReturnsExpectedView(
            int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var expectedResult = new DocumentsToReview
            {
                ListItems = GetTestInvalidFileShareDocuments(numberOfPages),
                FilterCategories = GetTestViewModelFilterCategories(numberOfFilters).ToList(),
                Error = true,
                ErrorAction = FakeString,
                Pagination = new PaginationViewModel
                {
                    TotalItems = expectedNumberOfItems,
                    TotalPages = numberOfPages,
                    Page = 1,
                    PageSize = TestConfiguration.ListPageSize
                },
                AnyDocumentsAvailable = expectedNumberOfItems > 0
            };

            var request = new DocumentsRequest
            {
                Error = true,
                ErrorAction = FakeString
            };

            Mock.Get(_agencyFileShareListRequestCoordinator)
                .Setup(c => c.GetDocumentsToReview(request))
                .ReturnsAsync(expectedResult);

            var controller = GetAgencyController();

            // Act
            var result = await controller.DocumentsToReview(request);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<DocumentsToReview>()
                .Which.Should().BeEquivalentTo(
                    expectedResult,
                    options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(_agencyFileShareListRequestCoordinator));
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 91)]
        [DataRow(15, 43)]
        [DataRow(23, 66)]
        [DataRow(34, 42)]
        public async Task DocumentsToReviewData_ForNullParameter_MakesExpectedApiCallsAndReturnsExpectedView(
          int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var expectedResult = new DocumentListUpdateData
            {
                ListItems = GetTestInvalidFileShareDocuments(numberOfPages),
                Pagination = GetExpectedPaginationUpdateData(expectedNumberOfItems, numberOfPages)
            };

            Mock.Get(_agencyFileShareListRequestCoordinator)
                .Setup(c => c.GetDocumentsToReviewData(null))
                .ReturnsAsync(expectedResult);

            var controller = GetAgencyController();

            // Act
            var result = await controller.DocumentsToReviewData(null);

            // Assert
            result.Should().BeJsonResult()
                .Value.Should().BeOfType<DocumentListUpdateData>()
                .Which.Should().BeEquivalentTo(expectedResult);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(_agencyFileShareListRequestCoordinator));
        }

        [TestMethod]
        public async Task DocumentsToReviewReport_WhenNoFileNamesPassed_RedirectsToReviewPage()
        {
            // Arrange
            var controller = GetAgencyController();

            // Act
            var result = await controller.DocumentsToReviewReport(new FileShareDocumentReferenceList());

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(AgencyController.DocumentsToReview))
                .WithControllerName(NameOf<AgencyController>())
                .WithRouteValue("error", true)
                .WithRouteValue("errorAction", nameof(AgencyController.DocumentsToReviewReport));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(12)]
        [DataRow(123)]
        [DataRow(234)]
        [DataRow(345)]
        public async Task DocumentsToReviewReport_ForOtherTeam_IsUnauthorized(int numberOfDocumentReferences)
        {
            // Arrange
            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync("my-team");

            var controller = GetAgencyController();

            var expectedDocumentReferences = Enumerable.Range(1, numberOfDocumentReferences).Select(f => $"not-my-team::file{f}.pdf");

            // Act
            var result = await controller.DocumentsToReviewReport(new FileShareDocumentReferenceList
            {
                FileShareDocumentReferences = expectedDocumentReferences
            });

            // Assert
            result.Should().BeUnauthorizedResult();

            Mock.Verify(
                Mock.Get(UserInformationProvider));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(12)]
        [DataRow(123)]
        [DataRow(234)]
        [DataRow(345)]
        public void DocumentsToReviewReport_ForInvalidDocumentReferences_Throws(int numberOfDocumentReferences)
        {
            // Arrange
            var controller = GetAgencyController();

            var expectedDocumentReferences = Enumerable.Range(1, numberOfDocumentReferences).Select(f => $"invalid.{f}.pdf");

            // Act
            Func<Task> act = () => controller.DocumentsToReviewReport(new FileShareDocumentReferenceList
            {
                FileShareDocumentReferences = expectedDocumentReferences
            });

            // Assert
            act.Should().ThrowAsync<ArgumentException>();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(12)]
        [DataRow(123)]
        [DataRow(234)]
        [DataRow(345)]
        public async Task DocumentsToReviewReport_WhenFileNamesPassed_BuildsTheReport(int numberOfDocumentReferences)
        {
            // Arrange
            var team = "my-team";

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync(team);

            var controller = GetAgencyController();

            var expectedDocumentReferences = Enumerable.Range(1, numberOfDocumentReferences).Select(f => $"{team}::file{f}.pdf").ToList();
            var expectedFileContent = Encoding.UTF8.GetBytes("File content");

            var mockReportBuilder = Mock.Get(AgencyDocumentErrorReportBuilder);

            mockReportBuilder
                .Setup(e => e.BuildErrorReportSpreadsheet(
                    team,
                    It.Is<AgencyListDocumentOptions>(
                        d => d.Validity == AgencyDocumentValidity.Invalid
                            && d.PageSize == int.MaxValue
                            && d.DocumentNames.Count() == expectedDocumentReferences.Count
                            && d.DocumentNames.All(n => expectedDocumentReferences.Any(r => r.EndsWith(n))))))
                .ReturnsAsync(expectedFileContent);

            var expectedFileName = $"DocumentExchange_DocumentErrors_2020-12-25.ods";

            var mockMimeMapService = Mock.Get(MimeMappingService);

            mockMimeMapService
                .Setup(m => m.GetContentType(expectedFileName))
                .Returns(FakeMimeTypeString);

            var mockSystemProvider = Mock.Get(SystemProvider);

            mockSystemProvider
                .Setup(s => s.DateTime.UtcNow())
                .Returns(new DateTime(2020, 12, 25));

            // Act
            var result = await controller.DocumentsToReviewReport(new FileShareDocumentReferenceList
            {
                FileShareDocumentReferences = expectedDocumentReferences
            });

            // Assert
            result.Should().BeFileContentResult()
                .WithFileDownloadName(expectedFileName)
                .WithContentType(FakeMimeTypeString);

            ((FileContentResult)result).FileContents.Should().BeEquivalentTo(expectedFileContent);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                mockReportBuilder,
                mockMimeMapService,
                mockSystemProvider);
        }

        #endregion


        #region Download Documents Tests

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 29)]
        [DataRow(15, 53)]
        [DataRow(25, 66)]
        [DataRow(24, 82)]
        public async Task DownloadDocuments_ForNullParameter_MakesExpectedApiCallsAndReturnsExpectedView(
           int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var expectedResult = new DownloadDocuments
            {
                ListItems = GetTestDownloadFileShareDocuments(numberOfPages),
                FilterCategories = GetTestViewModelFilterCategories(numberOfFilters).ToList(),
                Pagination = new PaginationViewModel
                {
                    TotalItems = expectedNumberOfItems,
                    TotalPages = numberOfPages,
                    Page = 1,
                    PageSize = TestConfiguration.ListPageSize
                },
                AnyDocumentsAvailable = expectedNumberOfItems > 0
            };

            Mock.Get(_agencyExchangeListRequestCoordinator)
                .Setup(c => c.GetDocumentsToDownload(null))
                .ReturnsAsync(expectedResult);

            var controller = GetAgencyController();

            // Act
            var result = await controller.DownloadDocuments(null);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<DownloadDocuments>()
                .Which.Should().BeEquivalentTo(expectedResult, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(_agencyExchangeListRequestCoordinator));
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 21)]
        [DataRow(15, 53)]
        [DataRow(25, 66)]
        [DataRow(34, 82)]
        public async Task DownloadDocuments_ForAdvancedUser_MakesExpectedApiCallsAndReturnsExpectedView(
           int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var expectedResult = new DownloadDocuments
            {
                ListItems = GetTestDownloadFileShareDocuments(numberOfPages),
                FilterCategories = GetTestViewModelFilterCategories(numberOfFilters).ToList(),
                Pagination = new PaginationViewModel
                {
                    TotalItems = expectedNumberOfItems,
                    TotalPages = numberOfPages,
                    Page = 1,
                    PageSize = TestConfiguration.ListPageSize
                },
                AnyDocumentsAvailable = expectedNumberOfItems > 0,
                UserIsAdvancedAgencyUser = true
            };

            Mock.Get(_agencyExchangeListRequestCoordinator)
                .Setup(c => c.GetDocumentsToDownload(null))
                .ReturnsAsync(expectedResult);

            var controller = GetAgencyController();

            // Act
            var result = await controller.DownloadDocuments(null);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<DownloadDocuments>()
                .Which.Should().BeEquivalentTo(expectedResult, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(_agencyExchangeListRequestCoordinator));
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 29)]
        [DataRow(15, 53)]
        [DataRow(25, 66)]
        [DataRow(34, 82)]
        public async Task DownloadDocuments_WhenErrorDocumentsRequestHasError_MakesExpectedApiCallsAndReturnsExpectedViewWithError(
            int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var documentsRequestWithError = new DocumentsRequest
            {
                Error = true,
                ErrorAction = "the-error-action"
            };

            var expectedResult = new DownloadDocuments
            {
                ListItems = GetTestDownloadFileShareDocuments(numberOfPages),
                FilterCategories = GetTestViewModelFilterCategories(numberOfFilters).ToList(),
                Pagination = new PaginationViewModel
                {
                    TotalItems = expectedNumberOfItems,
                    TotalPages = numberOfPages,
                    Page = 1,
                    PageSize = TestConfiguration.ListPageSize
                },
                AnyDocumentsAvailable = expectedNumberOfItems > 0,
                Error = true,
                ErrorAction = documentsRequestWithError.ErrorAction
            };

            Mock.Get(_agencyExchangeListRequestCoordinator)
                .Setup(c => c.GetDocumentsToDownload(documentsRequestWithError))
                .ReturnsAsync(expectedResult);

            var controller = GetAgencyController();

            // Act
            var result = await controller.DownloadDocuments(documentsRequestWithError);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<DownloadDocuments>()
                .Which.Should().BeEquivalentTo(
                    expectedResult, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(_agencyExchangeListRequestCoordinator));
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 91)]
        [DataRow(15, 43)]
        [DataRow(23, 66)]
        [DataRow(34, 42)]
        public async Task DocumentsToDownloadData_ForNullParameter_MakesExpectedApiCallsAndReturnsExpectedView(
          int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var expectedResult = new DocumentListUpdateData
            {
                ListItems = GetTestDownloadFileShareDocuments(numberOfPages),
                Pagination = GetExpectedPaginationUpdateData(expectedNumberOfItems, numberOfPages)
            };

            Mock.Get(_agencyExchangeListRequestCoordinator)
                .Setup(c => c.GetDocumentsToDownloadData(null))
                .ReturnsAsync(expectedResult);

            var controller = GetAgencyController();

            // Act
            var result = await controller.DocumentsToDownloadData(null);

            // Assert
            result.Should().BeJsonResult()
                .Value.Should().BeOfType<DocumentListUpdateData>()
                .Which.Should().BeEquivalentTo(expectedResult);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(_agencyExchangeListRequestCoordinator));
        }

        #endregion


        #region Delete Documents Journey Tests

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task DeleteDocumentsAreYouSure_WhenNoFileNamesPassed_RedirectsToDownloadPage(bool nullDocumentReferences)
        {
            // Arrange
            var deleteDocumentsAreYouSure = new DeleteDocumentsAreYouSure
            {
                DocumentReferences = nullDocumentReferences ? null : Enumerable.Empty<string>()
            };

            var listRequest = new ListRequest();
            var controller = GetAgencyController();

            // Act
            var result = await controller.DeleteDocumentsAreYouSure(deleteDocumentsAreYouSure, listRequest);

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(AgencyController.DownloadDocuments))
                .WithControllerName(NameOf<AgencyController>())
                .WithRouteValue("error", true)
                .WithRouteValue("errorAction", nameof(AgencyController.DeleteDocumentsAreYouSure));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(12)]
        [DataRow(124)]
        [DataRow(245)]
        [DataRow(234)]
        public async Task DeleteDocumentsAreYouSure_WhenFileNamesPassedSentByOrganisation_ReturnsView(int numberOfDocumentReferences)
        {
            // Arrange
            var testDocRefs = Enumerable.Range(1, numberOfDocumentReferences).Select(f => $"file{f}.pdf|batch-x|parent-y").ToList();
            var expectedDocuments = GetTestDownloadFileShareDocuments(1).ToList();

            var listRequest = SentByOrganisationListRequest();

            Mock.Get(_agencyDeletionRequestCoordinator)
                .Setup(c => c.GetDocumentsToDelete(testDocRefs, listRequest.FilterRequest.Filters))
                .ReturnsAsync(expectedDocuments);

            var controller = GetAgencyController();

            var inputViewModel = new DeleteDocumentsAreYouSure
            {
                DocumentReferences = testDocRefs
            };

            var expectedViewModel = new DeleteDocumentsAreYouSure
            {
                DocumentReferences = testDocRefs,
                ListItems = expectedDocuments
            };

            // Act
            var result = await controller.DeleteDocumentsAreYouSure(inputViewModel, listRequest);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedViewModel);

            Mock.VerifyAll(
                Mock.Get(_agencyDeletionRequestCoordinator));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(12)]
        [DataRow(124)]
        [DataRow(245)]
        [DataRow(234)]
        public async Task DeleteDocumentsAreYouSure_WhenFileNamesPublishedByAgency_ReturnsView(int numberOfDocumentReferences)
        {
            // Arrange
            var testDocRefs = Enumerable.Range(1, numberOfDocumentReferences).Select(f => $"file{f}.pdf|batch-x|parent-y").ToList();
            var expectedDocuments = GetTestDownloadFileShareDocuments(1).ToList();
            foreach (AgencyExchangeDocument document in expectedDocuments)
            {
                document.PublishedBy = TestAgencyAdvancedUser.FullName;
            }

            var listRequest = PublishedByAgencyListRequest();

            Mock.Get(_agencyDeletionRequestCoordinator)
                .Setup(c => c.GetDocumentsToDelete(testDocRefs, listRequest.FilterRequest.Filters))
                .ReturnsAsync(expectedDocuments);

            var controller = GetAgencyController();

            var inputViewModel = new DeleteDocumentsAreYouSure
            {
                DocumentReferences = testDocRefs
            };

            var expectedViewModel = new DeleteDocumentsAreYouSure
            {
                DocumentReferences = testDocRefs,
                ListItems = expectedDocuments
            };

            // Act
            var result = await controller.DeleteDocumentsAreYouSure(inputViewModel, listRequest);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedViewModel);

            Mock.VerifyAll(
                Mock.Get(_agencyDeletionRequestCoordinator));
        }

        [TestMethod]
        public async Task DeleteDocumentsConfirmation_WhenNoDocumentReferencesPassed_RedirectsToDownloadPage()
        {
            // Arrange
            var controller = GetAgencyController();

            // Act
            var result = await controller.DeleteDocumentsConfirmation(new DeleteDocumentsConfirmation());

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(AgencyController.DownloadDocuments));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(12)]
        [DataRow(123)]
        [DataRow(567)]
        [DataRow(234)]
        public async Task DeleteDocumentsConfirmation_WhenDocumentReferencesPassed_AndShouldDisplayPublishOrUpdateIsTrue_DeletesTheDocumentsAndShowsExpectedView(int numberOfDocumentReferences)
        {
            // Arrange
            var testDocRefs = Enumerable.Range(1, numberOfDocumentReferences).Select(f => $"file{f}.pdf|batch-x|parent-y").ToList();
            var testDocuments = GetTestDownloadFileShareDocuments(1).ToList();
            var expectedDocuments = AddVersionsAndPublishedByToTestDownloadFileshareDocuments(testDocuments, TestAgencyAdvancedUserInfo.FullName);

            var publishers = new List<string>();
            foreach (AgencyExchangeDocument document in expectedDocuments)
            {
                publishers.Add(TestAgencyAdvancedUserInfo.FullName);
                document.IsDeleted = true;
            }

            Mock.Get(_agencyDeletionRequestCoordinator)
                .Setup(c => c.DeleteDocuments(testDocRefs, It.IsAny<List<string>>()))
                .ReturnsAsync(expectedDocuments);

            var expectedViewModel = new DeleteDocumentsConfirmation
            {
                DocumentReferences = testDocRefs,
                ListItems = expectedDocuments,
                Publishers = publishers
            };

            var controller = GetAgencyController();

            // Act
            var result = await controller.DeleteDocumentsConfirmation(
                new DeleteDocumentsConfirmation
                {
                    DocumentReferences = testDocRefs,
                    Publishers = publishers
                });

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedViewModel);

            Mock.VerifyAll(
                Mock.Get(_agencyDeletionRequestCoordinator));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(12)]
        [DataRow(123)]
        [DataRow(567)]
        [DataRow(234)]
        public async Task DeleteDocumentsConfirmation_WhenDocumentReferencesPassed_AndShouldDisplayPublishOrUpdateIsFalse_DeletesTheDocumentsAndShowsExpectedView(int numberOfDocumentReferences)
        {
            // Arrange
            var testDocRefs = Enumerable.Range(1, numberOfDocumentReferences).Select(f => $"file{f}.pdf|batch-x|parent-y").ToList();
            var testDocuments = GetTestDownloadFileShareDocuments(1).ToList();
            var expectedDocuments = AddVersionsAndPublishedByToTestDownloadFileshareDocuments(testDocuments);

            foreach (AgencyExchangeDocument document in expectedDocuments)
            {
                document.IsDeleted = true;
            }

            Mock.Get(_agencyDeletionRequestCoordinator)
                .Setup(c => c.DeleteDocuments(testDocRefs, It.IsAny<List<string>>()))
                .ReturnsAsync(expectedDocuments);

            var expectedViewModel = new DeleteDocumentsConfirmation
            {
                DocumentReferences = testDocRefs,
                ListItems = expectedDocuments,
                Publishers = new List<string>()
            };

            var controller = GetAgencyController();

            // Act
            var result = await controller.DeleteDocumentsConfirmation(
                new DeleteDocumentsConfirmation
                {
                    DocumentReferences = testDocRefs,
                });

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedViewModel);

            Mock.VerifyAll(
                Mock.Get(_agencyDeletionRequestCoordinator));
        }

        #endregion


        #region Remove Documents Journey Tests

        [TestMethod]
        public async Task RemoveDocumentsAreYouSure_WhenNoFileNamesPassed_RedirectsToEntryAction()
        {
            // Arrange
            var controller = GetAgencyController();

            // Act
            var result = await controller.RemoveDocumentsAreYouSure(new RemoveDocumentsAreYouSure
            {
                EntryAction = FakeString
            });

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(FakeString)
                .WithControllerName(NameOf<AgencyController>())
                .WithRouteValue("error", true)
                .WithRouteValue("errorAction", nameof(AgencyController.RemoveDocumentsAreYouSure));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(12)]
        [DataRow(1245)]
        [DataRow(23445)]
        [DataRow(234)]
        public async Task RemoveDocumentsAreYouSure_ForOtherTeam_IsUnauthorized(int numberOfFileNames)
        {
            // Arrange
            var team = "my-team";

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync(team);

            var controller = GetAgencyController();

            // Act
            var viewModel = new RemoveDocumentsAreYouSure
            {
                EntryAction = FakeString,
                FileShareDocumentReferences = Enumerable.Range(1, numberOfFileNames).Select(f => $"not-my-team::file{f}.pdf")
            };

            var result = await controller.RemoveDocumentsAreYouSure(viewModel);

            // Assert
            result.Should().BeUnauthorizedResult();

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(12)]
        [DataRow(1245)]
        [DataRow(23445)]
        [DataRow(234)]
        public void RemoveDocumentsAreYouSure_ForInvalidDocumentReferences_Throws(int numberOfFileNames)
        {
            // Arrange
            var controller = GetAgencyController();

            // Act
            var viewModel = new RemoveDocumentsAreYouSure
            {
                EntryAction = FakeString,
                FileShareDocumentReferences = Enumerable.Range(1, numberOfFileNames).Select(f => $"invalid.{f}")
            };

            Func<Task> act = () => controller.RemoveDocumentsAreYouSure(viewModel);

            // Assert
            act.Should().ThrowAsync<ArgumentException>();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(12)]
        [DataRow(1245)]
        [DataRow(23445)]
        [DataRow(234)]
        public async Task RemoveDocumentsAreYouSure_WhenFileNamesPassed_ReturnsView(int numberOfFileNames)
        {
            // Arrange
            var team = "my-team";

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync(team);

            var controller = GetAgencyController();

            // Act
            var viewModel = new RemoveDocumentsAreYouSure
            {
                EntryAction = FakeString,
                FileShareDocumentReferences = Enumerable.Range(1, numberOfFileNames).Select(f => $"{team}::file{f}.pdf")
            };

            var result = await controller.RemoveDocumentsAreYouSure(viewModel);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(viewModel);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        [TestMethod]
        public async Task RemoveDocumentsConfirmation_WhenConfirmationNotChosen_RedirectsToAreYouSurePage()
        {
            // Arrange
            var controller = GetAgencyController();

            // Act
            var result = await controller.RemoveDocumentsConfirmation(new RemoveDocumentsConfirmation
            {
                RemovalConfirmed = null
            });

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithPreserveMethod(true)
                .WithActionName(nameof(AgencyController.RemoveDocumentsAreYouSure))
                .WithControllerName(NameOf<AgencyController>())
                .WithRouteValue("error", true);
        }

        [TestMethod]
        public async Task RemoveDocumentsConfirmation_WhenNoFileNamesPassed_RedirectsToEntryAction()
        {
            // Arrange
            var controller = GetAgencyController();

            // Act
            var result = await controller.RemoveDocumentsConfirmation(new RemoveDocumentsConfirmation
            {
                RemovalConfirmed = true,
                EntryAction = FakeString
            });

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(FakeString);
        }

        [TestMethod]
        public async Task RemoveDocumentsConfirmation_WhenChosenToAbortRemoval_RedirectsToEntryActionWithoutRemovingFiles()
        {
            // Arrange
            var controller = GetAgencyController();

            // Act
            var result = await controller.RemoveDocumentsConfirmation(new RemoveDocumentsConfirmation
            {
                RemovalConfirmed = false,
                EntryAction = FakeString,
                FileShareDocumentReferences = new[] { FakeString }
            });

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(FakeString);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(12)]
        [DataRow(1245)]
        [DataRow(23445)]
        [DataRow(234)]
        public async Task RemoveDocumentsConfirmation_ForOtherTeam_IsUnauthorized(
            int numberOfDocumentReferences)
        {
            // Arrange
            var team = "my-team";

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync(team);

            var controller = GetAgencyController();

            var viewModel = new RemoveDocumentsConfirmation
            {
                RemovalConfirmed = true,
                FileShareDocumentReferences = Enumerable.Range(1, numberOfDocumentReferences).Select(f => $"not-my-team::file{f}.pdf")
            };

            // Act
            var result = await controller.RemoveDocumentsConfirmation(viewModel);

            // Assert
            result.Should().BeUnauthorizedResult();

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(12)]
        [DataRow(1245)]
        [DataRow(23445)]
        [DataRow(234)]
        public void RemoveDocumentsConfirmation_ForInvalidDocumentReferences_Throws(
            int numberOfDocumentReferences)
        {
            // Arrange
            var controller = GetAgencyController();

            var viewModel = new RemoveDocumentsConfirmation
            {
                RemovalConfirmed = true,
                FileShareDocumentReferences = Enumerable.Range(1, numberOfDocumentReferences).Select(f => $"something.invalid.{f}")
            };

            // Act
            Func<Task> act = () => controller.RemoveDocumentsConfirmation(viewModel);

            // Assert
            act.Should().ThrowAsync<ArgumentException>();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(12)]
        [DataRow(1245)]
        [DataRow(23445)]
        [DataRow(234)]
        public async Task RemoveDocumentsConfirmation_WhenFileNamesPassedAndChosenToConfirmRemoval_RemovesFilesAndReturnsView(
            int numberOfDocumentReferences)
        {
            // Arrange
            var controller = GetAgencyController();

            var team = "my-team";

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync(team);

            var viewModel = new RemoveDocumentsConfirmation
            {
                RemovalConfirmed = true,
                FileShareDocumentReferences = Enumerable.Range(1, numberOfDocumentReferences).Select(f => $"{team}::file{f}.pdf")
            };

            var mockAgencyApiClient = Mock.Get(AgencyApiClient);

            List<string> actualRemovedFileNames = null;

            mockAgencyApiClient
                .Setup(a => a.RemoveTeamDocuments(team, It.IsAny<IEnumerable<string>>()))
                .Returns((string capturedTeam, IEnumerable<string> capturedFileNames) =>
                {
                    actualRemovedFileNames = capturedFileNames.ToList();
                    return Task.CompletedTask;
                });

            // Act
            var result = await controller.RemoveDocumentsConfirmation(viewModel);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(viewModel);

            actualRemovedFileNames.Should().BeEquivalentTo(
                Enumerable.Range(1, numberOfDocumentReferences).Select(f => $"file{f}.pdf"));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                mockAgencyApiClient);
        }

        #endregion


        #region Private Helpers

        private AgencyController GetAgencyController(DocumentExchangeConfiguration configuration = null)
        {
            return new AgencyController(
                UserInformationProvider,
                Options.Create(configuration ?? TestConfiguration),
                AgencyApiClient,
                MimeMappingService,
                AgencyDocumentErrorReportBuilder,
                SystemProvider,
                ExchangeDocumentDownloadService,
                Mapper,
                _agencySummaryCoordinator,
                _agencyFileShareListRequestCoordinator,
                _agencyExchangeListRequestCoordinator,
                _agencyDeletionRequestCoordinator,
                _documentReferenceService);
        }

        #endregion
    }
}