using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Services.Models.Filters;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Implementations.Coordinators;
using Pds.DocumentExchange.Web.Interfaces.Converters;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models;
using Pds.DocumentExchange.Web.Models.Agency;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Tests.Unit.Coordinators
{
    [TestClass, TestCategory("Unit")]
    public sealed class AgencyExchangeListRequestCoordinatorTests
    {
        private const string FakeTeamsList = "a,b,c";

        private static readonly DocumentExchangeConfiguration _configuration
            = new DocumentExchangeConfiguration
            {
                ListPageSize = 37
            };

        private readonly IUserInformationProvider _userInfoProvider
            = Mock.Of<IUserInformationProvider>(MockBehavior.Strict);

        private readonly IExchangeApiClient _exchangeApiClient
            = Mock.Of<IExchangeApiClient>(MockBehavior.Strict);

        private readonly IListHelper _listHelper
            = Mock.Of<IListHelper>(MockBehavior.Strict);

        private readonly IDocumentModelConverter _documentConverter
            = Mock.Of<IDocumentModelConverter>(MockBehavior.Strict);

        private readonly IExchangeDocumentDownloadService _documentDownloadService
            = Mock.Of<IExchangeDocumentDownloadService>(MockBehavior.Strict);

        [TestMethod, DynamicData(nameof(GetDocumentsToDownload_TestData))]
        public async Task GetDocumentsToDownload_ReturnsExpected(ListRequest request, int numberOfDocuments, bool advancedUser)
        {
            // Arrange
            var fakeTeams = "team1,team2";

            ExchangeListDocumentOptions actualOptions = null;

            var fakeDocuments = Enumerable
                .Range(0, numberOfDocuments)
                .Select(id => new ExchangeDocument
                {
                    DocumentReference = new DocumentReference
                    {
                        FileName = $"file-{id}.jpg"
                    }
                }).ToList();

            var fakeListResult = new ListResult<ExchangeDocument>
            {
                Items = fakeDocuments
            };

            var fakeFilterOptions = Enumerable.Empty<IFilterOption>();
            var fakePagination = new PaginationViewModel();
            var fakeFilterCategories = Enumerable.Empty<IFilterCategoryViewModel>();
            var fakeAgencyDocuments = Enumerable
                .Range(0, numberOfDocuments)
                .Select(id => new AgencyExchangeDocument { FileName = $"file-{id}.jpg" })
                .ToList();

            var expectedResult = new DownloadDocuments
            {
                Pagination = fakePagination,
                ListItems = fakeAgencyDocuments,
                FilterCategories = fakeFilterCategories,
                AnyDocumentsAvailable = numberOfDocuments > 0,
                UserIsAdvancedAgencyUser = advancedUser
            };

            Mock.Get(_userInfoProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync(fakeTeams);

            Mock.Get(_userInfoProvider)
                .Setup(u => u.CurrentUserIsAdvancedAgencyUser())
                .ReturnsAsync(advancedUser);

            Mock.Get(_exchangeApiClient)
                .Setup(e => e.GetAgencyTeamDocuments(
                    fakeTeams,
                    It.IsAny<ExchangeListDocumentOptions>()))
                .ReturnsAsync((string teams, ExchangeListDocumentOptions options) =>
                {
                    actualOptions = options;
                    return fakeListResult;
                });

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetFilterOptions(request, true))
                .Returns(fakeFilterOptions);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetPaginationViewModel(request, fakeListResult))
                .Returns(fakePagination);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetFilterCategories(fakeListResult))
                .Returns(fakeFilterCategories);

            Mock.Get(_listHelper)
                .Setup(lh => lh.AnyDocumentsAvailable(request, fakeListResult))
                .Returns(numberOfDocuments > 0);

            if (numberOfDocuments > 0)
            {
                Mock.Get(_documentConverter)
                    .Setup(dc => dc.CreateAgencyExchangeDocumentFromExchangeDocument(It.IsAny<ExchangeDocument>()))
                    .Returns((ExchangeDocument inputDocument) =>
                    {
                        var index = fakeDocuments.IndexOf(inputDocument);
                        return fakeAgencyDocuments.ElementAt(index);
                    });
            }

            var coordinator = GetTestCoordinator();

            // Act
            var actual = await coordinator.GetDocumentsToDownload(request);

            // Assert
            actual.Should().BeEquivalentTo(expectedResult);

            actualOptions.Should().BeEquivalentTo(
                new ExchangeListDocumentOptions
                {
                    DocumentStatusOption = ExchangeDocumentDirection.SentByOrganisation,
                    PageSize = _configuration.ListPageSize,
                    PageNumber = request?.Page ?? 1,
                    FilterOptions = fakeFilterOptions
                });

            Mock.VerifyAll(
                Mock.Get(_userInfoProvider),
                Mock.Get(_exchangeApiClient),
                Mock.Get(_listHelper),
                Mock.Get(_documentConverter));
        }

        [TestMethod, DynamicData(nameof(GetDocumentsToDownload_TestData))]
        public async Task GetDocumentsToDownloadData_ReturnsExpected(ListRequest request, int numberOfDocuments, bool advancedUser)
        {
            // Arrange
            var fakeTeams = "team1,team2";

            ExchangeListDocumentOptions actualOptions = null;
            IListViewModel actualManageDocuments = null;

            var fakeDocuments = Enumerable
                .Range(0, numberOfDocuments)
                .Select(id => new ExchangeDocument
                {
                    DocumentReference = new DocumentReference
                    {
                        FileName = $"file-{id}.jpg"
                    }
                }).ToList();

            var fakeListResult = new ListResult<ExchangeDocument>
            {
                Items = fakeDocuments
            };

            var fakeFilterOptions = Enumerable.Empty<IFilterOption>();
            var fakePagination = new PaginationViewModel();
            var fakeFilterCategories = Enumerable.Empty<IFilterCategoryViewModel>();
            var fakeAgencyDocuments = Enumerable
                .Range(0, numberOfDocuments)
                .Select(id => new AgencyExchangeDocument { FileName = $"file-{id}.jpg" })
                .ToList();

            var expectedManageDocuments = new DownloadDocuments
            {
                Pagination = fakePagination,
                ListItems = fakeAgencyDocuments,
                FilterCategories = fakeFilterCategories,
                AnyDocumentsAvailable = numberOfDocuments > 0,
                UserIsAdvancedAgencyUser = advancedUser
            };

            var fakeResult = new DocumentListUpdateData();

            Mock.Get(_userInfoProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync(fakeTeams);

            Mock.Get(_userInfoProvider)
                .Setup(u => u.CurrentUserIsAdvancedAgencyUser())
                .ReturnsAsync(advancedUser);

            Mock.Get(_exchangeApiClient)
                .Setup(e => e.GetAgencyTeamDocuments(
                    fakeTeams,
                    It.IsAny<ExchangeListDocumentOptions>()))
                .ReturnsAsync((string teams, ExchangeListDocumentOptions options) =>
                {
                    actualOptions = options;
                    return fakeListResult;
                });

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetFilterOptions(request, true))
                .Returns(fakeFilterOptions);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetPaginationViewModel(request, fakeListResult))
                .Returns(fakePagination);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetFilterCategories(fakeListResult))
                .Returns(fakeFilterCategories);

            Mock.Get(_listHelper)
                .Setup(lh => lh.AnyDocumentsAvailable(request, fakeListResult))
                .Returns(numberOfDocuments > 0);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetDocumentListUpdateData(It.IsAny<IListViewModel>()))
                .Returns((IListViewModel viewModel) =>
                {
                    actualManageDocuments = viewModel;
                    return fakeResult;
                });

            if (numberOfDocuments > 0)
            {
                Mock.Get(_documentConverter)
                    .Setup(dc => dc.CreateAgencyExchangeDocumentFromExchangeDocument(It.IsAny<ExchangeDocument>()))
                    .Returns((ExchangeDocument inputDocument) =>
                    {
                        var index = fakeDocuments.IndexOf(inputDocument);
                        return fakeAgencyDocuments.ElementAt(index);
                    });
            }

            var coordinator = GetTestCoordinator();

            // Act
            var actual = await coordinator.GetDocumentsToDownloadData(request);

            // Assert
            actual.Should().Be(fakeResult);

            actualManageDocuments.Should().BeEquivalentTo(expectedManageDocuments);

            actualOptions.Should().BeEquivalentTo(
                new ExchangeListDocumentOptions
                {
                    DocumentStatusOption = ExchangeDocumentDirection.SentByOrganisation,
                    PageSize = _configuration.ListPageSize,
                    PageNumber = request?.Page ?? 1,
                    FilterOptions = fakeFilterOptions
                });

            Mock.VerifyAll(
                Mock.Get(_userInfoProvider),
                Mock.Get(_exchangeApiClient),
                Mock.Get(_listHelper),
                Mock.Get(_documentConverter));
        }

        [TestMethod]
        public async Task DownloadDocuments_ReturnsExpectedResultFromDownloadService()
        {
            // Arrange
            var listRequest = new ListRequest();
            var userInfo = new UserInfo();
            var teams = "test-team1,team2";

            var expectedOptions = new ExchangeListDocumentOptions
            {
                DocumentStatusOption = ExchangeDocumentDirection.SentByOrganisation,
                PageSize = int.MaxValue,
                PageNumber = 1,
                FilterOptions = new List<IFilterOption>()
            };

            var expectedResult = new DownloadedFile();

            ExchangeListDocumentOptions actualOptions = null;

            var mockUserInfoProvider = Mock.Get(_userInfoProvider);

            mockUserInfoProvider
                .Setup(u => u.GetCurrentUserInfo())
                .ReturnsAsync(userInfo);

            mockUserInfoProvider
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync(teams);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetFilterOptions(listRequest, false))
                .Returns(expectedOptions.FilterOptions);

            Mock.Get(_documentDownloadService)
                .Setup(
                    dds => dds.DownloadAgencyExchangeDocumentsByListOptions(
                        It.IsAny<ExchangeListDocumentOptions>(),
                        userInfo,
                        teams))
                .ReturnsAsync((ExchangeListDocumentOptions capturedOptions, UserInfo u, string t) =>
                {
                    actualOptions = capturedOptions;
                    return expectedResult;
                });

            var coordinator = GetTestCoordinator();

            // Act
            var actualResult = await coordinator.DownloadDocuments(listRequest);

            // Assert
            actualResult.Should().Be(expectedResult);
            actualOptions.Should().BeEquivalentTo(expectedOptions);

            Mock.VerifyAll(
                mockUserInfoProvider,
                Mock.Get(_listHelper),
                Mock.Get(_documentDownloadService));
        }

        private static IEnumerable<object[]> GetDocumentsToDownload_TestData
        {
            get
            {
                yield return new object[]
                {
                    null,
                    0,
                    false
                };
                yield return new object[]
                {
                    null,
                    1,
                    false
                };
                yield return new object[]
                {
                    null,
                    2,
                    false
                };
                yield return new object[]
                {
                    new ListRequest
                    {
                        Page = 123
                    },
                    37,
                    true
                };
            }
        }

        private AgencyExchangeListRequestCoordinator GetTestCoordinator()
        {
            return new AgencyExchangeListRequestCoordinator(
                _userInfoProvider,
                _exchangeApiClient,
                _listHelper,
                _documentConverter,
                Options.Create(_configuration),
                _documentDownloadService);
        }
    }
}