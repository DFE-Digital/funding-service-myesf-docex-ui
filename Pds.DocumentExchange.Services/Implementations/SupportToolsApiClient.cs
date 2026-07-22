using Microsoft.Extensions.Options;
using Pds.Core.ApiClient;
using Pds.Core.ApiClient.Exceptions;
using Pds.Core.ApiClient.Interfaces;
using Pds.Core.Logging;
using Pds.DocumentExchange.Services.Configuration;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Implementations
{
    /// <summary>
    /// The support tools API client implementation.
    /// </summary>
    public class SupportToolsApiClient : BaseApiClient<DataApiClientConfiguration>, ISupportToolsApiClient
    {
        private const string UrlBase = "/api/support-tools";

        private readonly ILoggerAdapter<SupportToolsApiClient> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportToolsApiClient"/> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="logger">The logger.</param>
        public SupportToolsApiClient(
             IAuthenticationService<DataApiClientConfiguration> authenticationService,
             HttpClient httpClient,
             IOptions<DocumentExchangeServicesConfiguration> configurationOptions,
             ILoggerAdapter<SupportToolsApiClient> logger)
            : base(authenticationService, httpClient, Options.Create(configurationOptions.Value.DataApiClient))
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<ListResult<PublishedBatch>> GetDocumentsPublishedByDfE(int pageNumber, int pageSize)
            => Get<ListResult<PublishedBatch>>($"{UrlBase}/documents-published-by-esfa?pageNumber={pageNumber}&pageSize={pageSize}");

        /// <inheritdoc/>
        public Task<byte[]> DownloadDocumentsPublishedCsv(Guid parentBatchIdentifier)
            => Get<byte[]>($"{UrlBase}/documents-published-csv/{parentBatchIdentifier}");

        /// <inheritdoc/>
        public Task<byte[]> DownloadNotificationRecipientsCsv(Guid parentBatchIdentifier)
            => Get<byte[]>($"{UrlBase}/notification-recipients-csv/{parentBatchIdentifier}");

        /// <inheritdoc/>
        public Task<ListResult<ExchangeDocument>> DeleteDocumentsSearch(ExchangeDocumentDirection exchangeDocumentDirection, int ukprn, string fileType, string year)
            => Get<ListResult<ExchangeDocument>>($"{UrlBase}/delete-documents-search?direction={exchangeDocumentDirection}&ukprn={ukprn}&fileType={fileType}&year={year}");

        /// <inheritdoc/>
        public Task<byte[]> DownloadMIReport(DateTime from, DateTime to)
            => Get<byte[]>($"{UrlBase}/download-mireport?from={from.ToString("yyyy-MM-dd")}&to={to.ToString("yyyy-MM-dd")}");

        /// <inheritdoc/>
        protected override Action<ApiGeneralException> FailureAction
            => exception =>
            {
                if (exception.ResponseStatusCode != HttpStatusCode.NotFound)
                {
                    _logger.LogError(exception, exception.Message);
                    throw exception;
                }
            };
    }
}