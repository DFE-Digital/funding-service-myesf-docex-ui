using Microsoft.Extensions.Options;
using Pds.Core.ApiClient;
using Pds.Core.ApiClient.Exceptions;
using Pds.Core.ApiClient.Interfaces;
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
    /// A class exposing methods for interacting with the Document Exchange Agency API.
    /// </summary>
    public class AgencyApiClient : BaseApiClient<DataApiClientConfiguration>, IAgencyApiClient
    {
        private readonly ILoggerAdapter<AgencyApiClient> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgencyApiClient"/> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="logger">The logger.</param>
        public AgencyApiClient(
            IAuthenticationService<DataApiClientConfiguration> authenticationService,
            HttpClient httpClient,
            IOptions<DocumentExchangeServicesConfiguration> configurationOptions,
            ILoggerAdapter<AgencyApiClient> logger)
            : base(authenticationService, httpClient, Options.Create(configurationOptions.Value.DataApiClient))
        {
            _logger = logger;
            httpClient.Timeout = TimeSpan.FromSeconds(configurationOptions.Value.DataApiClient.Timeout);
        }

        /// <inheritdoc/>
        public async Task<byte[]> DownloadTeamDocument(string team, string fileName)
        {
            return await Get<byte[]>($"/api/agency/teams/{team}/documents/{fileName}");
        }

        /// <inheritdoc/>
        public async Task<FileShareSummary> GetTeamSummary(string teams)
        {
            return await Get<FileShareSummary>($"/api/agency/teams/{teams}/summary");
        }

        /// <inheritdoc/>
        public async Task<ListResult<AgencyDocument>> ListTeamDocuments(
            string team,
            AgencyListDocumentOptions options)
        {
            return await Post<AgencyListDocumentOptions, ListResult<AgencyDocument>>(
                $"/api/agency/teams/{team}/documents",
                options);
        }

        /// <inheritdoc/>
        public async Task<KeyValuePair<Product, int>> PublishTeamDocuments(
            string team,
            AgencyPublishRequest agencyPublishRequest)
        {
            return await Post<AgencyPublishRequest, KeyValuePair<Product, int>>(
                $"/api/agency/teams/{team}/documents/publish",
                agencyPublishRequest);
        }

        /// <inheritdoc/>
        public async Task RemoveTeamDocuments(string team, IEnumerable<string> fileNames)
        {
            await Post($"/api/agency/teams/{team}/documents/remove", fileNames);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DocumentReference>> GetPreviousDocumentVersionReferences(string team, IEnumerable<DocumentReference> documentReferences)
        {
            return await Post<IEnumerable<DocumentReference>, List<DocumentReference>>(
                $"/api/agency/teams/{team}/documents/previous-document-references",
                documentReferences);
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