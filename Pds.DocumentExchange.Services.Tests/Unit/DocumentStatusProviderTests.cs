using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.Core.Common.Organisation.Enums;
using Pds.Core.Common.Organisation.Models;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Implementations;
using Pds.DocumentExchange.Services.Models;
using System;
using System.Linq;

namespace Pds.DocumentExchange.Services.Tests.Unit
{
    [TestClass]
    [TestCategory("Unit")]
    public class DocumentStatusProviderTests
    {
        private readonly DocumentStatusProvider _documentStatusProvider = new DocumentStatusProvider();

        [TestMethod]
        public void GetDownloadStatus_WhenDocumentEventsIsNull_ReturnNewStatus()
        {
            // Arrange
            var expectedResult = new DocumentStatusInfo
            {
                Status = DownloadDocumentStatus.New
            };

            // Act
            var result = _documentStatusProvider.GetDownloadStatus(null);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void GetDownloadStatus_WhenDocumentEventsIsEmpty_ReturnNewStatus()
        {
            // Arrange
            var expectedResult = new DocumentStatusInfo
            {
                Status = DownloadDocumentStatus.New
            };

            // Act
            var result = _documentStatusProvider.GetDownloadStatus(Enumerable.Empty<ExchangeDocumentEvent>());

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void GetDownloadStatus_WhenDocumentEventsDoesntContainDownloadedByReceiver_ReturnNewStatus()
        {
            // Arrange
            var events = new[]
            {
                new ExchangeDocumentEvent
                {
                    EventType = ExchangeDocumentEventType.DownloadedBySender,
                    EventDateTime = new DateTime(2020, 9, 1),
                    UserInfo = CreateTestUserInfo()
                }
            };

            var expectedResult = new DocumentStatusInfo
            {
                Status = DownloadDocumentStatus.New
            };

            // Act
            var result = _documentStatusProvider.GetDownloadStatus(events);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void GetDownloadStatus_WhenDocumentEventsContainsDownloadedByReceiver_ReturnDownloadedStatus()
        {
            // Arrange
            var exchangeDocumentEvent = new ExchangeDocumentEvent
            {
                EventType = ExchangeDocumentEventType.DownloadedByReceiver,
                EventDateTime = new DateTime(2020, 9, 1),
                UserInfo = CreateTestUserInfo()
            };

            var events = new[]
            {
               exchangeDocumentEvent
            };

            var expectedResult = new DocumentStatusInfo
            {
                Status = DownloadDocumentStatus.Downloaded,
                DownloadedTime = exchangeDocumentEvent.EventDateTime,
                DownloadedBy = exchangeDocumentEvent.UserInfo.FullName
            };

            // Act
            var result = _documentStatusProvider.GetDownloadStatus(events);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void GetDownloadStatus_WhenDocumentEventsContainsMultipleDownloadedByReceiver_ReturnDownloadedStatus()
        {
            // Arrange
            var exchangeDocumentEvent1 = new ExchangeDocumentEvent
            {
                EventType = ExchangeDocumentEventType.DownloadedByReceiver,
                EventDateTime = new DateTime(2020, 1, 1),
                UserInfo = CreateTestUserInfo()
            };

            var exchangeDocumentEvent2 = new ExchangeDocumentEvent
            {
                EventType = ExchangeDocumentEventType.DownloadedByReceiver,
                EventDateTime = new DateTime(2020, 9, 1),
                UserInfo = CreateTestUserInfo()
            };

            var events = new[]
            {
               exchangeDocumentEvent1,
               exchangeDocumentEvent2
            };

            var expectedResult = new DocumentStatusInfo
            {
                Status = DownloadDocumentStatus.Downloaded,
                DownloadedTime = exchangeDocumentEvent2.EventDateTime,
                DownloadedBy = exchangeDocumentEvent2.UserInfo.FullName
            };

            // Act
            var result = _documentStatusProvider.GetDownloadStatus(events);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        private UserInfo CreateTestUserInfo()
            => new UserInfo
            {
                Principal = "user-principal",
                FullName = "user-full-name",
                EmailAddress = "user-email@education.gov.uk",
                OrganisationInfo = new OrganisationInfo
                {
                    Name = "organisation-name",
                    OrganisationIdentifier = new OrganisationIdentifier
                    {
                        Type = OrganisationIdentifierType.Ukprn,
                        Value = "12345678"
                    }
                }
            };
    }
}