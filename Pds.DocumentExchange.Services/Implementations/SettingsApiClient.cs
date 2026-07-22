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
    /// A class exposing methods for interacting with the Document Exchange Settings API.
    /// </summary>
    public class SettingsApiClient : BaseApiClient<DataApiClientConfiguration>, ISettingsApiClient
    {
        private readonly ILoggerAdapter<SettingsApiClient> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsApiClient"/> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="logger">The logger.</param>
        public SettingsApiClient(
            IAuthenticationService<DataApiClientConfiguration> authenticationService,
            HttpClient httpClient,
            IOptions<DocumentExchangeServicesConfiguration> configurationOptions,
            ILoggerAdapter<SettingsApiClient> logger)
            : base(authenticationService, httpClient, Options.Create(configurationOptions.Value.DataApiClient))
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Product>> GetProductsThatOrganisationsCanUpload()
        {
            return await Get<IEnumerable<Product>>(
                "api/settings/products-that-organisations-can-upload");
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await Get<IEnumerable<Product>>(
                "api/settings/products");
        }

        /// <inheritdoc/>
        public async Task<Product> GetProduct(int identifier)
        {
            return await Get<Product>(
                $"api/settings/product/{identifier}");
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<AgencyTeam>> GetTeams()
        {
            return await Get<IEnumerable<AgencyTeam>>(
                "api/settings/teams");
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<FileExtensionInfo>> GetFileExtensions()
        {
            return await Get<IEnumerable<FileExtensionInfo>>(
                "api/settings/file-extensions");
        }

        /// <inheritdoc/>
        public async Task<int> GetMaxFileUploadSize()
        {
            return await Get<int>(
                $"api/settings/max-file-upload-size");
        }

        /// <inheritdoc/>
        public async Task<Product> AddOrUpdateProduct(int oldIdentifier, Product newProductValue)
        {
            return await Put<Product, Product>(
                $"api/settings/products/{oldIdentifier}", newProductValue);
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