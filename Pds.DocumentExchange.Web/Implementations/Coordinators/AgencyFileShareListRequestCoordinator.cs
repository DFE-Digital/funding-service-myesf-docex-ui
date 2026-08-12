using Microsoft.Extensions.Options;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Interfaces.Coordinators;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models;
using Pds.DocumentExchange.Web.Models.Agency;
using Pds.DocumentExchange.Web.Models.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Implementations.Coordinators
{
    /// <inheritdoc cref="IAgencyFileShareListRequestCoordinator"/>
    public class AgencyFileShareListRequestCoordinator : IAgencyFileShareListRequestCoordinator
    {
        private readonly IUserInformationProvider _userInfoProvider;
        private readonly IAgencyApiClient _agencyApiClient;
        private readonly IListHelper _listHelper;
        private readonly DocumentExchangeConfiguration _configuration;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgencyFileShareListRequestCoordinator"/> class.
        /// </summary>
        /// <param name="userInfoProvider"><see cref="IUserInformationProvider"/>.</param>
        /// <param name="agencyApiClient"><see cref="IAgencyApiClient"/>.</param>
        /// <param name="listHelper"><see cref="IListHelper"/>.</param>
        /// <param name="configurationOptions">Options containing <see cref="DocumentExchangeConfiguration"/>.</param>
        /// <param name="mapper"><see cref="IMapper"/>.</param>
        public AgencyFileShareListRequestCoordinator(
            IUserInformationProvider userInfoProvider,
            IAgencyApiClient agencyApiClient,
            IListHelper listHelper,
            IOptions<DocumentExchangeConfiguration> configurationOptions,
            IMapper mapper)
        {
            _userInfoProvider = userInfoProvider;
            _agencyApiClient = agencyApiClient;
            _listHelper = listHelper;
            _mapper = mapper;
            _configuration = configurationOptions.Value;
        }

        /// <inheritdoc/>
        public async Task<DocumentsToPublish> GetDocumentsToPublish(ListRequest request)
            => await GetFileShareDocuments<DocumentsToPublish, FileShareDocument>(
                AgencyDocumentValidity.Valid,
                request,
                document => new FileShareDocument
                {
                    DocumentReference = GetFileShareDocumentReference(document),
                    ProductName = document.Product.Name
                },
                (listResult, listPage) =>
                {
                    var firstDocument = listResult?.Items?.FirstOrDefault();
                    listPage.SelectedProduct = _mapper.ToWebProduct(firstDocument?.Product);
                    listPage.SelectedTeam = firstDocument?.Team;
                });

        /// <inheritdoc/>
        public async Task<DocumentListUpdateData> GetDocumentsToPublishData(ListRequest request)
        {
            var documentsToPublish = await GetDocumentsToPublish(request);
            var documentListUpdateData = _listHelper.GetDocumentListUpdateData(documentsToPublish);

            BuildDocumentsToPublishPageUpdateItems(documentListUpdateData, documentsToPublish);

            return documentListUpdateData;
        }

        /// <inheritdoc/>
        public async Task<DocumentsToReview> GetDocumentsToReview(ListRequest request)
            => await GetFileShareDocuments<DocumentsToReview, InvalidFileShareDocument>(
                AgencyDocumentValidity.Invalid,
                request,
                document => new InvalidFileShareDocument
                {
                    DocumentReference = GetFileShareDocumentReference(document),
                    ProductName = document.Product?.Name,
                    FileNameError = document.FileNameError
                });

        /// <inheritdoc/>
        public async Task<DocumentListUpdateData> GetDocumentsToReviewData(ListRequest request)
            => _listHelper.GetDocumentListUpdateData(await GetDocumentsToReview(request));

        private static void BuildDocumentsToPublishPageUpdateItems(
            DocumentListUpdateData documentListUpdateData,
            DocumentsToPublish documentsToPublish)
        {
            var totalDocumentsForSelectedProduct = documentsToPublish.Pagination.TotalItems;

            if (totalDocumentsForSelectedProduct == 0)
            {
                return;
            }

            var selectedProductName = totalDocumentsForSelectedProduct == 1
                ? documentsToPublish.SelectedProduct.Name
                : documentsToPublish.SelectedProduct.PluralName;

            documentListUpdateData.PageUpdateItems = new[]
            {
                new PageUpdateItem(
                    "#total-documents-for-selected-product",
                    totalDocumentsForSelectedProduct.ToString()),
                new PageUpdateItem(
                    "#selected-product-name",
                    selectedProductName)
            };
        }

        private async Task<TListPage> GetFileShareDocuments<TListPage, TFileShareDocument>(
            AgencyDocumentValidity validity,
            ListRequest request,
            Func<AgencyDocument, TFileShareDocument> documentConverter,
            Action<ListResult<AgencyDocument>, TListPage> postAction = null)
                where TListPage : IListViewModel, IDocumentListPage, new()
                where TFileShareDocument : FileShareDocument
        {
            var teams = await _userInfoProvider.GetCurrentUserAgencyTeams();
            var teamDocuments = await _agencyApiClient.ListTeamDocuments(teams, new AgencyListDocumentOptions
            {
                Validity = validity,
                PageSize = _configuration.ListPageSize,
                PageNumber = request?.Page ?? 1,
                FilterOptions = _listHelper.GetFilterOptions(request)
            });

            var listPage = new TListPage
            {
                Pagination = _listHelper.GetPaginationViewModel(request, teamDocuments),
                ListItems = teamDocuments.Items.Select(documentConverter),
                FilterCategories = _listHelper.GetFilterCategories(teamDocuments),
                AnyDocumentsAvailable = _listHelper.AnyDocumentsAvailable(request, teamDocuments)
            };

            postAction?.Invoke(teamDocuments, listPage);

            return listPage;
        }

        private FileShareDocumentReference GetFileShareDocumentReference(AgencyDocument document)
            => new FileShareDocumentReference
            {
                FileName = document.FileName,
                Team = document.Team
            };
    }
}