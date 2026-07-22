using Microsoft.Extensions.Options;
using Pds.Core.ApiClient;
using Pds.Core.ApiClient.Exceptions;
using Pds.Core.ApiClient.Interfaces;
using Pds.Core.Logging;
using Pds.DocumentExchange.Services.Configuration;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Implementations
{
    /// <summary>
    /// A class exposing methods for interacting with the Document Exchange "Upload" API.
    /// </summary>
    public class UploadApiClient : BaseApiClient<DataApiClientConfiguration>, IUploadApiClient
    {
        private readonly ILoggerAdapter<UploadApiClient> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadApiClient"/> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="logger">The logger.</param>
        public UploadApiClient(
            IAuthenticationService<DataApiClientConfiguration> authenticationService,
            HttpClient httpClient,
            IOptions<DocumentExchangeServicesConfiguration> configurationOptions,
            ILoggerAdapter<UploadApiClient> logger)
            : base(authenticationService, httpClient, Options.Create(configurationOptions.Value.DataApiClient))
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task UploadDocument(UploadDocumentRequest request)
        {
            await Post("/api/upload/document", request);
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