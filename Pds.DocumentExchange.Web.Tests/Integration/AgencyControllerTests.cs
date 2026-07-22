using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Common.Organisation.Enums;
using Pds.Core.Common.Organisation.Models;
using Pds.Core.Documents.Aspose.Interfaces;
using Pds.Core.Documents.Aspose.Models;
using Pds.Core.Web.Components.Areas.Lists.Builders;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Implementations;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Services.Models.Filters;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Helpers;
using Pds.DocumentExchange.Web.Implementations.Converters;
using Pds.DocumentExchange.Web.Implementations.Coordinators;
using Pds.DocumentExchange.Web.Implementations.Helpers;
using Pds.DocumentExchange.Web.Models.Agency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Tests.Integration
{
    [TestClass, TestCategory("Integration")]
    public class AgencyControllerTests : BaseControllerIntegrationTests
    {
        private static readonly string AllTeams = string.Join(',', AuthorizationHelper.DocumentExchangeAgencyTeamRoles);

        private readonly ISpreadsheetBuilder _spreadsheetBuilder
            = Mock.Of<ISpreadsheetBuilder>(MockBehavior.Strict);

        [TestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(1, 1, 0)]
        [DataRow(0, 0, null)]
        [DataRow(1, 1, null)]
        [DataRow(0, 0, 1)]
        [DataRow(1, 1, 1)]
        [DataRow(12, 21, 24)]
        [DataRow(145, 43, 87)]
        [DataRow(245, 66, 142)]
        [DataRow(24, 56, 26)]
        public async Task AgencyHome_MakesExpectedApiCallsAndReturnsExpectedView(
            int invalidCount, int validCount, int? newDocuments)
        {
            // Arrange
            SetupUserIdentity(TestAgencyUser);

            Mock.Get(AgencyApiClient)
                .Setup(a => a.GetTeamSummary(TestAgencyUser.Roles.First()))
                .ReturnsAsync(new FileShareSummary
                {
                    InvalidCount = invalidCount,
                    ValidCount = validCount
                });

            Mock.Get(ExchangeApiClient)
                .Setup(a => a.GetAgencyTeamSummary(TestAgencyUser.Roles.First()))
                .ReturnsAsync(new Summary
                {
                    CountOfNewDocuments = newDocuments
                });

            var controller = await GetAgencyController();

            // Act
            var result = await controller.AgencyHome();

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<AgencyHome>()
                .Which.Tiles.Should().HaveCount(3);

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(AgencyApiClient),
                Mock.Get(ExchangeApiClient));
        }

        [TestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(1, 1, 0)]
        [DataRow(0, 0, null)]
        [DataRow(1, 1, null)]
        [DataRow(0, 0, 1)]
        [DataRow(1, 1, 1)]
        [DataRow(12, 21, 24)]
        [DataRow(145, 43, 87)]
        [DataRow(245, 66, 142)]
        [DataRow(24, 56, 26)]
        public async Task AgencyHome_ForAdvancedUser_MakesExpectedApiCallsAndReturnsExpectedView(
            int invalidCount, int validCount, int? newDocuments)
        {
            // Arrange
            SetupUserIdentity(TestAgencyAdvancedUser);

            Mock.Get(AgencyApiClient)
                .Setup(a => a.GetTeamSummary(AllTeams))
                .ReturnsAsync(new FileShareSummary
                {
                    InvalidCount = invalidCount,
                    ValidCount = validCount
                });

            Mock.Get(ExchangeApiClient)
                .Setup(a => a.GetAgencyTeamSummary(AllTeams))
                .ReturnsAsync(new Summary
                {
                    CountOfNewDocuments = newDocuments
                });

            var controller = await GetAgencyController();

            // Act
            var result = await controller.AgencyHome();

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<AgencyHome>()
                .Which.Tiles.Should().HaveCount(3);

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(AgencyApiClient),
                Mock.Get(ExchangeApiClient));
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(12, 21)]
        [DataRow(145, 43)]
        [DataRow(245, 66)]
        [DataRow(24, 56)]
        public async Task FileShare_MakesExpectedApiCallsAndReturnsExpectedView(
            int invalidCount, int validCount)
        {
            // Arrange
            SetupUserIdentity(TestAgencyUser);

            Mock.Get(AgencyApiClient)
                .Setup(a => a.GetTeamSummary(TestAgencyUser.Roles.First()))
                .ReturnsAsync(new FileShareSummary
                {
                    InvalidCount = invalidCount,
                    ValidCount = validCount
                });

            var controller = await GetAgencyController();

            // Act
            var result = await controller.FileShare();

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<FileShare>()
                .Which.Should().BeEquivalentTo(
                    new FileShare
                    {
                        CountOfInvalidDocuments = invalidCount,
                        CountOfValidDocuments = validCount,
                        TotalCountOfDocuments = invalidCount + validCount
                    });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(AgencyApiClient));
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(12, 21)]
        [DataRow(145, 43)]
        [DataRow(245, 66)]
        [DataRow(24, 56)]
        public async Task FileShare_ForAdvancedUser_MakesExpectedApiCallsAndReturnsExpectedView(
            int invalidCount, int validCount)
        {
            // Arrange
            SetupUserIdentity(TestAgencyAdvancedUser);

            Mock.Get(AgencyApiClient)
                .Setup(a => a.GetTeamSummary(AllTeams))
                .ReturnsAsync(new FileShareSummary
                {
                    InvalidCount = invalidCount,
                    ValidCount = validCount
                });

            var controller = await GetAgencyController();

            // Act
            var result = await controller.FileShare();

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<FileShare>()
                .Which.Should().BeEquivalentTo(
                    new FileShare
                    {
                        CountOfInvalidDocuments = invalidCount,
                        CountOfValidDocuments = validCount,
                        TotalCountOfDocuments = invalidCount + validCount
                    });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(AgencyApiClient));
        }


        #region Documents to Review

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
            SetupUserIdentity(TestAgencyUser);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();
            var testViewModelFilters = GetTestViewModelFilterCategories(numberOfFilters).ToList();

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockAgencyClient = Mock.Get(AgencyApiClient);
            mockAgencyClient
                .Setup(a => a.ListTeamDocuments(
                    TestAgencyUser.Roles.First(),
                    It.Is<AgencyListDocumentOptions>(options =>
                        options.Validity == AgencyDocumentValidity.Invalid
                        && options.PageSize == TestConfiguration.ListPageSize)))
                .ReturnsAsync(new ListResult<AgencyDocument>
                {
                    Items = GetTestAgencyDocuments(numberOfPages, false),
                    Filters = testServiceFilters,
                    TotalPages = numberOfPages,
                    TotalItems = expectedNumberOfItems
                });

            var controller = await GetAgencyController();

            // Act
            var result = await controller.DocumentsToReview(null);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<DocumentsToReview>()
                .Which.Should().BeEquivalentTo(
                    new DocumentsToReview
                    {
                        ListItems = GetTestInvalidFileShareDocuments(numberOfPages),
                        FilterCategories = testViewModelFilters,
                        Pagination = new PaginationViewModel
                        {
                            TotalItems = expectedNumberOfItems,
                            TotalPages = numberOfPages,
                            Page = 1,
                            PageSize = TestConfiguration.ListPageSize
                        },
                        AnyDocumentsAvailable = expectedNumberOfItems > 0 || testServiceFilters.Count > 0,
                    }, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                mockAgencyClient);
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
            SetupUserIdentity(TestAgencyAdvancedUser);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();
            var testViewModelFilters = GetTestViewModelFilterCategories(numberOfFilters).ToList();

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockAgencyClient = Mock.Get(AgencyApiClient);
            mockAgencyClient
                .Setup(a => a.ListTeamDocuments(
                    AllTeams,
                    It.Is<AgencyListDocumentOptions>(options =>
                        options.Validity == AgencyDocumentValidity.Invalid
                        && options.PageSize == TestConfiguration.ListPageSize)))
                .ReturnsAsync(new ListResult<AgencyDocument>
                {
                    Items = GetTestAgencyDocuments(numberOfPages, false),
                    Filters = testServiceFilters,
                    TotalPages = numberOfPages,
                    TotalItems = expectedNumberOfItems
                });

            var controller = await GetAgencyController();

            // Act
            var result = await controller.DocumentsToReview(
                new DocumentsRequest
                {
                    Error = true,
                    ErrorAction = FakeString
                });

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<DocumentsToReview>()
                .Which.Should().BeEquivalentTo(
                    new DocumentsToReview
                    {
                        ListItems = GetTestInvalidFileShareDocuments(numberOfPages),
                        FilterCategories = testViewModelFilters,
                        Error = true,
                        ErrorAction = FakeString,
                        Pagination = new PaginationViewModel
                        {
                            TotalItems = expectedNumberOfItems,
                            TotalPages = numberOfPages,
                            Page = 1,
                            PageSize = TestConfiguration.ListPageSize
                        },
                        AnyDocumentsAvailable = expectedNumberOfItems > 0 || testServiceFilters.Count > 0,
                    }, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                mockAgencyClient);
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
            SetupUserIdentity(TestAgencyUser);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();
            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockAgencyClient = Mock.Get(AgencyApiClient);
            mockAgencyClient
                .Setup(a => a.ListTeamDocuments(
                    TestAgencyUser.Roles.First(),
                    It.Is<AgencyListDocumentOptions>(options =>
                        options.Validity == AgencyDocumentValidity.Invalid
                        && options.PageSize == TestConfiguration.ListPageSize)))
                .ReturnsAsync(new ListResult<AgencyDocument>
                {
                    Items = GetTestAgencyDocuments(numberOfPages, true),
                    Filters = testServiceFilters,
                    TotalPages = numberOfPages,
                    TotalItems = expectedNumberOfItems
                });

            var controller = await GetAgencyController();

            // Act
            var result = await controller.DocumentsToReviewData(null);

            // Assert
            result.Should().BeJsonResult()
                .Value.Should().BeOfType<DocumentListUpdateData>()
                .Which.Should().BeEquivalentTo(
                    new DocumentListUpdateData
                    {
                        ListItems = GetTestInvalidFileShareDocuments(numberOfPages),
                        Pagination = GetExpectedPaginationUpdateData(expectedNumberOfItems, numberOfPages)
                    });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                mockAgencyClient);
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
            SetupUserIdentity(TestAgencyUser);

            var controller = await GetAgencyController();

            var expectedDocumentReferences = Enumerable.Range(1, numberOfDocumentReferences).Select(f => $"not-my-team::file{f}.pdf");

            // Act
            var result = await controller.DocumentsToReviewReport(new FileShareDocumentReferenceList
            {
                FileShareDocumentReferences = expectedDocumentReferences
            });

            // Assert
            result.Should().BeUnauthorizedResult();

            Mock.Verify(
                Mock.Get(IdentityService));
        }

        [TestMethod, DynamicData(nameof(DocumentsToReviewReportTestData))]
        public async Task DocumentsToReviewReport_WhenFileNamesPassed_BuildsTheReport(
            int numberOfDocumentReferences,
            Dictionary<string, CellData> expectedCells)
        {
            // Arrange
            SetupUserIdentity(TestAgencyUser);

            var controller = await GetAgencyController();

            var expectedFileContent = Encoding.UTF8.GetBytes("File content");
            var expectedFileName = "DocumentExchange_DocumentErrors_2020-12-25.ods";
            var testAgencyDocuments = GetTestAgencyDocuments(1, numberOfDocumentReferences, false);

            Mock.Get(SystemProvider.DateTime)
                .Setup(d => d.UtcNow())
                .Returns(new DateTime(2020, 12, 25));

            AgencyListDocumentOptions actualListOptions = null;

            Mock.Get(AgencyApiClient)
                .Setup(a => a.ListTeamDocuments(
                    TestAgencyUser.Roles.First(),
                    It.Is<AgencyListDocumentOptions>(options =>
                        options.PageSize == int.MaxValue &&
                        options.Validity == AgencyDocumentValidity.Invalid)))
                .ReturnsAsync(
                    (string teams, AgencyListDocumentOptions listOptions) =>
                    {
                        actualListOptions = listOptions;
                        return new ListResult<AgencyDocument>
                        {
                            Items = testAgencyDocuments
                        };
                    });

            Spreadsheet actualSpreadsheet = null;

            Mock.Get(_spreadsheetBuilder)
                .Setup(s => s.BuildSpreadsheetWithData(It.IsAny<Spreadsheet>(), true, false))
                .Returns((Spreadsheet spreadsheet, bool odsFormat, bool removeFormulas) =>
                {
                    actualSpreadsheet = spreadsheet;
                    return expectedFileContent;
                });

            var expectedSpreadsheet = new Spreadsheet
            {
                Worksheets = new Dictionary<string, Worksheet>
                {
                    {
                        "DocumentErrors",
                        new Worksheet
                        {
                            Cells = expectedCells,
                            Columns = new List<Column>
                            {
                                new Column
                                {
                                    Position = 0,
                                    WidthInches = 4
                                },
                                new Column
                                {
                                    Position = 1,
                                    WidthInches = 4
                                }
                            }
                        }
                    }
                }
            };

            // Act
            var result = await controller.DocumentsToReviewReport(new FileShareDocumentReferenceList
            {
                FileShareDocumentReferences = Enumerable.Range(1, numberOfDocumentReferences).Select(f => $"{TestAgencyUser.Roles.First()}::file{f}.pdf")
            });

            // Assert
            result.Should().BeFileContentResult()
                .WithFileDownloadName(expectedFileName)
                .WithContentType("application/oleobject")
                .FileContents.Should().BeEquivalentTo(expectedFileContent);

            actualListOptions.DocumentNames.Should().BeEquivalentTo(
                Enumerable.Range(1, numberOfDocumentReferences).Select(f => $"file{f}.pdf"));

            actualSpreadsheet.Should().BeEquivalentTo(expectedSpreadsheet);

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(SystemProvider.DateTime),
                Mock.Get(AgencyApiClient),
                Mock.Get(_spreadsheetBuilder));
        }

        #endregion


        #region Documents to Publish

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
            SetupUserIdentity(TestAgencyUser);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();
            var testViewModelFilters = GetTestViewModelFilterCategories(numberOfFilters).ToList();

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockAgencyClient = Mock.Get(AgencyApiClient);
            var testAgencyDocuments = GetTestAgencyDocuments(numberOfPages, true).ToList();

            mockAgencyClient
                .Setup(a => a.ListTeamDocuments(
                    TestAgencyUser.Roles.First(),
                    It.Is<AgencyListDocumentOptions>(options =>
                        options.Validity == AgencyDocumentValidity.Valid
                        && options.PageSize == TestConfiguration.ListPageSize)))
                .ReturnsAsync(new ListResult<AgencyDocument>
                {
                    Items = testAgencyDocuments,
                    Filters = testServiceFilters,
                    TotalPages = numberOfPages,
                    TotalItems = expectedNumberOfItems
                });

            var controller = await GetAgencyController();

            // Act
            var result = await controller.DocumentsToPublish(null);

            // Assert
            result.Should()
                .BeViewResult()
                .Model.Should()
                .BeOfType<DocumentsToPublish>()
                .Which.Should()
                .BeEquivalentTo(
                    new DocumentsToPublish
                    {
                        ListItems = GetTestFileShareDocuments(numberOfPages),
                        FilterCategories = testViewModelFilters,
                        Pagination = new PaginationViewModel
                        {
                            TotalItems = expectedNumberOfItems,
                            TotalPages = numberOfPages,
                            Page = 1,
                            PageSize = TestConfiguration.ListPageSize
                        },
                        AnyDocumentsAvailable = expectedNumberOfItems > 0 || testServiceFilters.Count > 0,
                        SelectedProduct = numberOfPages == 0
                            ? null
                            : new Models.Shared.Product
                            {
                                Identifier = testAgencyDocuments.First().Product.Identifier,
                                Name = testAgencyDocuments.First().Product.Name,
                                PluralName = testAgencyDocuments.First().Product.PluralName
                            },
                        SelectedTeam = numberOfPages == 0
                            ? null
                            : testAgencyDocuments.First().Team
                    },
                    options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                mockAgencyClient);
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
            const int pageNumber = 37;

            SetupUserIdentity(TestAgencyAdvancedUser);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();
            var testViewModelFilters = GetTestViewModelFilterCategories(numberOfFilters).ToList();

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockAgencyClient = Mock.Get(AgencyApiClient);
            var testAgencyDocuments = GetTestAgencyDocuments(numberOfPages, true).ToList();

            mockAgencyClient
                .Setup(a => a.ListTeamDocuments(
                    AllTeams,
                    It.Is<AgencyListDocumentOptions>(options =>
                        options.Validity == AgencyDocumentValidity.Valid
                        && options.PageSize == TestConfiguration.ListPageSize
                        && options.PageNumber == pageNumber)))
                .ReturnsAsync(new ListResult<AgencyDocument>
                {
                    Items = testAgencyDocuments,
                    Filters = testServiceFilters,
                    TotalPages = numberOfPages,
                    TotalItems = expectedNumberOfItems
                });

            var controller = await GetAgencyController();

            // Act
            var result = await controller.DocumentsToPublish(new DocumentsRequest
            {
                Page = pageNumber
            });

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<DocumentsToPublish>()
                .Which.Should().BeEquivalentTo(
                    new DocumentsToPublish
                    {
                        ListItems = GetTestFileShareDocuments(numberOfPages),
                        FilterCategories = testViewModelFilters,
                        Pagination = new PaginationViewModel
                        {
                            TotalItems = expectedNumberOfItems,
                            TotalPages = numberOfPages,
                            Page = pageNumber,
                            PageSize = TestConfiguration.ListPageSize
                        },
                        AnyDocumentsAvailable = expectedNumberOfItems > 0 || testServiceFilters.Count > 0,
                        SelectedProduct = numberOfPages == 0
                            ? null
                            : new Models.Shared.Product
                            {
                                Identifier = testAgencyDocuments.First().Product.Identifier,
                                Name = testAgencyDocuments.First().Product.Name,
                                PluralName = testAgencyDocuments.First().Product.PluralName
                            },
                        SelectedTeam = numberOfPages == 0
                            ? null
                            : testAgencyDocuments.First().Team
                    }, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                mockAgencyClient);
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
            SetupUserIdentity(TestAgencyUser);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockAgencyClient = Mock.Get(AgencyApiClient);
            mockAgencyClient
                .Setup(a => a.ListTeamDocuments(
                    TestAgencyUser.Roles.First(),
                    It.Is<AgencyListDocumentOptions>(options =>
                        options.Validity == AgencyDocumentValidity.Valid
                        && options.PageSize == TestConfiguration.ListPageSize)))
                .ReturnsAsync(new ListResult<AgencyDocument>
                {
                    Items = GetTestAgencyDocuments(numberOfPages, true),
                    Filters = testServiceFilters,
                    TotalPages = numberOfPages,
                    TotalItems = expectedNumberOfItems
                });

            var controller = await GetAgencyController();

            // Act
            var result = await controller.DocumentsToPublishData(null);

            // Assert
            result.Should()
                .BeJsonResult()
                .Value.Should()
                .BeOfType<DocumentListUpdateData>()
                .Which.Should()
                .BeEquivalentTo(
                    new DocumentListUpdateData
                    {
                        ListItems = GetTestFileShareDocuments(numberOfPages),
                        Pagination = GetExpectedPaginationUpdateData(expectedNumberOfItems, numberOfPages),
                        PageUpdateItems = expectedNumberOfItems == 0
                            ? Enumerable.Empty<PageUpdateItem>()
                            : new[]
                            {
                                new PageUpdateItem("#total-documents-for-selected-product", $"{expectedNumberOfItems}"),
                                new PageUpdateItem("#selected-product-name", ExtractInfo(expectedNumberOfItems))
                            }
                    });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                mockAgencyClient);
        }

        #endregion


        #region Download Document

        [TestMethod]
        public async Task DownloadDocument_ForOtherTeam_IsUnauthorized()
        {
            // Arrange
            SetupUserIdentity(TestAgencyUser);

            var controller = await GetAgencyController();

            // Act
            var result = await controller.DownloadDocument("not-my-team", FakeString);

            // Assert
            result.Should().BeUnauthorizedResult();

            Mock.VerifyAll(
                Mock.Get(IdentityService));
        }

        [TestMethod, TestCategory("Integration")]
        [DataRow("test content 1", "abc", "application/octet-stream")]
        [DataRow("test content 2", "abc", "application/octet-stream")]
        [DataRow("test content 3", "abc", "application/octet-stream")]
        [DataRow("test content 1", "pdf", "application/pdf")]
        [DataRow("test content 2", "pdf", "application/pdf")]
        [DataRow("test content 3", "pdf", "application/pdf")]
        [DataRow("test content 1", "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        [DataRow("test content 2", "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        [DataRow("test content 3", "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        public async Task DownloadDocument_ReturnsExpectedFileResult(string fileContent, string extension, string contentType)
        {
            // Arrange
            var fileContentBytes = Encoding.UTF8.GetBytes(fileContent);

            SetupUserIdentity(TestAgencyUser);

            Mock.Get(AgencyApiClient)
                .Setup(a => a.DownloadTeamDocument(
                    TestAgencyUser.Roles.First(),
                    $"{FakeString}.{extension}"))
                .ReturnsAsync(fileContentBytes);

            var controller = await GetAgencyController();

            // Act
            var result = await controller.DownloadDocument(TestAgencyUser.Roles.First(), $"{FakeString}.{extension}");

            // Assert
            result.Should().BeFileContentResult()
                .WithFileDownloadName($"{FakeString}.{extension}")
                .WithContentType(contentType)
                .FileContents.Should().BeEquivalentTo(fileContentBytes);

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(AgencyApiClient));
        }


        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 29)]
        [DataRow(15, 53)]
        [DataRow(25, 66)]
        [DataRow(24, 82)]
        public async Task ManageDocuments_ForNullDocumentsRequest_MakesApiCallsAndReturnsExpectedNumberofDocumentsAndFilters(
           int numberOfPages, int numberOfFilters)
        {
            // Arrange
            SetupUserIdentity(TestAgencyUser);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();
            var testViewModelFilters = GetTestViewModelFilterCategories(numberOfFilters).ToList();

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockExchangeClient = Mock.Get(ExchangeApiClient);
            mockExchangeClient
                .Setup(a => a.GetAgencyTeamDocuments(
                    TestAgencyUser.Roles.First(),
                    It.Is<ExchangeListDocumentOptions>(options =>
                        options.DocumentStatusOption == ExchangeDocumentDirection.SentByOrganisation
                        && options.PageSize == TestConfiguration.ListPageSize)))
                .ReturnsAsync(new ListResult<ExchangeDocument>
                {
                    Items = GetTestExchangeDocuments(numberOfPages),
                    Filters = testServiceFilters,
                    TotalPages = numberOfPages,
                    TotalItems = expectedNumberOfItems
                });

            var expectedResult = new DownloadDocuments
            {
                ListItems = GetTestDownloadFileShareDocuments(numberOfPages),
                FilterCategories = testViewModelFilters,
                Pagination = new PaginationViewModel
                {
                    TotalItems = expectedNumberOfItems,
                    TotalPages = numberOfPages,
                    Page = 1,
                    PageSize = TestConfiguration.ListPageSize
                },
                AnyDocumentsAvailable = expectedNumberOfItems > 0 || testServiceFilters.Count > 0
            };

            var fakeDateTime = new DateTime(2020, 12, 25, 15, 37, 23, 123);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.ConvertToUKTime(It.IsAny<DateTime>()))
                .Returns((DateTime dt) => dt);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.Now())
                .Returns(fakeDateTime);

            var controller = await GetAgencyController();

            // Act
            var result = await controller.DownloadDocuments(null);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<DownloadDocuments>()
                .Which.Should().BeEquivalentTo(expectedResult, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                mockExchangeClient);
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
        public async Task ManageDocuments_ForAdvancedUser_MakesExpectedApiCallsAndReturnsExpectedView(
           int numberOfPages, int numberOfFilters)
        {
            // Arrange
            SetupUserIdentity(TestAgencyAdvancedUser);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();
            var testViewModelFilters = GetTestViewModelFilterCategories(numberOfFilters).ToList();

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockExchangeClient = Mock.Get(ExchangeApiClient);
            mockExchangeClient
                .Setup(a => a.GetAgencyTeamDocuments(
                    AllTeams,
                    It.Is<ExchangeListDocumentOptions>(options =>
                        options.DocumentStatusOption == ExchangeDocumentDirection.SentByOrganisation
                        && options.PageSize == TestConfiguration.ListPageSize)))
                .ReturnsAsync(new ListResult<ExchangeDocument>
                {
                    Items = GetTestExchangeDocuments(numberOfPages),
                    Filters = testServiceFilters,
                    TotalPages = numberOfPages,
                    TotalItems = expectedNumberOfItems
                });

            var expectedResult = new DownloadDocuments
            {
                ListItems = GetTestDownloadFileShareDocuments(numberOfPages),
                FilterCategories = testViewModelFilters,
                Pagination = new PaginationViewModel
                {
                    TotalItems = expectedNumberOfItems,
                    TotalPages = numberOfPages,
                    Page = 1,
                    PageSize = TestConfiguration.ListPageSize
                },
                AnyDocumentsAvailable = expectedNumberOfItems > 0 || testServiceFilters.Count > 0,
                UserIsAdvancedAgencyUser = true
            };

            var fakeDateTime = new DateTime(2020, 12, 25, 15, 37, 23, 123);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.ConvertToUKTime(It.IsAny<DateTime>()))
                .Returns((DateTime dt) => dt);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.Now())
                .Returns(fakeDateTime);

            var controller = await GetAgencyController();

            // Act
            var result = await controller.DownloadDocuments(null);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<DownloadDocuments>()
                .Which.Should().BeEquivalentTo(expectedResult, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                mockExchangeClient);
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
        public async Task ManageDocuments_WhenErrorDocumentsRequestHasError_MakesApiCallsReturnsExpectedNumberofDocumentsAndFiltersWithError(
            int numberOfPages, int numberOfFilters)
        {
            // Arrange
            SetupUserIdentity(TestAgencyUser);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();
            var testViewModelFilters = GetTestViewModelFilterCategories(numberOfFilters).ToList();

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockExchangeClient = Mock.Get(ExchangeApiClient);
            mockExchangeClient
                .Setup(a => a.GetAgencyTeamDocuments(
                    TestAgencyUser.Roles.First(),
                    It.Is<ExchangeListDocumentOptions>(options =>
                        options.DocumentStatusOption == ExchangeDocumentDirection.SentByOrganisation
                        && options.PageSize == TestConfiguration.ListPageSize)))
                .ReturnsAsync(new ListResult<ExchangeDocument>
                {
                    Items = GetTestExchangeDocuments(numberOfPages),
                    Filters = testServiceFilters,
                    TotalPages = numberOfPages,
                    TotalItems = expectedNumberOfItems
                });

            var fakeDateTime = new DateTime(2020, 12, 25, 15, 37, 23, 123);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.ConvertToUKTime(It.IsAny<DateTime>()))
                .Returns((DateTime dt) => dt);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.Now())
                .Returns(fakeDateTime);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.UtcNow())
                .Returns(fakeDateTime);

            var controller = await GetAgencyController();

            var documentsRequestWithError = new DocumentsRequest
            {
                Error = true,
                ErrorAction = "the-error-action"
            };

            // Act
            var result = await controller.DownloadDocuments(documentsRequestWithError);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<DownloadDocuments>()
                .Which.Should().BeEquivalentTo(
                    new DownloadDocuments
                    {
                        ListItems = GetTestDownloadFileShareDocuments(numberOfPages),
                        FilterCategories = testViewModelFilters,
                        Pagination = new PaginationViewModel
                        {
                            TotalItems = expectedNumberOfItems,
                            TotalPages = numberOfPages,
                            Page = 1,
                            PageSize = TestConfiguration.ListPageSize
                        },
                        AnyDocumentsAvailable = expectedNumberOfItems > 0 || testServiceFilters.Count > 0,
                        Error = true,
                        ErrorAction = documentsRequestWithError.ErrorAction
                    }, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                mockExchangeClient);
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
        public async Task DocumentsToManageData_ForNullListRequest_MakesApiCallsReturnsExpectedNumberofDocumentsAndFilters(
          int numberOfPages, int numberOfFilters)
        {
            // Arrange
            SetupUserIdentity(TestAgencyUser);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();
            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockExchangeClient = Mock.Get(ExchangeApiClient);
            mockExchangeClient
                .Setup(a => a.GetAgencyTeamDocuments(
                    TestAgencyUser.Roles.First(),
                    It.Is<ExchangeListDocumentOptions>(options =>
                        options.DocumentStatusOption == ExchangeDocumentDirection.SentByOrganisation
                        && options.PageSize == TestConfiguration.ListPageSize)))
                .ReturnsAsync(new ListResult<ExchangeDocument>
                {
                    Items = GetTestExchangeDocuments(numberOfPages),
                    Filters = testServiceFilters,
                    TotalPages = numberOfPages,
                    TotalItems = expectedNumberOfItems
                });

            var fakeDateTime = new DateTime(2020, 12, 25, 15, 37, 23, 123);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.ConvertToUKTime(It.IsAny<DateTime>()))
                .Returns((DateTime dt) => dt);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.Now())
                .Returns(fakeDateTime);

            var controller = await GetAgencyController();

            // Act
            var result = await controller.DocumentsToDownloadData(null);

            // Assert
            result.Should().BeJsonResult()
                .Value.Should().BeOfType<DocumentListUpdateData>()
                .Which.Should().BeEquivalentTo(
                    new DocumentListUpdateData
                    {
                        ListItems = GetTestDownloadFileShareDocuments(numberOfPages),
                        Pagination = GetExpectedPaginationUpdateData(expectedNumberOfItems, numberOfPages)
                    });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                mockExchangeClient);
        }

        #endregion


        #region Download Exchange Documents

        [TestMethod]
        [DataRow("test content 1", "abc", "application/octet-stream")]
        [DataRow("test content 2", "abc", "application/octet-stream")]
        [DataRow("test content 3", "abc", "application/octet-stream")]
        [DataRow("test content 1", "pdf", "application/pdf")]
        [DataRow("test content 2", "pdf", "application/pdf")]
        [DataRow("test content 3", "pdf", "application/pdf")]
        [DataRow("test content 1", "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        [DataRow("test content 2", "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        [DataRow("test content 3", "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        public async Task DownloadExchangeDocument_ReturnsExpectedFileResult(string fileContent, string extension, string contentType)
        {
            // Arrange
            SetupUserIdentity(TestAgencyUser);

            var fakeFileName = $"fakeFileName.{extension}";
            var fakeDocumentReference = $"{fakeFileName}|fakeBatchIdentifier|fakeParentBatchIdentifier";

            var fileContentBytes = Encoding.UTF8.GetBytes(fileContent);

            ExchangeDocumentDownloadRequest actualDownloadRequest = null;

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.DownloadDocuments(It.IsAny<ExchangeDocumentDownloadRequest>()))
                .ReturnsAsync((ExchangeDocumentDownloadRequest request) =>
                {
                    actualDownloadRequest = request;
                    return fileContentBytes;
                });

            var controller = await GetAgencyController();

            // Act
            var result = await controller.DownloadExchangeDocument(fakeDocumentReference);

            // Assert
            result.Should().BeFileContentResult()
                .WithFileDownloadName(fakeFileName)
                .WithContentType(contentType)
                .FileContents.Should().BeEquivalentTo(fileContentBytes);

            actualDownloadRequest.Should()
                .BeEquivalentTo(
                    new ExchangeDocumentDownloadRequest
                    {
                        UserInfo = TestAgencyUserInfo,
                        ListOptions = new ExchangeListDocumentOptions
                        {
                            DocumentReferences = new[]
                            {
                                new DocumentReference
                                {
                                    FileName = fakeFileName,
                                    BatchIdentifier = "fakeBatchIdentifier",
                                    ParentBatchIdentifier = "fakeParentBatchIdentifier"
                                }
                            }
                        }
                    });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(ExchangeApiClient));
        }

        [TestMethod]
        [DataRow("test content 1", "abc", "application/octet-stream")]
        [DataRow("test content 2", "abc", "application/octet-stream")]
        [DataRow("test content 3", "abc", "application/octet-stream")]
        [DataRow("test content 1", "pdf", "application/pdf")]
        [DataRow("test content 2", "pdf", "application/pdf")]
        [DataRow("test content 3", "pdf", "application/pdf")]
        [DataRow("test content 1", "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        [DataRow("test content 2", "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        [DataRow("test content 3", "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        public async Task DownloadExchangeDocumentList_ForSingleDocument_DownloadsFile(string fileContent, string extension, string contentType)
        {
            // Arrange
            SetupUserIdentity(TestAgencyUser);

            var fileName = $"fakeFileName.{extension}";
            var documentReference = $"{fileName}|batchIdentifier|parentBatchIdentifier";
            var documentReferences = new[] { documentReference };

            var documentReferenceList = new DocumentReferenceList
            {
                DocumentReferences = documentReferences
            };

            var fileContentBytes = Encoding.UTF8.GetBytes(fileContent);

            ExchangeDocumentDownloadRequest actualDownloadRequest = null;

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.DownloadDocuments(It.IsAny<ExchangeDocumentDownloadRequest>()))
                .ReturnsAsync((ExchangeDocumentDownloadRequest request) =>
                {
                    actualDownloadRequest = request;
                    return fileContentBytes;
                });

            var controller = await GetAgencyController();

            // Act
            var result = await controller.DownloadExchangeDocumentList(documentReferenceList);

            // Assert
            result.Should().BeFileContentResult()
                .WithFileDownloadName(fileName)
                .WithContentType(contentType)
                .FileContents.Should().BeEquivalentTo(fileContentBytes);

            actualDownloadRequest.Should()
                .BeEquivalentTo(
                    new ExchangeDocumentDownloadRequest
                    {
                        UserInfo = TestAgencyUserInfo,
                        ListOptions = new ExchangeListDocumentOptions
                        {
                            DocumentReferences = new[]
                            {
                                new DocumentReference
                                {
                                    FileName = fileName,
                                    BatchIdentifier = "batchIdentifier",
                                    ParentBatchIdentifier = "parentBatchIdentifier"
                                }
                            }
                        }
                    });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(ExchangeApiClient));
        }

        [TestMethod]
        [DataRow("test content 1")]
        [DataRow("test content 2")]
        [DataRow("test content 3")]
        public async Task DownloadExchangeDocumentList_ForMultipleDocuments_DownloadsFile(string fileContent)
        {
            // Arrange
            SetupUserIdentity(TestAgencyUser);

            var fileName = "fakeFileName.pdf";
            var documentReference1 = $"{fileName}1|batchIdentifier1|parentBatchIdentifier1";
            var documentReference2 = $"{fileName}2|batchIdentifier2|parentBatchIdentifier2";
            var documentReferences = new[] { documentReference1, documentReference2 };

            var documentReferenceList = new DocumentReferenceList
            {
                DocumentReferences = documentReferences
            };

            var fileContentBytes = Encoding.UTF8.GetBytes(fileContent);

            ExchangeDocumentDownloadRequest actualDownloadRequest = null;

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.DownloadDocuments(It.IsAny<ExchangeDocumentDownloadRequest>()))
                .ReturnsAsync((ExchangeDocumentDownloadRequest request) =>
                {
                    actualDownloadRequest = request;
                    return fileContentBytes;
                });

            Mock.Get(AgencyApiClient)
                .Setup(apiClient => apiClient.GetPreviousDocumentVersionReferences(It.IsAny<string>(), It.IsAny<IEnumerable<DocumentReference>>()))
                .ReturnsAsync(Enumerable.Empty<DocumentReference>());

            var fakeDateTime = new DateTime(2020, 12, 25, 15, 37, 23, 123);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.Now())
                .Returns(fakeDateTime);

            var controller = await GetAgencyController();

            // Act
            var result = await controller.DownloadExchangeDocumentList(documentReferenceList);

            // Assert
            result.Should().BeFileContentResult()
                .WithFileDownloadName($"DocumentDownload{fakeDateTime}.zip")
                .WithContentType("application/x-zip-compressed")
                .FileContents.Should().BeEquivalentTo(fileContentBytes);

            actualDownloadRequest.Should()
                .BeEquivalentTo(
                    new ExchangeDocumentDownloadRequest
                    {
                        UserInfo = TestAgencyUserInfo,
                        ListOptions = new ExchangeListDocumentOptions
                        {
                            DocumentReferences = new[]
                            {
                                new DocumentReference
                                {
                                    FileName = $"{fileName}1",
                                    BatchIdentifier = "batchIdentifier1",
                                    ParentBatchIdentifier = "parentBatchIdentifier1"
                                },
                                new DocumentReference
                                {
                                    FileName = $"{fileName}2",
                                    BatchIdentifier = "batchIdentifier2",
                                    ParentBatchIdentifier = "parentBatchIdentifier2"
                                }
                            }
                        }
                    });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(ExchangeApiClient),
                Mock.Get(SystemProvider.DateTime));
        }

        [TestMethod]
        [DataRow("test content 1")]
        [DataRow("test content 2")]
        [DataRow("test content 3")]
        public async Task DownloadExchangeDocumentsByFilters_ReturnsFileContentFromApi(string fileContent)
        {
            // Arrange
            SetupUserIdentity(TestAgencyUser);

            var listFilterCategory = new ListFilterCategory
            {
                Key = "filter1",
                Values = new[] { "value1", "value2" }
            };

            var listRequest = new ListRequest
            {
                FilterRequest = new FilterRequest
                {
                    Filters = new[]
                    {
                        listFilterCategory
                    }
                }
            };

            var fileContentBytes = Encoding.UTF8.GetBytes(fileContent);

            ExchangeDocumentDownloadRequest actualDownloadRequest = null;

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.DownloadAgencyTeamDocuments(TestAgencyUser.Roles.Single(), It.IsAny<ExchangeDocumentDownloadRequest>()))
                .ReturnsAsync((string _, ExchangeDocumentDownloadRequest capturedRequest) =>
                {
                    actualDownloadRequest = capturedRequest;
                    return fileContentBytes;
                });

            var fakeDateTime = new DateTime(2020, 12, 25, 15, 37, 23, 123);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.Now())
                .Returns(fakeDateTime);

            var controller = await GetAgencyController();

            // Act
            var result = await controller.DownloadExchangeDocumentsByFilters(listRequest);

            // Assert
            result.Should().BeFileContentResult()
                .WithFileDownloadName($"DocumentDownload{fakeDateTime}.zip")
                .WithContentType("application/x-zip-compressed")
                .FileContents.Should().BeEquivalentTo(fileContentBytes);

            actualDownloadRequest.Should()
                .BeEquivalentTo(
                    new ExchangeDocumentDownloadRequest
                    {
                        UserInfo = TestAgencyUserInfo,
                        ListOptions = new ExchangeListDocumentOptions
                        {
                            FilterOptions = new List<IFilterOption>
                            {
                                new ListFilterOption
                                {
                                    Key = listFilterCategory.Key,
                                    Type = nameof(ListFilterOption),
                                    Values = listFilterCategory.Values
                                }
                            },
                            PageNumber = 1,
                            PageSize = int.MaxValue
                        }
                    });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(ExchangeApiClient),
                Mock.Get(SystemProvider.DateTime));
        }

        #endregion


        #region Delete Documents

        [TestMethod]
        [DataRow(1)]
        [DataRow(12)]
        [DataRow(124)]
        [DataRow(245)]
        public async Task DeleteDocumentsAreYouSure_WhenFileNamesPassed_ReturnsView(int numberOfDocumentReferences)
        {
            // Arrange
            SetupUserIdentity(TestAgencyUser);

            var controller = await GetAgencyController();

            var testDocRefs = Enumerable.Range(1, numberOfDocumentReferences).Select(f => $"file{f}.pdf|batch-x|parent-y").ToList();
            var testDocRefsData = testDocRefs.Select(docRef => new DocumentReference
            {
                FileName = docRef.Split('|')[0],
                BatchIdentifier = "batch-x",
                ParentBatchIdentifier = "parent-y"
            }).ToList();

            ExchangeListDocumentOptions actualOptions = null;
            var mockExchangeClient = Mock.Get(ExchangeApiClient);
            mockExchangeClient
                .Setup(a => a.GetAgencyTeamDocuments(
                    TestAgencyUser.Roles.First(),
                    It.IsAny<ExchangeListDocumentOptions>()))
                .ReturnsAsync((string team, ExchangeListDocumentOptions options) =>
                {
                    actualOptions = options;
                    return new ListResult<ExchangeDocument>
                    {
                        Items = GetTestExchangeDocuments(1)
                    };
                });

            var fakeDateTime = new DateTime(2020, 12, 25, 15, 37, 23, 123);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.ConvertToUKTime(It.IsAny<DateTime>()))
                .Returns((DateTime dt) => dt);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.Now())
                .Returns(fakeDateTime);

            var inputViewModel = new DeleteDocumentsAreYouSure
            {
                DocumentReferences = testDocRefs
            };

            var expectedDocuments = GetTestDownloadFileShareDocuments(1);

            var expectedViewModel = new DeleteDocumentsAreYouSure
            {
                DocumentReferences = testDocRefs,
                ListItems = expectedDocuments
            };

            var listRequest = SentByOrganisationListRequest();

            // Act
            var result = await controller.DeleteDocumentsAreYouSure(inputViewModel, listRequest);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedViewModel);

            actualOptions.Should().BeEquivalentTo(new ExchangeListDocumentOptions
            {
                PageSize = int.MaxValue,
                DocumentStatusOption = ExchangeDocumentDirection.SentByOrganisation,
                DocumentReferences = testDocRefsData,
                FilterOptions = new IFilterOption[]
                {
                    new RadioFilterOption
                    {
                      Key = "Team",
                      Type = "RadioFilterOption",
                      Value = "DocumentExchangeAdministratorFundingCentre"
                    }
                }
            });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                mockExchangeClient);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(12)]
        [DataRow(123)]
        [DataRow(567)]
        [DataRow(234)]
        public async Task DeleteDocumentsConfirmation_WhenDocumentReferencesPassed_DeletesTheDocuments(int numberOfDocumentReferences)
        {
            // Arrange
            SetupUserIdentity(TestAgencyAdvancedUser2);

            var testDocRefs = Enumerable.Range(1, numberOfDocumentReferences).Select(f => $"file{f}.pdf|batch-x|parent-y").ToList();
            var testDocRefsData = testDocRefs.Select(docRef => new DocumentReferenceWithPreviousVersions(new DocumentReference
            {
                FileName = docRef.Split('|')[0],
                BatchIdentifier = "batch-x",
                ParentBatchIdentifier = "parent-y"
            })).ToList();
            var testDocuments = GetTestDownloadFileShareDocuments(1).ToList();
            var expectedDocuments = AddVersionsAndPublishedByToTestDownloadFileshareDocuments(testDocuments);

            foreach (AgencyExchangeDocument document in expectedDocuments)
            {
                document.IsDeleted = true;
            }

            var mockExchangeClient = Mock.Get(ExchangeApiClient);

            ExchangeDocumentDeleteRequest actualRequest = null;
            mockExchangeClient
                .Setup(e => e.DeleteDocuments(It.IsAny<ExchangeDocumentDeleteRequest>()))
                .ReturnsAsync((ExchangeDocumentDeleteRequest request) =>
                {
                    actualRequest = request;
                    return GetTestExchangeDocuments(1);
                });

            var fakeDateTime = new DateTime(2020, 12, 25, 15, 37, 23, 123);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.ConvertToUKTime(It.IsAny<DateTime>()))
                .Returns((DateTime dt) => dt);

            Mock.Get(SystemProvider.DateTime)
                .Setup(dt => dt.Now())
                .Returns(fakeDateTime);

            var expectedViewModel = new DeleteDocumentsConfirmation
            {
                DocumentReferences = testDocRefs,
                ListItems = expectedDocuments,
                Publishers = new List<string>()
            };

            var controller = await GetAgencyController();

            // Act
            var result = await controller.DeleteDocumentsConfirmation(
                new DeleteDocumentsConfirmation
                {
                    DocumentReferences = testDocRefs
                });

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedViewModel);

            actualRequest.Should().BeEquivalentTo(new ExchangeDocumentDeleteRequest
            {
                UserInfo = TestAgencyAdvancedUserInfo,
                DocumentReferences = testDocRefsData
            });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                mockExchangeClient);
        }

        #endregion


        #region Remove Documents

        [TestMethod]
        [DataRow(1)]
        [DataRow(12)]
        [DataRow(1245)]
        [DataRow(23445)]
        [DataRow(234)]
        public async Task RemoveDocumentsAreYouSure_ForOtherTeam_IsUnauthorized(int numberOfFileNames)
        {
            // Arrange
            SetupUserIdentity(TestAgencyUser);

            var controller = await GetAgencyController();

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
                Mock.Get(IdentityService));
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
            SetupUserIdentity(TestAgencyUser);

            var controller = await GetAgencyController();

            // Act
            var viewModel = new RemoveDocumentsAreYouSure
            {
                EntryAction = FakeString,
                FileShareDocumentReferences = Enumerable.Range(1, numberOfFileNames).Select(f => $"{TestAgencyUser.Roles.First()}::file{f}.pdf")
            };

            var result = await controller.RemoveDocumentsAreYouSure(viewModel);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(viewModel);

            Mock.VerifyAll(
                Mock.Get(IdentityService));
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
            SetupUserIdentity(TestAgencyUser);

            var controller = await GetAgencyController();

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
                Mock.Get(IdentityService));
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
            SetupUserIdentity(TestAgencyUser);

            var controller = await GetAgencyController();

            var viewModel = new RemoveDocumentsConfirmation
            {
                RemovalConfirmed = true,
                FileShareDocumentReferences = Enumerable.Range(1, numberOfDocumentReferences).Select(f => $"{TestAgencyUser.Roles.First()}::file{f}.pdf")
            };

            var mockAgencyApiClient = Mock.Get(AgencyApiClient);

            List<string> actualRemovedFileNames = null;

            mockAgencyApiClient
                .Setup(a => a.RemoveTeamDocuments(TestAgencyUser.Roles.First(), It.IsAny<IEnumerable<string>>()))
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
                Mock.Get(IdentityService),
                mockAgencyApiClient);
        }

        #endregion

        private static IEnumerable<object[]> DocumentsToReviewReportTestData
        {
            get
            {
                var testDocuments = GetTestAgencyDocuments(1, 3, false).ToArray();

                yield return new object[]
                {
                    1,
                    new Dictionary<string, CellData>
                    {
                        { "A1", new CellData("Document name") { FontSize = 11 } },
                        { "B1", new CellData("Document name error") { FontSize = 11 } },
                        { "A2", new CellData(testDocuments[0].FileName) { FontSize = 11 } },
                        { "B2", new CellData(testDocuments[0].FileNameError) { FontSize = 11 } }
                    }
                };
                yield return new object[]
                {
                    2,
                    new Dictionary<string, CellData>
                    {
                        { "A1", new CellData("Document name") { FontSize = 11 } },
                        { "B1", new CellData("Document name error") { FontSize = 11 } },
                        { "A2", new CellData(testDocuments[0].FileName) { FontSize = 11 } },
                        { "B2", new CellData(testDocuments[0].FileNameError) { FontSize = 11 } },
                        { "A3", new CellData(testDocuments[1].FileName) { FontSize = 11 } },
                        { "B3", new CellData(testDocuments[1].FileNameError) { FontSize = 11 } }
                    }
                };
                yield return new object[]
                {
                    3,
                    new Dictionary<string, CellData>
                    {
                        { "A1", new CellData("Document name") { FontSize = 11 } },
                        { "B1", new CellData("Document name error") { FontSize = 11 } },
                        { "A2", new CellData(testDocuments[0].FileName) { FontSize = 11 } },
                        { "B2", new CellData(testDocuments[0].FileNameError) { FontSize = 11 } },
                        { "A3", new CellData(testDocuments[1].FileName) { FontSize = 11 } },
                        { "B3", new CellData(testDocuments[1].FileNameError) { FontSize = 11 } },
                        { "A4", new CellData(testDocuments[2].FileName) { FontSize = 11 } },
                        { "B4", new CellData(testDocuments[2].FileNameError) { FontSize = 11 } }
                    }
                };
            }
        }

        private static string ExtractInfo(int expectedNumberOfItems)
        {
            return expectedNumberOfItems == 1 ? "product1" : "product1s";
        }

        private static IEnumerable<AgencyDocument> GetTestAgencyDocuments(
           int start,
           int count,
           bool validDocuments,
           int productNumber = 1)
        {
            return Enumerable
                .Range(start, count)
                .Select(d => new AgencyDocument
                {
                    FileName = $"file{d}",
                    FileNameError = validDocuments ? string.Empty : FakeString,
                    IsValid = validDocuments,
                    Product = new Product
                    {
                        Identifier = 10000 + productNumber,
                        Name = $"product{productNumber}",
                        PluralName = $"product{productNumber}s"
                    },
                    Year = 201920,
                    OrganisationInfo = new OrganisationInfo
                    {
                        OrganisationIdentifier = new OrganisationIdentifier
                        {
                            Type = OrganisationIdentifierType.Ukprn,
                            Value = $"{10000000 + d}",
                        },
                        Name = $"school {d}"
                    }
                });
        }

        private async Task<AgencyController> GetAgencyController()
        {
            var documentReferenceService = new DocumentReferenceService();
            var mimeMappingService = new MimeMappingService();

            var userInfoProvider = UserInfoProvider;

            var testConfigurationOptions = Options.Create(TestConfiguration);

            var listHelper =
                new ListHelper(
                    new RouteValueDictionaryBuilder(),
                    Mapper,
                    SystemProvider,
                    testConfigurationOptions);

            var dateTimeDisplayHelper = new DateTimeDisplayHelper(SystemProvider.DateTime);

            var documentConverter =
                new DocumentModelConverter(
                    dateTimeDisplayHelper,
                    new DocumentStatusProvider(),
                    null);

            await userInfoProvider.Initialise(null);

            var exchangeDocumentDownloadService = new ExchangeDocumentDownloadService(
                documentReferenceService,
                ExchangeApiClient,
                mimeMappingService,
                SystemProvider);

            var controller = new AgencyController(
                userInfoProvider,
                testConfigurationOptions,
                AgencyApiClient,
                mimeMappingService,
                new AgencyDocumentErrorReportBuilder(
                    AgencyApiClient,
                    _spreadsheetBuilder,
                    MockLoggerAdapter<AgencyDocumentErrorReportBuilder>()),
                SystemProvider,
                exchangeDocumentDownloadService,
                Mapper,
                new AgencySummaryRequestCoordinator(
                    userInfoProvider,
                    AgencyApiClient,
                    ExchangeApiClient),
                new AgencyFileShareListRequestCoordinator(
                    userInfoProvider,
                    AgencyApiClient,
                    listHelper,
                    testConfigurationOptions,
                    Mapper),
                new AgencyExchangeListRequestCoordinator(
                    userInfoProvider,
                    ExchangeApiClient,
                    listHelper,
                    documentConverter,
                    testConfigurationOptions,
                    exchangeDocumentDownloadService),
                new AgencyExchangeDeletionRequestCoordinator(
                    userInfoProvider,
                    ExchangeApiClient,
                    documentConverter,
                    documentReferenceService,
                    SupportToolsApiClient),
                documentReferenceService);

            return controller;
        }

        private IEnumerable<AgencyDocument> GetTestAgencyDocuments(
            int numberOfPages,
            bool validDocuments,
            int pageNumber = 1)
        {
            if (numberOfPages == 0)
            {
                return Enumerable.Empty<AgencyDocument>();
            }

            return Enumerable
                .Range(1, numberOfPages)
                .Select(p => GetTestAgencyDocuments(
                    ((p - 1) * TestConfiguration.ListPageSize) + 1,
                    TestConfiguration.ListPageSize,
                    validDocuments,
                    p))
                .ElementAt(pageNumber - 1);
        }

        private IEnumerable<ExchangeDocument> GetTestExchangeDocuments(int numberOfPages, int pageNumber = 1)
        {
            if (numberOfPages == 0)
            {
                return Enumerable.Empty<ExchangeDocument>();
            }

            var sentByOrganisationEvent = new ExchangeDocumentEvent
            {
                EventType = ExchangeDocumentEventType.SentByOrganisation,
                EventDateTime = new DateTime(2020, 6, 1),
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

            var downloadedByReceiverEvent = new ExchangeDocumentEvent
            {
                EventType = ExchangeDocumentEventType.DownloadedByReceiver,
                EventDateTime = new DateTime(2020, 7, 1),
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

            var eventsForNewStatus = new[] { sentByOrganisationEvent };

            var eventsForDownloadedStatus = new[]
            {
                sentByOrganisationEvent,
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
                        AgencyTeam = TestAgencyUser.Roles.First(),
                        ExchangeDirection = ExchangeDocumentDirection.SentByOrganisation,
                        EventHistory = d % 2 == 0 ? eventsForNewStatus : eventsForDownloadedStatus,
                        Version = d
                    })).ElementAt(pageNumber - 1);
        }
    }
}