using Microsoft.Extensions.Options;
using Pds.Core.ApiClient;
using Pds.Core.ApiClient.Interfaces;
using Pds.Core.Common.Organisation.Enums;
using Pds.Core.Common.Organisation.Models;
using Pds.Core.Logging;
using Pds.Core.Utils.Helpers;
using Pds.DocumentExchange.Services.Configuration;
using Pds.DocumentExchange.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Implementations
{
    /// <summary>
    /// A class exposing methods for interacting with the Document Exchange API Organisation controller.
    /// </summary>
    public class OrganisationApiClient : BaseApiClient<DataApiClientConfiguration>, IOrganisationApiClient
    {
        private readonly ILoggerAdapter<OrganisationApiClient> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganisationApiClient"/> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="logger">The logger.</param>
        public OrganisationApiClient(
            IAuthenticationService<DataApiClientConfiguration> authenticationService,
            HttpClient httpClient,
            IOptions<DocumentExchangeServicesConfiguration> configurationOptions,
            ILoggerAdapter<OrganisationApiClient> logger)
            : base(authenticationService, httpClient, Options.Create(configurationOptions.Value.DataApiClient))
        {
            _logger = logger;
            httpClient.Timeout = TimeSpan.FromSeconds(configurationOptions.Value.DataApiClient.Timeout);
        }

        /// <inheritdoc/>
        public async Task<Organisation> GetOrganisation(OrganisationIdentifier identifier)
        {
            if (identifier.Type == OrganisationIdentifierType.Ukprn && !string.IsNullOrWhiteSpace(identifier.Value))
            {
                return await Get<Organisation>($"/api/organisation/{identifier.Value}");
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyCollection<Organisation>> Search(string searchTerm, int maxResults)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                return await Get<IReadOnlyCollection<Organisation>>($"/api/organisation/search/{searchTerm}/{maxResults}");
            }

            return Array.Empty<Organisation>().AsSafeReadOnlyList();
        }
    }
}
