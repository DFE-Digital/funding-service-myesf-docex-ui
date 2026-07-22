using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.Interfaces.Coordinators;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using Pds.DocumentExchange.Web.Models;
using Pds.DocumentExchange.Web.Models.Agency;
using Pds.DocumentExchange.Web.Models.SupportTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Tests.Unit.Controllers
{
    [TestClass]
    [TestCategory("Unit")]
    public class SupportToolsControllerTests : BaseControllerUnitTests
    {
        private readonly SupportToolsController _controller;

        private readonly IAgencyExchangeDeletionRequestCoordinator _agencyDeletionRequestCoordinator
           = Mock.Of<IAgencyExchangeDeletionRequestCoordinator>(MockBehavior.Strict);

        private readonly IListHelper _listHelper
            = Mock.Of<IListHelper>(MockBehavior.Strict);

        public SupportToolsControllerTests()
        {
            _controller = new SupportToolsController(
                UserInformationProvider,
                SupportToolsApiClient,
                SettingsApiClient,
                DateTimeProvider,
                Options.Create(new DocumentExchangeConfiguration()),
                _agencyDeletionRequestCoordinator,
                _listHelper);
        }

        [TestMethod]
        [DataRow(true, true, true)]
        [DataRow(true, false, true)]
        [DataRow(false, true, true)]
        [DataRow(false, false, false)]
        public async Task SupportTools_ReturnsExpectedView(bool isAdmin, bool isAdvanced, bool isAnyInternalRole)
        {
            //Arrange
            Mock.Get(UserInformationProvider)
               .Setup(u => u.CurrentUserIsAdvancedAgencyUser())
               .ReturnsAsync(isAdvanced);

            Mock.Get(UserInformationProvider)
               .Setup(u => u.CurrentUserCanAccessToSupportTools())
               .ReturnsAsync(isAnyInternalRole);

            if (!isAdvanced)
            {
                Mock.Get(UserInformationProvider)
               .Setup(u => u.CurrentUserIsAdminUser())
               .ReturnsAsync(isAdmin);
            }

            // Act
            var result = await _controller.SupportTools();

            // Assert
            var docsPublishedAndDeleteButtonsVisible = result.Should().BeViewResult()
                .Model.Should().BeOfType<SupportToolsModel>().Which.IsUserAdvancedOrAdmin;

            var reportsButtonsVisible = result.Should().BeViewResult()
                .Model.Should().BeOfType<SupportToolsModel>().Which.IsAnyDocumentExchangeInternalUser;

            Assert.IsTrue(docsPublishedAndDeleteButtonsVisible == (isAdmin || isAdvanced));

            Assert.IsTrue(reportsButtonsVisible == isAnyInternalRole);

            Mock.VerifyAll(
              Mock.Get(UserInformationProvider));
        }

        [TestMethod]
        public async Task DocumentsPublishedByDfe_ReturnsExpectedView()
        {
            // Arrange
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

            Mock.Get(_listHelper)
                .Setup(l => l.GetPaginationViewModel(listRequest, apiExpected))
                .Returns((PaginationViewModel)null);

            // Act
            var result = await _controller.DocumentsPublishedByDfe(listRequest);

            // Assert
            var publishedBatches = result.Should().BeViewResult()
                .Model.Should().BeOfType<PublishedBatchesViewModel>()
                .Which.ListItems;

            publishedBatches.Should().BeEquivalentTo(expected);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(SupportToolsApiClient),
                Mock.Get(DateTimeProvider));
        }

        [TestMethod]
        public async Task DownloadDocumentsCsv_ReturnsExpectedCsvFile()
        {
            // Arrange
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
                Mock.Get(UserInformationProvider),
                Mock.Get(SupportToolsApiClient),
                Mock.Get(DateTimeProvider));
        }

        [TestMethod]
        public async Task DownloadEmailsCsv_ReturnsExpectedCsvFile()
        {
            // Arrange
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
                Mock.Get(UserInformationProvider),
                Mock.Get(SupportToolsApiClient),
                Mock.Get(DateTimeProvider));
        }

        [TestMethod]
        public async Task DeleteDocuments_ShouldReturnView()
        {
            // Arrange
            var products = Enumerable.Range(1, 100).Select(num => new Product
            {
                Identifier = num,
                Name = $"Product-{num}"
            });

            Mock.Get(SettingsApiClient)
                .Setup(client => client.GetAllProducts())
                .ReturnsAsync(products);

            // Act
            var result = await _controller.DeleteDocuments(null);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<DeleteDocumentsViewModel>();

            Mock.VerifyAll(
               Mock.Get(SettingsApiClient));
        }

        [TestMethod]
        [DataRow(null, "valid-type", "202122")]
        [DataRow("", "valid-type", "202122")]
        [DataRow("123456789", "valid-type", "202122")]
        [DataRow("InvalidUkprn", "valid-type", "202122")]
        [DataRow("12345678", null, "202122")]
        [DataRow("12345678", "", "202122")]
        [DataRow("12345678", "-1", "202122")]
        [DataRow("12345678", "valid-type", null)]
        [DataRow("12345678", "valid-type", "")]
        [DataRow("12345678", "valid-type", "2021220")]
        [DataRow("12345678", "valid-type", "invalid-period")]
        public async Task SelectDocumentsToDelete_WhenInputModelNotValid_ShouldReturnWithError(string ukprn, string documentType, string period)
        {
            // Arrange
            var model = new DeleteDocumentsViewModel()
            {
                Ukprn = ukprn,
                DocumentType = documentType,
                Period = period
            };

            // Act
            var result = await _controller.SelectDocumentsToDelete(model);

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName("DeleteDocuments")
                .WithControllerName("SupportTools");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(2)]
        [DataRow(20)]
        public async Task SelectDocumentsToDelete_WhenDocumentNotSingle_ShouldReturnWithError(int numberOfDocuments)
        {
            // Arrange
            string ukprn = "12345678";
            string documentType = "filetype1";
            string period = "202021";

            var model = new DeleteDocumentsViewModel()
            {
                Ukprn = ukprn,
                DocumentType = documentType,
                Period = period,
                ExchangeDocumentDirection = Services.Enums.ExchangeDocumentDirection.PublishedByAgency
            };

            var expectedDocuments = GetTestDownloadFileShareDocuments(numberOfDocuments).ToList();

            Mock.Get(_agencyDeletionRequestCoordinator)
               .Setup(c => c.GetDocumentsToDelete(model.ExchangeDocumentDirection, int.Parse(model.Ukprn), model.DocumentType, model.Period))
               .ReturnsAsync(expectedDocuments);

            // Act
            var result = await _controller.SelectDocumentsToDelete(model);

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName("DeleteDocuments")
                .WithControllerName("SupportTools");

            Mock.VerifyAll(
               Mock.Get(_agencyDeletionRequestCoordinator));
        }

        [TestMethod]
        public async Task SelectDocumentsToDelete_WhenDocumentHasNoPreviousVersions_ShouldRedirectToDeletePublishedDocumentAreYouSure()
        {
            // Arrange
            string ukprn = "12345678";
            string documentType = "filetype1";
            string period = "202021";

            var model = new DeleteDocumentsViewModel()
            {
                Ukprn = ukprn,
                DocumentType = documentType,
                Period = period,
                ExchangeDocumentDirection = Services.Enums.ExchangeDocumentDirection.PublishedByAgency
            };

            var expectedDocuments = new List<AgencyExchangeDocument>()
            {
                new AgencyExchangeDocument()
                {
                    PreviousVersions = null
                }
            };

            Mock.Get(_agencyDeletionRequestCoordinator)
               .Setup(c => c.GetDocumentsToDelete(model.ExchangeDocumentDirection, int.Parse(model.Ukprn), model.DocumentType, model.Period))
               .ReturnsAsync(expectedDocuments);

            // Act
            var result = await _controller.SelectDocumentsToDelete(model);

            // Assert
            result.Should().BeViewResult()
                  .Model.Should().BeOfType<DeletePublishedDocumentAreYouSure>();

            Mock.VerifyAll(
              Mock.Get(_agencyDeletionRequestCoordinator));
        }

        [TestMethod]
        public async Task SelectDocumentsToDelete_WhenDocumentHasPreviousVersions_ShouldRedirectToDeleteDocumentsVersionSelect()
        {
            // Arrange
            string ukprn = "12345678";
            string documentType = "filetype1";
            string period = "202021";

            var model = new DeleteDocumentsViewModel()
            {
                Ukprn = ukprn,
                DocumentType = documentType,
                Period = period,
                ExchangeDocumentDirection = Services.Enums.ExchangeDocumentDirection.PublishedByAgency
            };

            var expectedDocuments = new List<AgencyExchangeDocument>()
            {
                new AgencyExchangeDocument()
                {
                    ParentBatchIdentifier = "id1",
                    PreviousVersions = new List<AgencyExchangeDocument>()
                    {
                        new AgencyExchangeDocument()
                        {
                            ParentBatchIdentifier = "id2"
                        }
                    }
                }
            };

            Mock.Get(_agencyDeletionRequestCoordinator)
               .Setup(c => c.GetDocumentsToDelete(model.ExchangeDocumentDirection, int.Parse(model.Ukprn), model.DocumentType, model.Period))
               .ReturnsAsync(expectedDocuments);

            // Act
            var result = await _controller.SelectDocumentsToDelete(model);

            // Assert
            result.Should().BeViewResult()
                  .Model.Should().BeOfType<DeleteDocumentsVersionSelect>();

            Mock.VerifyAll(
              Mock.Get(_agencyDeletionRequestCoordinator));
        }

        [TestMethod]
        public void Reports_ReturnsExpectedView()
        {
            // Act
            var result = _controller.Reports();

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<ReportsViewModel>();

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        [TestMethod]
        public void Reports_ManagementInformation_ReturnsExpectedView()
        {
            // Act
            var result = _controller.ManagementInformation();

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<ManagementInformationReportViewModel>();

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        [TestMethod]
        public async Task Reports_ManagementInformationOnSubmittingForm_ReturnsFileContentResultAsExpected()
        {
            // Arrange
            var to = DateTime.Now;
            var from = to.AddDays(-15);
            var fileContent = new byte[] { 1, 2, 3, 4, 5 };

            var model = new ManagementInformationReportViewModel()
            {
                FromDay = from.Day,
                FromMonth = from.Month,
                FromYear = from.Year,
                ToDay = to.Day,
                ToMonth = to.Month,
                ToYear = to.Year
            };

            Mock.Get(SupportToolsApiClient).Setup(x => x.DownloadMIReport(from.Date, to.Date)).ReturnsAsync(fileContent);

            // Act
            var result = await _controller.ManagementInformation(model);

            // Assert
            result.Should().BeOfType<FileContentResult>().Which.FileDownloadName.Should().BeEquivalentTo($"Document exchange MI report {DateTime.UtcNow.ToString("dd-MM-yyyy")}.ods");
            result.Should().BeOfType<FileContentResult>().Which.ContentType.Should().BeEquivalentTo("application/vnd.oasis.opendocument.spreadsheet");

            Mock.VerifyAll(
                Mock.Get(SupportToolsApiClient));
        }
    }
}