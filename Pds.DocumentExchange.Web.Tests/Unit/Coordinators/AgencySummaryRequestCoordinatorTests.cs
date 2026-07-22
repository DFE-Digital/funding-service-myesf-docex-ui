using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Implementations.Coordinators;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models.Agency;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Tests.Unit.Coordinators
{
    [TestClass, TestCategory("Unit")]
    public sealed class AgencySummaryRequestCoordinatorTests
    {
        private const string FakeTeamsList = "a,b,c";

        private readonly IUserInformationProvider _userInfoProvider
            = Mock.Of<IUserInformationProvider>(MockBehavior.Strict);

        private readonly IExchangeApiClient _exchangeApiClient
            = Mock.Of<IExchangeApiClient>(MockBehavior.Strict);

        private readonly IAgencyApiClient _agencyApiClient
            = Mock.Of<IAgencyApiClient>(MockBehavior.Strict);

        [TestMethod]
        [DataRow(0, 0, 0, false, false, false)]
        [DataRow(1, 1, 0, false, true, true)]
        [DataRow(0, 0, null, false, false, false)]
        [DataRow(1, 1, null, false, true, true)]
        [DataRow(0, 0, 1, false, false, false)]
        [DataRow(1, 1, 1, true, true, true)]
        [DataRow(12, 21, 24, true, false, false)]
        [DataRow(145, 43, 87, true, true, true)]
        [DataRow(245, 66, 142, true, false, false)]
        [DataRow(24, 56, 26, true, true, true)]
        public async Task GetHomePageData_MakesApiCallsAndReturnsExpectedData(
            int invalidCount, int validCount, int? newDocuments, bool canViewAsOrg, bool isAdmin, bool canViewSupportTools)
        {
            // Arrange
            Mock.Get(_userInfoProvider)
                .Setup(i => i.GetCurrentUserAgencyTeams())
                .ReturnsAsync(FakeTeamsList);

            Mock.Get(_userInfoProvider)
                .Setup(i => i.CurrentUserCanViewAsOrganisation())
                .ReturnsAsync(canViewAsOrg);

            Mock.Get(_userInfoProvider)
                .Setup(i => i.CurrentUserCanAccessToSupportTools())
                .ReturnsAsync(canViewSupportTools);

            Mock.Get(_userInfoProvider)
                .Setup(i => i.CurrentUserIsAdminUser())
                .ReturnsAsync(isAdmin);

            Mock.Get(_agencyApiClient)
                .Setup(a => a.GetTeamSummary(FakeTeamsList))
                .ReturnsAsync(new FileShareSummary
                {
                    InvalidCount = invalidCount,
                    ValidCount = validCount
                });

            Mock.Get(_exchangeApiClient)
                .Setup(a => a.GetAgencyTeamSummary(FakeTeamsList))
                .ReturnsAsync(new Summary
                {
                    CountOfNewDocuments = newDocuments
                });

            var coordinator = GetTestCoordinator();

            // Act
            var result = await coordinator.GetHomePageData();

            // Assert
            result.Should().BeEquivalentTo(
                new AgencyHomePageData
                {
                    CountOfNewDocuments = newDocuments ?? 0,
                    TotalCountOfDocumentsInFileShare = invalidCount + validCount,
                    ShowViewAsOrganisation = canViewAsOrg,
                    ShowSettingsOption = isAdmin,
                    ShowDocumentOptions = true,
                    ShowToolsOption = canViewSupportTools
                });

            Mock.VerifyAll(
                Mock.Get(_userInfoProvider),
                Mock.Get(_agencyApiClient),
                Mock.Get(_exchangeApiClient));
        }

        [TestMethod]
        [DataRow(false, false, false)]
        [DataRow(false, true, true)]
        [DataRow(true, false, false)]
        [DataRow(true, true, true)]
        public async Task GetHomePageData_ForNonTeamUser_ReturnsExpectedData(bool canViewAsOrg, bool isAdmin, bool canViewSupportTools)
        {
            // Arrange
            Mock.Get(_userInfoProvider)
                .Setup(i => i.GetCurrentUserAgencyTeams())
                .ReturnsAsync(string.Empty);

            Mock.Get(_userInfoProvider)
                .Setup(i => i.CurrentUserCanViewAsOrganisation())
                .ReturnsAsync(canViewAsOrg);

            Mock.Get(_userInfoProvider)
              .Setup(i => i.CurrentUserCanAccessToSupportTools())
              .ReturnsAsync(canViewSupportTools);

            Mock.Get(_userInfoProvider)
                .Setup(i => i.CurrentUserIsAdminUser())
                .ReturnsAsync(isAdmin);

            var coordinator = GetTestCoordinator();

            // Act
            var result = await coordinator.GetHomePageData();

            // Assert
            result.Should().BeEquivalentTo(
                new AgencyHomePageData
                {
                    CountOfNewDocuments = 0,
                    TotalCountOfDocumentsInFileShare = 0,
                    ShowViewAsOrganisation = canViewAsOrg,
                    ShowSettingsOption = isAdmin,
                    ShowDocumentOptions = false,
                    ShowToolsOption = canViewSupportTools
                });

            Mock.VerifyAll(
                Mock.Get(_userInfoProvider));
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 291)]
        [DataRow(145, 53)]
        [DataRow(235, 64)]
        [DataRow(234, 46)]
        public async Task GetFileSharePage__MakesExpectedApiCallsAndReturnsExpectedView(int invalidCount, int validCount)
        {
            // Arrange
            Mock.Get(_userInfoProvider)
                .Setup(i => i.GetCurrentUserAgencyTeams())
                .ReturnsAsync(FakeTeamsList);

            Mock.Get(_agencyApiClient)
                .Setup(a => a.GetTeamSummary(FakeTeamsList))
                .ReturnsAsync(new FileShareSummary
                {
                    InvalidCount = invalidCount,
                    ValidCount = validCount
                });

            var coordinator = GetTestCoordinator();

            // Act
            var result = await coordinator.GetFileSharePage();

            // Assert
            result.Should().BeEquivalentTo(
                new FileShare
                {
                    TotalCountOfDocuments = invalidCount + validCount,
                    CountOfInvalidDocuments = invalidCount,
                    CountOfValidDocuments = validCount
                });

            Mock.VerifyAll(
                Mock.Get(_userInfoProvider),
                Mock.Get(_agencyApiClient));
        }


        private AgencySummaryRequestCoordinator GetTestCoordinator()
        {
            return new AgencySummaryRequestCoordinator(
                _userInfoProvider,
                _agencyApiClient,
                _exchangeApiClient);
        }
    }
}