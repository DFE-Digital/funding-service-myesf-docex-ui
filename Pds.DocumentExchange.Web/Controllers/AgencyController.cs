using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pds.Core.Utils;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.Areas.Admin.Controllers;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Agency;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Shared;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Helpers;
using Pds.DocumentExchange.Web.Interfaces.Coordinators;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models;
using Pds.DocumentExchange.Web.Models.Agency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Pds.DocumentExchange.Web.Helpers.AuthorizationHelper;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Controllers
{
    /// <summary>
    /// The controller containing actions that can be accessed by agency users.
    /// </summary>
    [Authorize(Policy = Policies.RequireAnyDocumentExchangeAgencyRole)]
    public class AgencyController : BaseDocumentExchangeController
    {
        private readonly IAgencyApiClient _agencyApiClient;
        private readonly IMimeMappingService _mimeMappingService;
        private readonly IAgencyDocumentErrorReportBuilder _errorReportBuilder;
        private readonly ISystemProvider _system;
        private readonly IExchangeDocumentDownloadService _exchangeDocumentDownloadService;
        private readonly IMapper _mapper;
        private readonly IAgencySummaryRequestCoordinator _summaryRequestCoordinator;
        private readonly IAgencyFileShareListRequestCoordinator _fileShareListRequestCoordinator;
        private readonly IAgencyExchangeListRequestCoordinator _exchangeListRequestCoordinator;
        private readonly IAgencyExchangeDeletionRequestCoordinator _deletionRequestCoordinator;
        private readonly IDocumentReferenceService _documentReferenceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgencyController"/> class.
        /// </summary>
        /// <param name="userInformationProvider"><see cref="IUserInformationProvider"/>.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="agencyApiClient">The agency API client.</param>
        /// <param name="mimeMappingService">The MIME mapping service.</param>
        /// <param name="errorReportBuilder">The error report builder.</param>
        /// <param name="system">The system provider.</param>
        /// <param name="exchangeDocumentDownloadService">The exchange document download service.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="agencySummaryCoordinator">The agency summary coordinator.</param>
        /// <param name="agencyFileShareListRequestCoordinator"><see cref="IAgencyFileShareListRequestCoordinator"/>.</param>
        /// <param name="agencyExchangeListRequestCoordinator"><see cref="IAgencyExchangeListRequestCoordinator"/>.</param>
        /// <param name="deletionRequestCoordinator"><see cref="IAgencyExchangeDeletionRequestCoordinator"/>.</param>
        /// <param name="documentReferenceService"><see cref="IDocumentReferenceService"/>.</param>
        public AgencyController(
            IUserInformationProvider userInformationProvider,
            IOptions<DocumentExchangeConfiguration> configurationOptions,
            IAgencyApiClient agencyApiClient,
            IMimeMappingService mimeMappingService,
            IAgencyDocumentErrorReportBuilder errorReportBuilder,
            ISystemProvider system,
            IExchangeDocumentDownloadService exchangeDocumentDownloadService,
            IMapper mapper,
            IAgencySummaryRequestCoordinator agencySummaryCoordinator,
            IAgencyFileShareListRequestCoordinator agencyFileShareListRequestCoordinator,
            IAgencyExchangeListRequestCoordinator agencyExchangeListRequestCoordinator,
            IAgencyExchangeDeletionRequestCoordinator deletionRequestCoordinator,
            IDocumentReferenceService documentReferenceService)
            : base(userInformationProvider, configurationOptions)
        {
            _agencyApiClient = agencyApiClient;
            _mimeMappingService = mimeMappingService;
            _errorReportBuilder = errorReportBuilder;
            _system = system;
            _exchangeDocumentDownloadService = exchangeDocumentDownloadService;
            _mapper = mapper;
            _summaryRequestCoordinator = agencySummaryCoordinator;
            _fileShareListRequestCoordinator = agencyFileShareListRequestCoordinator;
            _exchangeListRequestCoordinator = agencyExchangeListRequestCoordinator;
            _deletionRequestCoordinator = deletionRequestCoordinator;
            _documentReferenceService = documentReferenceService;
        }

        /// <summary>
        /// The landing page action for agency users.
        /// </summary>
        /// <returns>The landing page view for agency users.</returns>
        [LandingBreadCrumb(true)]
        public async Task<IActionResult> AgencyHome()
        {
            var pageData = await _summaryRequestCoordinator.GetHomePageData();

            pageData.ShowViewAsOrganisation &= Configuration.ShowServiceStartPage;

            var tiles = new List<DashboardTile>();

            static string GetAlertClass(int count) => count > 0
                                ? "alert alert-name-red"
                                : "alert alert-name-green";

            if (pageData.ShowDocumentOptions)
            {
                tiles.Add(new DashboardTile
                {
                    LinkId = "publish-documents",
                    LinkUrl = Url?.ActionLink(
                        nameof(AgencyController.FileShare)),
                    TitleId = "publishDocuments",
                    Title = "Publish documents from your file share",
                    AlertClass = GetAlertClass(pageData.TotalCountOfDocumentsInFileShare),
                    AlertText = ContentHelper.GetDocumentCountMessage(pageData.TotalCountOfDocumentsInFileShare, "{count} {document(s)} in your file share")
                });

                tiles.Add(new DashboardTile
                {
                    LinkId = "download-documents",
                    LinkUrl = Url?.ActionLink(
                        nameof(AgencyController.DownloadDocuments)),
                    TitleId = "downloadDocuments",
                    Title = "Download your documents",
                    AlertClass = GetAlertClass(pageData.CountOfNewDocuments),
                    AlertText = ContentHelper.GetDocumentCountMessage(pageData.CountOfNewDocuments, "{count} new {document(s)} uploaded by providers")
                });
            }

            if (pageData.ShowSettingsOption)
            {
                tiles.Add(new DashboardTile
                {
                    LinkId = "docex-settings",
                    LinkUrl = Url?.ActionLink(
                        nameof(SettingsController.Index),
                        NameOf<SettingsController>()),
                    TitleId = "DocExSettings",
                    Title = "Document exchange settings",
                    AlertClass = GetAlertClass(0),
                    AlertText = "View or change the settings for Document exchange"
                });
            }

            if (pageData.ShowViewAsOrganisation)
            {
                tiles.Add(new DashboardTile
                {
                    LinkId = "view-as-organisation",
                    LinkUrl = Url?.ActionLink(
                        nameof(Areas.ViewAsOrganisation.Controllers.OrganisationSearchController.SearchForAnOrganisation),
                        NameOf<Areas.ViewAsOrganisation.Controllers.OrganisationSearchController>(),
                        new { Area = Areas.ViewAsOrganisation.Constants.AreaName }),
                    TitleId = "viewAsOrganisation",
                    Title = "View as an organisation",
                    AlertClass = GetAlertClass(0),
                    AlertText = "Search for an organisation to view their documents"
                });
            }


            if (pageData.ShowToolsOption)
            {
                tiles.Add(new DashboardTile
                {
                    LinkId = "support-tools",
                    LinkUrl = Url?.ActionLink(
                        nameof(SupportToolsController.SupportTools), NameOf<SupportToolsController>()),
                    TitleId = "supportTools",
                    Title = "Document exchange support tools",
                    AlertClass = GetAlertClass(0),
                    AlertText = "Access support tools"
                });
            }

            return View(new AgencyHome { Tiles = tiles });
        }

        /// <summary>
        /// The action for the file share page.
        /// </summary>
        /// <returns>The file share page view.</returns>
        [FileShareBreadCrumb(true)]
        public async Task<IActionResult> FileShare()
        {
            var viewModel = await _summaryRequestCoordinator.GetFileSharePage();

            return View(viewModel);
        }

        #region Review

        /// <summary>
        /// The action for the page listing invalid documents to review.
        /// </summary>
        /// <param name="request">Object containing the request parameters.</param>
        /// <returns>The review page view.</returns>
        [DocumentsToReviewBreadCrumb(true)]
        public async Task<IActionResult> DocumentsToReview(DocumentsRequest request)
        {
            var documentsToReview = await _fileShareListRequestCoordinator.GetDocumentsToReview(request);

            documentsToReview.Error = request?.Error ?? false;
            documentsToReview.ErrorAction = request?.ErrorAction;

            return View(documentsToReview);
        }

        /// <summary>
        /// The documents to review data action.
        /// </summary>
        /// <param name="request">Object containing the request parameters.</param>
        /// <returns>The review page data.</returns>
        public async Task<IActionResult> DocumentsToReviewData(ListRequest request)
        {
            var data = await _fileShareListRequestCoordinator.GetDocumentsToReviewData(request);

            return Json(data);
        }

        /// <summary>
        /// Action for the report of invalid documents to review.
        /// </summary>
        /// <param name="documentReferenceList">The list of references to the documents that should be included in the report.</param>
        /// <returns>The documents to review report file result.</returns>
        [HttpPost]
        public async Task<IActionResult> DocumentsToReviewReport(FileShareDocumentReferenceList documentReferenceList)
        {
            var documentReferences = documentReferenceList?.FileShareDocumentReferences?.ToList();

            if (documentReferences?.Any() != true)
            {
                return RedirectToAction(
                    nameof(DocumentsToReview),
                    NameOf<AgencyController>(),
                    new
                    {
                        error = true,
                        errorAction = nameof(DocumentsToReviewReport)
                    });
            }

            var fileNameGroups = GetTeamGroupsFromFileShareDocumentReferences(documentReferences);
            var fileNameGroup = fileNameGroups.Single();

            var teams = await UserInformationProvider.GetCurrentUserAgencyTeams();

            if (!teams.Contains(fileNameGroup.Key, StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized();
            }

            var report = await _errorReportBuilder.BuildErrorReportSpreadsheet(
                fileNameGroup.Key,
                new AgencyListDocumentOptions
                {
                    Validity = AgencyDocumentValidity.Invalid,
                    PageSize = int.MaxValue,
                    DocumentNames = fileNameGroup.Value
                });

            var fileName = $"DocumentExchange_DocumentErrors_{_system.DateTime.UtcNow():yyyy-MM-dd}.ods";
            var contentType = _mimeMappingService.GetContentType(fileName);

            return File(report, contentType, fileName);
        }

        #endregion


        #region Publish

        /// <summary>
        /// The documents to publish page action.
        /// </summary>
        /// <param name="request">Object containing the request parameters.</param>
        /// <returns>The publish page view.</returns>
        [DocumentsToPublishBreadCrumb(true)]
        public async Task<IActionResult> DocumentsToPublish(DocumentsRequest request)
        {
            var documentsToPublish = await _fileShareListRequestCoordinator.GetDocumentsToPublish(request);

            return View(documentsToPublish);
        }

        /// <summary>
        /// The documents to publish data action.
        /// </summary>
        /// <param name="request">Object containing the request parameters.</param>
        /// <returns>The publish page data.</returns>
        public async Task<IActionResult> DocumentsToPublishData(ListRequest request)
        {
            var data = await _fileShareListRequestCoordinator.GetDocumentsToPublishData(request);

            return Json(data);
        }

        /// <summary>
        /// Action for the "are you sure you want to publish these documents?" page.
        /// </summary>
        /// <param name="request">Object containing the request parameters.</param>
        /// <returns>The "are you sure you want to publish these documents?" page view.</returns>
        [PublishDocumentsAreYouSureBreadCrumb(true)]
        public async Task<IActionResult> PublishDocumentsAreYouSure(DocumentsRequest request)
        {
            var documentsToPublish = await _fileShareListRequestCoordinator.GetDocumentsToPublish(request);

            if (documentsToPublish.Pagination.TotalItems == 0)
            {
                return RedirectToAction(
                    nameof(DocumentsToPublish),
                    NameOf<AgencyController>());
            }

            var viewModel = new PublishDocumentsAreYouSure
            {
                Count = documentsToPublish.Pagination.TotalItems,
                Product = documentsToPublish.SelectedProduct,
                SelectedProductId = documentsToPublish.SelectedProduct.Identifier,
                SelectedTeam = documentsToPublish.SelectedTeam
            };

            return View(viewModel);
        }

        /// <summary>
        /// Action for the "you've published N document(s)" page.
        /// </summary>
        /// <param name="actionData">An object containing the data required to build this page.</param>
        /// <returns>The "you've published N document(s)" page view.</returns>
        public async Task<IActionResult> PublishDocumentsConfirmation(PublishDocumentsConfirmation actionData)
        {
            if (actionData?.PublishConfirmed == null)
            {
                return RedirectToActionPreserveMethod(
                    nameof(PublishDocumentsAreYouSure),
                    NameOf<AgencyController>(),
                    new
                    {
                        error = true
                    });
            }

            if (actionData.PublishConfirmed == false ||
                string.IsNullOrEmpty(actionData.SelectedTeam) ||
                actionData.SelectedProductId == default)
            {
                return RedirectToAction(nameof(DocumentsToPublish));
            }

            var teams = await UserInformationProvider.GetCurrentUserAgencyTeams();

            if (!teams.Contains(actionData.SelectedTeam, StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized();
            }

            var (product, count) = await _agencyApiClient.PublishTeamDocuments(
                actionData.SelectedTeam,
                new AgencyPublishRequest
                {
                    ProductId = actionData.SelectedProductId,
                    UserInfo = await UserInformationProvider.GetCurrentUserInfo()
                });

            actionData.Product = _mapper.ToWebProduct(product);
            actionData.Count = count;

            return View(actionData);
        }

        #endregion


        #region Download

        /// <summary>
        /// The manage documents page action.
        /// </summary>
        /// <param name="request">Object containing the request parameters.</param>
        /// <returns>The manage documents page view.</returns>
        [DownloadDocumentsBreadCrumb(true)]
        public async Task<IActionResult> DownloadDocuments(DocumentsRequest request)
        {
            var downloadDocuments = await _exchangeListRequestCoordinator.GetDocumentsToDownload(request);

            downloadDocuments.Error = request?.Error ?? false;
            downloadDocuments.ErrorAction = request?.ErrorAction;

            return View(downloadDocuments);
        }

        /// <summary>
        /// The documents to manage data action.
        /// </summary>
        /// <param name="request">Object containing the request parameters.</param>
        /// <returns>The manage documents page data.</returns>
        public async Task<IActionResult> DocumentsToDownloadData(ListRequest request)
        {
            var data = await _exchangeListRequestCoordinator.GetDocumentsToDownloadData(request);

            return Json(data);
        }

        #endregion


        /// <summary>
        /// Action for downloading a document from the file share.
        /// </summary>
        /// <param name="team">The team identifier.</param>
        /// <param name="fileName">The file name of the document to download.</param>
        /// <returns>A <see cref="FileContentResult"/> containing the requested file.</returns>
        public async Task<IActionResult> DownloadDocument(string team, string fileName)
        {
            var teams = await UserInformationProvider.GetCurrentUserAgencyTeams();

            if (!teams.Contains(team, StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized();
            }

            var documentContent = await _agencyApiClient.DownloadTeamDocument(team, fileName);
            var contentType = _mimeMappingService.GetContentType(fileName);

            return File(documentContent, contentType, fileName);
        }

        /// <summary>
        /// Action for downloading an exchange document.
        /// </summary>
        /// <param name="documentReferenceString">The document reference information.</param>
        /// <returns>A <see cref="FileContentResult"/> containing the requested file.</returns>
        public async Task<IActionResult> DownloadExchangeDocument(string documentReferenceString)
        {
            var userInfo = await UserInformationProvider.GetCurrentUserInfo();

            var file = await _exchangeDocumentDownloadService.DownloadExchangeDocumentByReference(
                documentReferenceString,
                userInfo);

            return File(file.Content, file.ContentType, file.Name);
        }

        /// <summary>
        /// Action for downloading a list of exchange documents.
        /// </summary>
        /// <param name="documentReferenceList">The list of document references to download.</param>
        /// <returns>A <see cref="FileContentResult"/> containing the requested file(s).</returns>
        public async Task<IActionResult> DownloadExchangeDocumentList(DocumentReferenceList documentReferenceList)
        {
            if (documentReferenceList?.DocumentReferences?.Any() != true)
            {
                return RedirectToAction(
                    nameof(DownloadDocuments),
                    NameOf<AgencyController>(),
                    new
                    {
                        error = true,
                        errorAction = nameof(DownloadExchangeDocumentList)
                    });
            }

            var userInfo = await UserInformationProvider.GetCurrentUserInfo();

            var documentReferences = await GetAllDocumentReferencesIncludingPreviousVersions(documentReferenceList.DocumentReferences);

            var file = await _exchangeDocumentDownloadService.DownloadExchangeDocumentsByReferences(
                documentReferences,
                userInfo);

            return File(file.Content, file.ContentType, file.Name);
        }

        /// <summary>
        /// Action for downloading a list of exchange documents.
        /// </summary>
        /// <param name="request">An object containing the request parameters.</param>
        /// <returns>A <see cref="FileContentResult"/> containing the requested file(s).</returns>
        public async Task<IActionResult> DownloadExchangeDocumentsByFilters(ListRequest request)
        {
            var file = await _exchangeListRequestCoordinator.DownloadDocuments(request);

            return File(file.Content, file.ContentType, file.Name);
        }

        /// <summary>
        /// Action for the "are you sure you want to remove these documents?" page.
        /// </summary>
        /// <param name="actionData">An object containing the data required to build this page.</param>
        /// <returns>The "are you sure you want to remove these documents?" page view.</returns>
        [HttpPost]
        [RemoveDocumentsAreYouSureBreadCrumb(true)]
        public async Task<IActionResult> RemoveDocumentsAreYouSure(RemoveDocumentsAreYouSure actionData)
        {
            var documentReferences = actionData?.FileShareDocumentReferences?.ToList();

            if (documentReferences?.Any() != true)
            {
                return RedirectToAction(
                    actionData?.EntryAction,
                    NameOf<AgencyController>(),
                    new
                    {
                        error = true,
                        errorAction = nameof(RemoveDocumentsAreYouSure)
                    });
            }

            var fileNameGroups = GetTeamGroupsFromFileShareDocumentReferences(documentReferences);
            var fileNameGroup = fileNameGroups.Single();

            var teams = await UserInformationProvider.GetCurrentUserAgencyTeams();

            if (!teams.Contains(fileNameGroup.Key, StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized();
            }

            actionData.Team = fileNameGroup.Key;
            actionData.FileNames = fileNameGroup.Value;

            return View(actionData);
        }

        /// <summary>
        /// Action for the "you've removed N document(s)" page.
        /// </summary>
        /// <param name="actionData">An object containing the data required to build this page.</param>
        /// <returns>The "you've removed N document(s)" page view.</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveDocumentsConfirmation(RemoveDocumentsConfirmation actionData)
        {
            if (actionData?.RemovalConfirmed == null)
            {
                return RedirectToActionPreserveMethod(
                    nameof(RemoveDocumentsAreYouSure),
                    NameOf<AgencyController>(),
                    new
                    {
                        error = true
                    });
            }

            var documentReferences = actionData.FileShareDocumentReferences?.ToList();

            if (actionData.RemovalConfirmed == false ||
                documentReferences?.Any() != true)
            {
                return RedirectToAction(actionData.EntryAction);
            }

            var fileNameGroups = GetTeamGroupsFromFileShareDocumentReferences(documentReferences);
            var fileNameGroup = fileNameGroups.Single();

            var teams = await UserInformationProvider.GetCurrentUserAgencyTeams();

            if (!teams.Contains(fileNameGroup.Key, StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized();
            }

            await _agencyApiClient.RemoveTeamDocuments(
                fileNameGroup.Key,
                fileNameGroup.Value);

            actionData.Team = fileNameGroup.Key;
            actionData.FileNames = fileNameGroup.Value;

            return View(actionData);
        }

        /// <summary>
        /// Action for the "are you sure you want to delete these documents?" page.
        /// </summary>
        /// <param name="actionData">An object containing the data required to build this page.</param>
        /// <param name="request">Object containing the request parameters.</param>
        /// <returns>The "are you sure you want to delete these documents?" page view.</returns>
        [HttpPost]
        [Authorize(Policy = Policies.RequireDocumentExchangeAgencyAdvancedUserRole)]
        [DeleteDocumentsAreYouSureBreadCrumb(true)]
        public async Task<IActionResult> DeleteDocumentsAreYouSure(
            DeleteDocumentsAreYouSure actionData,
            ListRequest request)
        {
            if (actionData?.DocumentReferences?.Any() != true)
            {
                return RedirectToAction(
                    nameof(DownloadDocuments),
                    NameOf<AgencyController>(),
                    new
                    {
                        error = true,
                        errorAction = nameof(DeleteDocumentsAreYouSure)
                    });
            }

            var documentsToDelete =
                await _deletionRequestCoordinator.GetDocumentsToDelete(
                    actionData.DocumentReferences,
                    request.FilterRequest.Filters);

            actionData.ListItems = documentsToDelete;

            return View(actionData);
        }

        /// <summary>
        /// Action for the "you've deleted N document(s)" page.
        /// </summary>
        /// <param name="actionData">An object containing the data required to build this page.</param>
        /// <returns>The "you've deleted N document(s)" page view.</returns>
        [HttpPost]
        [Authorize(Policy = Policies.RequireDocumentExchangeAgencyAdvancedUserRole)]
        public async Task<IActionResult> DeleteDocumentsConfirmation(DeleteDocumentsConfirmation actionData)
        {
            actionData.Publishers ??= new List<string>();

            if (actionData?.DocumentReferences?.Any() != true)
            {
                return RedirectToAction(
                    nameof(DownloadDocuments),
                    NameOf<AgencyController>());
            }

            var deletedDocuments =
                await _deletionRequestCoordinator.DeleteDocuments(
                    actionData.DocumentReferences,
                    actionData.Publishers);

            actionData.ListItems = deletedDocuments;

            return View(actionData);
        }

        private static IDictionary<string, IEnumerable<string>> GetTeamGroupsFromFileShareDocumentReferences(
            List<string> documentReferences)
        {
            var result = new Dictionary<string, IEnumerable<string>>();

            foreach (var r in documentReferences)
            {
                if (!FileShareDocumentReference.TryParse(r, out var documentReference))
                {
                    throw new ArgumentException($"Could not parse document reference: {r}", nameof(documentReferences));
                }

                if (result.ContainsKey(documentReference.Team))
                {
                    result[documentReference.Team] = result[documentReference.Team].Append(documentReference.FileName);
                }
                else
                {
                    result.Add(documentReference.Team, new List<string> { documentReference.FileName });
                }
            }

            return result;
        }

        private async Task<IEnumerable<string>> GetAllDocumentReferencesIncludingPreviousVersions(IEnumerable<string> documentReferences)
        {
            var documentReferencesStringList = new List<string>();

            documentReferencesStringList.AddRange(documentReferences);

            var documentReferencesList = documentReferences
                .Select(item => _documentReferenceService.CreateDocumentReferenceFromString(item)).ToList();

            var result = await _agencyApiClient.GetPreviousDocumentVersionReferences(
                await UserInformationProvider.GetCurrentUserAgencyTeams(),
                documentReferencesList);

            documentReferencesStringList
                .AddRange((result ?? Enumerable.Empty<DocumentReference>())
                .Select(item => $"{item.FileName}|{item.BatchIdentifier}|{item.ParentBatchIdentifier}"));

            return documentReferencesStringList;
        }
    }
}