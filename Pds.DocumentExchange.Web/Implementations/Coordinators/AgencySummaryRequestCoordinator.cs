using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Interfaces.Coordinators;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models.Agency;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Implementations.Coordinators
{
    /// <inheritdoc cref="IAgencySummaryRequestCoordinator"/>
    public class AgencySummaryRequestCoordinator : IAgencySummaryRequestCoordinator
    {
        private readonly IUserInformationProvider _userInfoProvider;
        private readonly IAgencyApiClient _agencyApiClient;
        private readonly IExchangeApiClient _exchangeApiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgencySummaryRequestCoordinator"/> class.
        /// </summary>
        /// <param name="userInfoProvider"><see cref="IUserInformationProvider"/>.</param>
        /// <param name="agencyApiClient"><see cref="IAgencyApiClient"/>.</param>
        /// <param name="exchangeApiClient"><see cref="IExchangeApiClient"/>.</param>
        public AgencySummaryRequestCoordinator(
            IUserInformationProvider userInfoProvider,
            IAgencyApiClient agencyApiClient,
            IExchangeApiClient exchangeApiClient)
        {
            _userInfoProvider = userInfoProvider;
            _agencyApiClient = agencyApiClient;
            _exchangeApiClient = exchangeApiClient;
        }

        /// <inheritdoc/>
        public async Task<FileShare> GetFileSharePage()
        {
            var teams = await _userInfoProvider.GetCurrentUserAgencyTeams();
            var fileShareSummary = await _agencyApiClient.GetTeamSummary(teams);

            return new FileShare
            {
                TotalCountOfDocuments = fileShareSummary.TotalCount,
                CountOfInvalidDocuments = fileShareSummary.InvalidCount,
                CountOfValidDocuments = fileShareSummary.ValidCount
            };
        }

        /// <inheritdoc/>
        public async Task<AgencyHomePageData> GetHomePageData()
        {
            var fileShareCount = 0;
            var newDocsCount = 0;
            var canSeeTeamsData = false;

            var teams = await _userInfoProvider.GetCurrentUserAgencyTeams();

            if (!string.IsNullOrEmpty(teams))
            {
                var fileShareSummaryTask = _agencyApiClient.GetTeamSummary(teams);
                var teamSummaryTask = _exchangeApiClient.GetAgencyTeamSummary(teams);

                await Task.WhenAll(fileShareSummaryTask, teamSummaryTask);

                fileShareCount = fileShareSummaryTask.Result.TotalCount;
                newDocsCount = teamSummaryTask.Result.CountOfNewDocuments ?? 0;
                canSeeTeamsData = true;
            }

            var canViewAsOrganisation = await _userInfoProvider.CurrentUserCanViewAsOrganisation();
            var isAdmin = await _userInfoProvider.CurrentUserIsAdminUser();
            var canViewSupportTools = await _userInfoProvider.CurrentUserCanAccessToSupportTools();

            return new AgencyHomePageData
            {
                TotalCountOfDocumentsInFileShare = fileShareCount,
                CountOfNewDocuments = newDocsCount,
                ShowViewAsOrganisation = canViewAsOrganisation,
                ShowSettingsOption = isAdmin,
                ShowDocumentOptions = canSeeTeamsData,
                ShowToolsOption = canViewSupportTools
            };
        }
    }
}