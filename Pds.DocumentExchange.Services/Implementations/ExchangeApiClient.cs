using Microsoft.Extensions.Options;
using Pds.Core.ApiClient;
using Pds.Core.ApiClient.Exceptions;
using Pds.Core.ApiClient.Interfaces;
using Pds.Core.Common.Organisation.Models;
using Pds.Core.Logging;
using Pds.DocumentExchange.Services.Configuration;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Implementations
{
    /// <summary>
    /// A class exposing methods for interacting with the Document Exchange "Exchange" API.
    /// </summary>
    public class ExchangeApiClient : BaseApiClient<DataApiClientConfiguration>, IExchangeApiClient
    {
        private readonly ILoggerAdapter<ExchangeApiClient> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeApiClient"/> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="logger">The logger.</param>
        public ExchangeApiClient(
            IAuthenticationService<DataApiClientConfiguration> authenticationService,
            HttpClient httpClient,
            IOptions<DocumentExchangeServicesConfiguration> configurationOptions,
            ILoggerAdapter<ExchangeApiClient> logger)
            : base(authenticationService, httpClient, Options.Create(configurationOptions.Value.DataApiClient))
        {
            _logger = logger;
            httpClient.Timeout = TimeSpan.FromSeconds(configurationOptions.Value.DataApiClient.Timeout);
        }

        /// <inheritdoc/>
        public async Task<byte[]> DownloadDocuments(ExchangeDocumentDownloadRequest downloadRequest)
        {
            return await Post<ExchangeDocumentDownloadRequest, byte[]>(
                "/api/exchange/documents/download", downloadRequest);
        }

        /// <inheritdoc/>
        public async Task<byte[]> DownloadAgencyTeamDocuments(
            string team, ExchangeDocumentDownloadRequest downloadRequest)
        {
            return await Post<ExchangeDocumentDownloadRequest, byte[]>(
                $"/api/exchange/teams/{team}/documents/download", downloadRequest);
        }

        /// <inheritdoc/>
        public async Task<ListResult<ExchangeDocument>> GetAgencyTeamDocuments(
            string team, ExchangeListDocumentOptions options)
        {
            return await Post<ExchangeListDocumentOptions, ListResult<ExchangeDocument>>(
                $"/api/exchange/teams/{team}/documents", options);
        }

        /// <inheritdoc/>
        public async Task<Summary> GetAgencyTeamSummary(string teams)
        {
            return await Get<Summary>(
                $"/api/exchange/teams/{teams}/team-summary");
        }

        /// <inheritdoc/>
        public async Task<int> GetCurrentProductVersionForOrganisation(
            OrganisationIdentifier organisationIdentifier, string productIdentifier)
        {
            return await Post<OrganisationIdentifier, int>(
                $"api/exchange/current-product-version-for-organisation?productIdentifier={productIdentifier}",
                organisationIdentifier);
        }

        /// <inheritdoc/>
        public async Task<ListResult<ExchangeDocument>> GetOrganisationDocuments(
            ExchangeListOrganisationDocumentOptions options)
        {
            return await Post<ExchangeListOrganisationDocumentOptions, ListResult<ExchangeDocument>>(
                $"api/exchange/documents", options);
        }

        /// <inheritdoc/>
        public async Task<Summary> GetOrganisationUserSummary(UserInfo userInfo)
        {
            return await Post<UserInfo, Summary>(
                $"api/exchange/summary", userInfo);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ExchangeDocument>> DeleteDocuments(ExchangeDocumentDeleteRequest deleteRequest)
        {
            return await Post<ExchangeDocumentDeleteRequest, IEnumerable<ExchangeDocument>>(
                $"api/exchange/documents/delete", deleteRequest);
        }

        /// <inheritdoc/>
        public async Task<ExchangeDocument> DeleteDocument(ExchangeDocumentDeleteRequest deleteRequest)
        {
            return await Post<ExchangeDocumentDeleteRequest, ExchangeDocument>(
                $"api/exchange/documents/delete-single", deleteRequest);
        }

        /// <inheritdoc/>
        public async Task<MIReportData> MIReport(MIReportOptions options)
        {
            return await Post<MIReportOptions, MIReportData>(
                $"api/exchange/mi-report", options);
        }

        /// <inheritdoc/>
        protected override Action<ApiGeneralException> FailureAction
            => exception =>
            {
                _logger.LogError(exception, exception.Message);
                throw exception;
            };
    }
}