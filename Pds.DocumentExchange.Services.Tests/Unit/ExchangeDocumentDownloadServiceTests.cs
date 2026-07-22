using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Utils;
using Pds.DocumentExchange.Services.Implementations;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Tests.Unit
{
    [TestClass]
    [TestCategory("Unit")]
    public class ExchangeDocumentDownloadServiceTests
    {
        private readonly Mock<IDocumentReferenceService> _documentReferenceService = new Mock<IDocumentReferenceService>(MockBehavior.Strict);
        private readonly Mock<IExchangeApiClient> _exchangeApiClient = new Mock<IExchangeApiClient>(MockBehavior.Strict);
        private readonly Mock<IMimeMappingService> _mimeMappingService = new Mock<IMimeMappingService>(MockBehavior.Strict);
        private readonly Mock<ISystemProvider> _systemProvider = new Mock<ISystemProvider>(MockBehavior.Strict);

        private readonly ExchangeDocumentDownloadService _exchangeDocumentDownloadService;

        public ExchangeDocumentDownloadServiceTests()
        {
            _exchangeDocumentDownloadService = new ExchangeDocumentDownloadService(
                _documentReferenceService.Object,
                _exchangeApiClient.Object,
                _mimeMappingService.Object,
                _systemProvider.Object);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task DownloadExchangeDocumentByReference_WhenDocumentReferenceIsNullOrEmpty_ThrowsArgumentNullException(string documentReference)
        {
            // Act
            Func<Task<DownloadedFile>> func = () => _exchangeDocumentDownloadService.DownloadExchangeDocumentByReference(documentReference, new UserInfo());

            // Assert
            await func.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task DownloadExchangeDocumentByReference_WhenUserInfoIsNull_ThrowsArgumentNullException()
        {
            // Act
            Func<Task<DownloadedFile>> func = () => _exchangeDocumentDownloadService.DownloadExchangeDocumentByReference("the-documentReference", null);

            // Assert
            await func.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task DownloadExchangeDocumentByReference_WhenDocumentReferenceIsValid_ReturnsExpectedFile()
        {
            // Arrange
            var fileName = "FileName.pdf";
            var batchId = "BatchIdentifier";
            var parentBatchId = "ParentBatchIdentifier";

            var documentReferenceString = $"{fileName}|{batchId}|{parentBatchId}";

            var documentReference = new DocumentReference
            {
                FileName = fileName,
                BatchIdentifier = batchId,
                ParentBatchIdentifier = parentBatchId
            };

            var fileContentBytes = new byte[] { 1, 2, 3, 4, 5 };
            var mimeType = "application/the-file-type";

            var expectedResult = new DownloadedFile
            {
                Name = fileName,
                Content = fileContentBytes,
                ContentType = mimeType
            };

            _documentReferenceService
                .Setup(d => d.CreateDocumentReferenceFromString(documentReferenceString))
                .Returns(documentReference);

            _exchangeApiClient
                .Setup(a => a.DownloadDocuments(It.IsAny<ExchangeDocumentDownloadRequest>()))
                .ReturnsAsync(fileContentBytes);

            _mimeMappingService
                .Setup(m => m.GetContentType(fileName))
                .Returns(mimeType);

            // Act
            var result = await _exchangeDocumentDownloadService.DownloadExchangeDocumentByReference(documentReferenceString, new UserInfo());

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task DownloadExchangeDocumentsByReferences_WhenSingleDocumentReference_ReturnsExpectedFile()
        {
            // Arrange
            var fileName = "FileName.pdf";
            var batchId = "BatchIdentifier";
            var parentBatchId = "ParentBatchIdentifier";

            var documentReferenceString = $"{fileName}|{batchId}|{parentBatchId}";
            var documentReferenceStrings = new[] { documentReferenceString };

            var documentReference = new DocumentReference
            {
                FileName = fileName,
                BatchIdentifier = batchId,
                ParentBatchIdentifier = parentBatchId
            };

            var fileContentBytes = new byte[] { 1, 2, 3, 4, 5 };
            var mimeType = "application/the-file-type";

            var expectedResult = new DownloadedFile
            {
                Name = fileName,
                Content = fileContentBytes,
                ContentType = mimeType
            };

            _documentReferenceService
               .Setup(d => d.CreateDocumentReferenceFromString(documentReferenceString))
               .Returns(documentReference);

            _exchangeApiClient
                .Setup(a => a.DownloadDocuments(It.IsAny<ExchangeDocumentDownloadRequest>()))
                .ReturnsAsync(fileContentBytes)
                .Verifiable();

            _mimeMappingService
                .Setup(m => m.GetContentType(fileName))
                .Returns(mimeType)
                .Verifiable();

            // Act
            var result = await _exchangeDocumentDownloadService.DownloadExchangeDocumentsByReferences(documentReferenceStrings, new UserInfo());

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void DownloadExchangeDocumentsByReferences_ForNullOrEmptyList_Throws(bool nullList)
        {
            // Arrange
            IEnumerable<string> param = nullList ? null : Enumerable.Empty<string>();

            // Act
            Func<Task> act = async () => await _exchangeDocumentDownloadService.DownloadExchangeDocumentsByReferences(
                param,
                new UserInfo());

            // Assert
            act.Should().ThrowAsync<ArgumentException>();
        }

        [TestMethod]
        public async Task DownloadExchangeDocumentsByReferences_WhenMultipleDocumentReferences_ReturnsZipFile()
        {
            // Arrange
            var firstFileName = "file-name-01.pdf";
            var firstBatchId = "batch-id-01";
            var firstParentBatchId = "parent-batch-id-01";

            var secondFileName = "file-name-02.pdf";
            var secondBatchId = "batch-id-02";
            var secondParentBatchId = "parent-batch-id-02";

            var firstDocumentReference = $"{firstFileName}|{firstBatchId}|{firstParentBatchId}";
            var secondDocumentReference = $"{secondFileName}|{secondBatchId}|{secondParentBatchId}";
            var documentReferences = new[] { firstDocumentReference, secondDocumentReference };

            var zipFileContentBytes = new byte[] { 1, 2, 3, 4, 5 };
            var zipMimeType = "application/the-zip-file-type";

            var systemNow = new DateTime(2020, 6, 1);
            var zipFileName = $"DocumentDownload{systemNow}.zip";

            var expectedResult = new DownloadedFile
            {
                Name = zipFileName,
                Content = zipFileContentBytes,
                ContentType = zipMimeType
            };

            _exchangeApiClient
                .Setup(a => a.DownloadDocuments(
                    It.IsAny<ExchangeDocumentDownloadRequest>()))
                .ReturnsAsync(zipFileContentBytes);

            _systemProvider
                .Setup(s => s.DateTime.Now())
                .Returns(systemNow);

            _mimeMappingService
                .Setup(m => m.GetContentType(zipFileName))
                .Returns(zipMimeType);

            // Act
            var result = await _exchangeDocumentDownloadService.DownloadExchangeDocumentsByReferences(
               documentReferences,
               new UserInfo());

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void DownloadAgencyExchangeDocumentsByListOptions_ForNullListOptions_Throws()
        {
            // Arrange / Act
            Func<Task> act = async () => await _exchangeDocumentDownloadService.DownloadAgencyExchangeDocumentsByListOptions(
                null,
                new UserInfo(),
                "test");

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>();
            act.Should().ThrowAsync<ArgumentNullException>().Where(e => e.Message.Contains("The list options parameter cannot be null."));
        }

        [TestMethod]
        public void DownloadAgencyExchangeDocumentsByListOptions_ForNullUserInfo_Throws()
        {
            // Arrange / Act
            Func<Task> act = async () => await _exchangeDocumentDownloadService.DownloadAgencyExchangeDocumentsByListOptions(
                new ExchangeListDocumentOptions(),
                null,
                "test");

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>();
            act.Should().ThrowAsync<ArgumentNullException>().Where(e => e.Message.Contains("The user info parameter cannot be null."));
        }

        [TestMethod]
        public void DownloadAgencyExchangeDocumentsByListOptions_ForEmptyTeams_Throws()
        {
            // Arrange / Act
            Func<Task> act = async () => await _exchangeDocumentDownloadService.DownloadAgencyExchangeDocumentsByListOptions(
                new ExchangeListDocumentOptions(),
                new UserInfo(),
                string.Empty);

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>();
            act.Should().ThrowAsync<ArgumentNullException>().Where(e => e.Message.Contains("The teams parameter cannot be null or empty"));
        }

        [TestMethod]
        public async Task DownloadAgencyExchangeDocumentsByListOptions_ForValidParams_ReturnsExpectedContentFromApi()
        {
            // Arrange
            var listOptions = new ExchangeListDocumentOptions();
            var userInfo = new UserInfo();
            var teams = "test-team1,test-team2";

            var zipFileContentBytes = new byte[] { 1, 2, 3, 4, 5 };
            var zipMimeType = "application/the-zip-file-type";

            var systemNow = new DateTime(2020, 6, 1);
            var zipFileName = $"DocumentDownload{systemNow}.zip";

            var expectedResult = new DownloadedFile
            {
                Name = zipFileName,
                Content = zipFileContentBytes,
                ContentType = zipMimeType
            };

            ExchangeDocumentDownloadRequest actualDocumentDownloadRequest = null;

            _ = _exchangeApiClient
                .Setup(a => a.DownloadAgencyTeamDocuments(
                    teams,
                    It.IsAny<ExchangeDocumentDownloadRequest>()))
                .ReturnsAsync((string _, ExchangeDocumentDownloadRequest capturedRequest) =>
                {
                    actualDocumentDownloadRequest = capturedRequest;
                    return zipFileContentBytes;
                });

            _systemProvider
                .Setup(s => s.DateTime.Now())
                .Returns(systemNow);

            _mimeMappingService
                .Setup(m => m.GetContentType(zipFileName))
                .Returns(zipMimeType);

            // Act
            var actualResult = await _exchangeDocumentDownloadService.DownloadAgencyExchangeDocumentsByListOptions(
                listOptions,
                userInfo,
                teams);

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
            actualDocumentDownloadRequest.Should().BeEquivalentTo(new ExchangeDocumentDownloadRequest
            {
                UserInfo = userInfo,
                ListOptions = listOptions
            });
        }
    }
}