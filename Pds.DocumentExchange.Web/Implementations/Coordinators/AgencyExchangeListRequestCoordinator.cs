using Microsoft.Extensions.Options;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Interfaces.Converters;
using Pds.DocumentExchange.Web.Interfaces.Coordinators;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models;
using Pds.DocumentExchange.Web.Models.Agency;
using System.Linq;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Implementations.Coordinators
{
    /// <inheritdoc cref="IAgencyExchangeListRequestCoordinator"/>
    public class AgencyExchangeListRequestCoordinator : IAgencyExchangeListRequestCoordinator
    {
        private readonly IUserInformationProvider _userInfoProvider;
        private readonly IExchangeApiClient _exchangeApiClient;
        private readonly IListHelper _listHelper;
        private readonly IDocumentModelConverter _documentConverter;
        private readonly DocumentExchangeConfiguration _configuration;
        private readonly IExchangeDocumentDownloadService _documentDownloadService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgencyExchangeListRequestCoordinator"/> class.
        /// </summary>
        /// <param name="userInfoProvider"><see cref="IUserInformationProvider"/>.</param>
        /// <param name="exchangeApiClient"><see cref="IExchangeApiClient"/>.</param>
        /// <param name="listHelper"><see cref="IListHelper"/>.</param>
        /// <param name="documentConverter"><see cref="IDocumentModelConverter"/>.</param>
        /// <param name="configurationOptions">Options containing <see cref="DocumentExchangeConfiguration"/>.</param>
        /// <param name="documentDownloadService"><see cref="IExchangeDocumentDownloadService"/>.</param>
        public AgencyExchangeListRequestCoordinator(
            IUserInformationProvider userInfoProvider,
            IExchangeApiClient exchangeApiClient,
            IListHelper listHelper,
            IDocumentModelConverter documentConverter,
            IOptions<DocumentExchangeConfiguration> configurationOptions,
            IExchangeDocumentDownloadService documentDownloadService)
        {
            _userInfoProvider = userInfoProvider;
            _exchangeApiClient = exchangeApiClient;
            _listHelper = listHelper;
            _documentConverter = documentConverter;
            _documentDownloadService = documentDownloadService;
            _configuration = configurationOptions.Value;
        }

        /// <inheritdoc/>
        public async Task<DownloadDocuments> GetDocumentsToDownload(ListRequest request)
        {
            var teams = await _userInfoProvider.GetCurrentUserAgencyTeams();

            var filterOptions = _listHelper.GetFilterOptions(request, true);

            var options = new ExchangeListDocumentOptions
            {
                DocumentStatusOption = ExchangeDocumentDirection.SentByOrganisation,
                PageSize = _configuration.ListPageSize,
                PageNumber = request?.Page ?? 1,
                FilterOptions = filterOptions
            };

            var teamDocuments = await _exchangeApiClient.GetAgencyTeamDocuments(teams, options);

            var listItems = teamDocuments.Items
                .Select(document => _documentConverter.CreateAgencyExchangeDocumentFromExchangeDocument(document));

            return new DownloadDocuments
            {
                Pagination = _listHelper.GetPaginationViewModel(request, teamDocuments),
                ListItems = listItems,
                FilterCategories = _listHelper.GetFilterCategories(teamDocuments),
                AnyDocumentsAvailable = _listHelper.AnyDocumentsAvailable(request, teamDocuments),
                UserIsAdvancedAgencyUser = await _userInfoProvider.CurrentUserIsAdvancedAgencyUser()
            };
        }

        /// <inheritdoc/>
        public async Task<DocumentListUpdateData> GetDocumentsToDownloadData(ListRequest request)
            => _listHelper.GetDocumentListUpdateData(await GetDocumentsToDownload(request));

        /// <inheritdoc/>
        public async Task<DownloadedFile> DownloadDocuments(ListRequest request)
        {
            var userInfo = await _userInfoProvider.GetCurrentUserInfo();
            var teams = await _userInfoProvider.GetCurrentUserAgencyTeams();
            var filterOptions = _listHelper.GetFilterOptions(request);

            var options = new ExchangeListDocumentOptions
            {
                DocumentStatusOption = ExchangeDocumentDirection.SentByOrganisation,
                PageSize = int.MaxValue,
                PageNumber = 1,
                FilterOptions = filterOptions
            };

            var file = await _documentDownloadService.DownloadAgencyExchangeDocumentsByListOptions(
                options,
                userInfo,
                teams);

            return file;
        }
    }
}