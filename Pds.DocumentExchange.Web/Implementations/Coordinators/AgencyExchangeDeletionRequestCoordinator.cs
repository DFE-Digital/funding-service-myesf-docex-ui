using Pds.Core.Utils.Helpers;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Services.Models.Filters;
using Pds.DocumentExchange.Web.Enums;
using Pds.DocumentExchange.Web.Interfaces.Converters;
using Pds.DocumentExchange.Web.Interfaces.Coordinators;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models.Agency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Implementations.Coordinators
{
    /// <inheritdoc cref="IAgencyExchangeDeletionRequestCoordinator"/>
    public class AgencyExchangeDeletionRequestCoordinator : IAgencyExchangeDeletionRequestCoordinator
    {
        private readonly IUserInformationProvider _userInfoProvider;
        private readonly IExchangeApiClient _exchangeApiClient;
        private readonly IDocumentModelConverter _documentConverter;
        private readonly IDocumentReferenceService _documentReferenceService;
        private readonly ISupportToolsApiClient _supportToolsApiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgencyExchangeDeletionRequestCoordinator"/> class.
        /// </summary>
        /// <param name="userInfoProvider"><see cref="IUserInformationProvider"/>.</param>
        /// <param name="exchangeApiClient"><see cref="IExchangeApiClient"/>.</param>
        /// <param name="documentConverter"><see cref="IDocumentModelConverter"/>.</param>
        /// <param name="documentReferenceService"><see cref="IDocumentReferenceService"/>.</param>
        /// <param name="supportToolsApiClient">The support tools API client.</param>.
        public AgencyExchangeDeletionRequestCoordinator(
            IUserInformationProvider userInfoProvider,
            IExchangeApiClient exchangeApiClient,
            IDocumentModelConverter documentConverter,
            IDocumentReferenceService documentReferenceService,
            ISupportToolsApiClient supportToolsApiClient)
        {
            _userInfoProvider = userInfoProvider;
            _exchangeApiClient = exchangeApiClient;
            _documentConverter = documentConverter;
            _documentReferenceService = documentReferenceService;
            _supportToolsApiClient = supportToolsApiClient;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<AgencyExchangeDocument>> GetDocumentsToDelete(
            IEnumerable<string> documentReferences,
            IEnumerable<IFilterCategory> filters = null)
        {
            var teams = await _userInfoProvider.GetCurrentUserAgencyTeams();

            var documentReferenceList = documentReferences
                .Select(_documentReferenceService.CreateDocumentReferenceFromString)
                .ToList();

            var filterOptions = CreateFilterOptions(filters);

            var documents = await _exchangeApiClient.GetAgencyTeamDocuments(
                teams,
                new ExchangeListDocumentOptions
                {
                    DocumentReferences = documentReferenceList,
                    DocumentStatusOption = ExchangeDocumentDirection.SentByOrganisation,
                    PageSize = int.MaxValue,
                    FilterOptions = filterOptions
                });

            return documents.Items
                .Select(document => _documentConverter.CreateAgencyExchangeDocumentFromExchangeDocument(document))
                .ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<AgencyExchangeDocument>> GetDocumentsToDelete(ExchangeDocumentDirection exchangeDocumentDirection, int ukprn, string fileType, string year)
        {
            var documents = await _supportToolsApiClient.DeleteDocumentsSearch(exchangeDocumentDirection, ukprn, fileType, year);

            if (documents != null && documents.Items != null)
            {
                return documents.Items
               .Select(document => _documentConverter.CreateAgencyExchangeDocumentFromExchangeDocument(document))
               .ToList();
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<AgencyExchangeDocument>> DeleteDocuments(IEnumerable<string> documentReferences, IEnumerable<string> publishers = null)
        {
            publishers ??= new List<string>();
            var currentUserInfo = await _userInfoProvider.GetCurrentUserInfo();

            var documentReferencesToDelete = _documentReferenceService
                .CreateDocumentReferencesWithPreviousVersionsFromStrings(documentReferences);

            var deleteResult = await _exchangeApiClient.DeleteDocuments(
                new ExchangeDocumentDeleteRequest
                {
                    UserInfo = currentUserInfo,
                    DocumentReferences = documentReferencesToDelete
                });

            var result = deleteResult
                .Select(document => _documentConverter.CreateAgencyExchangeDocumentFromExchangeDocument(document))
                .Select(d =>
                {
                    d.IsDeleted = true;
                    return d;
                }).ToList();

            for (int i = 0; i < publishers.Count(); i++)
            {
                result[i].PublishedBy = publishers.ToList()[i];
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<AgencyExchangeDocument> DeleteDocumentVersion(string documentReference)
        {
            var currentUserInfo = await _userInfoProvider.GetCurrentUserInfo();

            var documentReferenceToDelete = _documentReferenceService
                .CreateDocumentReferenceFromString(documentReference);

            var deleteResult = await _exchangeApiClient.DeleteDocument(
                new ExchangeDocumentDeleteRequest
                {
                    UserInfo = currentUserInfo,
                    DocumentReferences = new List<DocumentReferenceWithPreviousVersions>
                    {
                        new DocumentReferenceWithPreviousVersions(documentReferenceToDelete)
                    }
                });

            return _documentConverter.CreateAgencyExchangeDocumentFromExchangeDocument(deleteResult);
        }

        private IEnumerable<IFilterOption> CreateFilterOptions(IEnumerable<IFilterCategory> filters)
        {
            var filterOptions = Collection.Empty<IFilterOption>();

            var teamFilterExists = TryGetRadioFilter(filters, FilterKey.Team.ToString(), out RadioFilterCategory teamFilter);
            if (teamFilterExists)
            {
                var teamFilterOption = new RadioFilterOption
                {
                    Type = FilterOptionType.RadioFilterOption.ToString(),
                    Key = teamFilter.Key,
                    Value = teamFilter.Value
                };

                filterOptions.Add(teamFilterOption);
            }

            return filterOptions;
        }

        private bool TryGetRadioFilter(IEnumerable<IFilterCategory> filters, string filterKey, out RadioFilterCategory filter)
        {
            filter = filters
                ?.OfType<RadioFilterCategory>()
                .FirstOrDefault(filter => string.Equals(filter.Key, filterKey, StringComparison.OrdinalIgnoreCase));

            return !It.IsNull(filter);
        }
    }
}