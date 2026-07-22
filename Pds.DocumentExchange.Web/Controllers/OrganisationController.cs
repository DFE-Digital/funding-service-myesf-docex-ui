using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pds.Core.Common.Organisation.Enums;
using Pds.Core.Common.Organisation.Models;
using Pds.Core.Logging;
using Pds.Core.Utils.Helpers;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Organisation;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Shared;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Helpers;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models;
using Pds.DocumentExchange.Web.Models.Organisation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Pds.DocumentExchange.Web.Helpers.AuthorizationHelper;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Controllers
{
    /// <summary>
    /// The controller containing actions that can be accessed by organisation users.
    /// </summary>
    [Authorize(Policy = Policies.RequireDocumentExchangeOrganisationDocumentViewerRole)]
    public class OrganisationController : BaseDocumentExchangeController
    {
        private const int FileSizeCoefficient = 1000;

        private readonly DocumentExchangeConfiguration _configuration;
        private readonly IExchangeApiClient _exchangeApiClient;
        private readonly IUploadApiClient _uploadApiClient;
        private readonly ISettingsApiClient _settingsApiClient;
        private readonly IMimeMappingService _mimeMappingService;
        private readonly IExchangeDocumentDownloadService _exchangeDocumentDownloadService;
        private readonly IDocumentStatusProvider _documentStatusProvider;
        private readonly ILoggerAdapter<OrganisationController> _logger;
        private readonly IDateTimeDisplayHelper _dateTimeDisplayHelper;
        private readonly IListHelper _listHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganisationController"/> class.
        /// </summary>
        /// <param name="userInformationProvider"><see cref="IUserInformationProvider"/>.</param>
        /// <param name="mimeMappingService">The MIME mapping service.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="documentExchangeApi">The "exchange" API client.</param>
        /// <param name="uploadApiClient">The upload API client.</param>
        /// <param name="settingsApiClient">The settings API client.</param>
        /// <param name="exchangeDocumentDownloadService">The exchange document download service.</param>
        /// <param name="documentStatusProvider">The document status provider.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="dateTimeDisplayHelper">The DateTime display helper.</param>
        /// <param name="listHelper"><see cref="IListHelper"/>.</param>
        public OrganisationController(
            IUserInformationProvider userInformationProvider,
            IMimeMappingService mimeMappingService,
            IOptions<DocumentExchangeConfiguration> configurationOptions,
            IExchangeApiClient documentExchangeApi,
            IUploadApiClient uploadApiClient,
            ISettingsApiClient settingsApiClient,
            IExchangeDocumentDownloadService exchangeDocumentDownloadService,
            IDocumentStatusProvider documentStatusProvider,
            ILoggerAdapter<OrganisationController> logger,
            IDateTimeDisplayHelper dateTimeDisplayHelper,
            IListHelper listHelper)
            : base(userInformationProvider, configurationOptions)
        {
            _mimeMappingService = mimeMappingService;
            _configuration = configurationOptions.Value;
            _exchangeApiClient = documentExchangeApi;
            _uploadApiClient = uploadApiClient;
            _settingsApiClient = settingsApiClient;
            _exchangeDocumentDownloadService = exchangeDocumentDownloadService;
            _documentStatusProvider = documentStatusProvider;
            _logger = logger;
            _dateTimeDisplayHelper = dateTimeDisplayHelper;
            _listHelper = listHelper;
        }

        /// <summary>
        /// The landing page action for organisation users.
        /// </summary>
        /// <returns>The landing page view for organisation users.</returns>
        [LandingBreadCrumb(true)]
        public async Task<IActionResult> Home()
        {
            var currentUserInfo = await UserInformationProvider.GetCurrentUserInfo();
            var summary = await _exchangeApiClient.GetOrganisationUserSummary(currentUserInfo);
            var showParentView = await UserInformationProvider.CurrentUserShouldSeeParentView();

            var viewModel = new Home
            {
                CountOfNewDocuments = summary.CountOfNewDocuments ?? 0,
                SendNewDocumentActionName = GetSendNewDocumentActionName(showParentView)
            };

            return View(viewModel);
        }

        /// <summary>
        /// The received documents page for organisation users.
        /// </summary>
        /// <param name="request">Object containing the request parameters.</param>
        /// <returns>The received documents page view.</returns>
        [ReceivedDocumentsBreadCrumb(true)]
        public async Task<IActionResult> ReceivedDocuments(DocumentsRequest request)
        {
            var viewModel = await GetReceivedDocuments(request);

            viewModel.Error = request?.Error ?? false;
            viewModel.ErrorAction = request?.ErrorAction;

            return View(viewModel);
        }

        /// <summary>
        /// The received documents data action.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The received documents data.</returns>
        public async Task<IActionResult> ReceivedDocumentsData(ListRequest request)
        {
            var viewModel = await GetReceivedDocuments(request);
            return Json(_listHelper.GetDocumentListUpdateData(viewModel));
        }

        /// <summary>
        /// Action for the report of invalid documents to review.
        /// </summary>
        /// <param name="documentReferenceList">The list of documents to be downloaded.</param>
        /// <returns>The documents to review report file result.</returns>
        [HttpPost]
        public async Task<IActionResult> ReceivedDocumentsDownload(DocumentReferenceList documentReferenceList)
        {
            if (documentReferenceList?.DocumentReferences?.Any() != true)
            {
                return RedirectToAction(
                    nameof(ReceivedDocuments),
                    NameOf<OrganisationController>(),
                    new
                    {
                        error = true,
                        errorAction = nameof(ReceivedDocumentsDownload)
                    });
            }

            var userInfo = await UserInformationProvider.GetCurrentUserInfo();

            var file = await _exchangeDocumentDownloadService.DownloadExchangeDocumentsByReferences(
                documentReferenceList.DocumentReferences,
                userInfo);

            return File(file.Content, file.ContentType, file.Name);
        }

        /// <summary>
        /// The sent documents action.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The sent documents page view.</returns>
        [SentDocumentsBreadCrumb(true)]
        public async Task<IActionResult> SentDocuments(ListRequest request)
        {
            var viewModel = await GetSentDocuments(request);
            return View(viewModel);
        }

        /// <summary>
        /// The sent documents data action.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The sent documents data.</returns>
        public async Task<IActionResult> SentDocumentsData(ListRequest request)
        {
            var viewModel = await GetSentDocuments(request);
            return Json(_listHelper.GetDocumentListUpdateData(viewModel));
        }

        /// <summary>
        /// The 'select an organisation' action.
        /// </summary>
        /// <param name="error">Whether there is an error that causes a redirect.</param>
        /// <returns>The 'select an organisation' page view.</returns>
        [Authorize(Policy = Policies.RequireDocumentExchangeOrganisationDocumentUploaderRole)]
        [SelectAnOrganisationBreadCrumb(true)]
        public async Task<IActionResult> SelectOrganisation(bool error)
        {
            var showParentView = await UserInformationProvider.CurrentUserShouldSeeParentView();

            if (!showParentView)
            {
                return RedirectToAction(nameof(SelectDocumentType));
            }

            return View(new SelectOrganisation { Error = error });
        }

        /// <summary>
        /// The 'select an academy' action.
        /// </summary>
        /// <param name="selectAcademyModel">The 'select an academy' page model.</param>
        /// <returns>The 'select an academy' page view.</returns>
        [Authorize(Policy = Policies.RequireDocumentExchangeOrganisationDocumentUploaderRole)]
        [SelectAnOrganisationBreadCrumb(true)]
        [HttpGet, HttpPost]
        public async Task<IActionResult> SelectAcademy(SelectAcademy selectAcademyModel)
        {
            if (selectAcademyModel != null && selectAcademyModel.SelectChildAcademy == null)
            {
                return RedirectToActionPreserveMethod(
                    nameof(SelectOrganisation),
                    NameOf<OrganisationController>(),
                    new
                    {
                        error = true
                    });
            }

            selectAcademyModel ??= new SelectAcademy();

            if (selectAcademyModel.SelectChildAcademy == false)
            {
                return RedirectToAction(nameof(SelectDocumentType));
            }

            var childOrganisations = await UserInformationProvider.GetCurrentUserChildOrganisations();

            if (childOrganisations?.Any() != true)
            {
                return RedirectToAction(nameof(SelectDocumentType));
            }

            if (selectAcademyModel.NewSearch)
            {
                selectAcademyModel.PageNumber = null;
            }
            else if (selectAcademyModel.ClearSearch)
            {
                selectAcademyModel.SearchTerm = string.Empty;
                selectAcademyModel.PageNumber = null;
            }

            selectAcademyModel.ChildAcademyPages = childOrganisations
                .Select(o =>
                    new ChildOrganisationInfo
                    {
                        Name = o.Name,
                        Ukprn = o.Identifiers.FirstOrDefault(
                            i => i.Type == OrganisationIdentifierType.Ukprn)?.Value
                    })
                .Where(o =>
                    string.IsNullOrWhiteSpace(selectAcademyModel.SearchTerm) ||
                    o.ToString().Contains(selectAcademyModel.SearchTerm.Trim(), StringComparison.OrdinalIgnoreCase))
                .Paginate(_configuration.ListPageSize);

            return View(selectAcademyModel);
        }

        /// <summary>
        /// The 'select a document type' action.
        /// </summary>
        /// <param name="selectDocumentTypeModel">The 'select a document type' page model.</param>
        /// <returns>The 'select a document type' page view.</returns>
        [Authorize(Policy = Policies.RequireDocumentExchangeOrganisationDocumentUploaderRole)]
        [SendYourDocumentBreadCrumb(true)]
        [HttpGet, HttpPost]
        public async Task<IActionResult> SelectDocumentType(SelectDocumentType selectDocumentTypeModel)
        {
            if (selectDocumentTypeModel?.SelectAnOrganisationRequired == true &&
                selectDocumentTypeModel?.UploadingForUkprn == null)
            {
                return RedirectToActionPreserveMethod(
                    nameof(SelectAcademy),
                    NameOf<OrganisationController>(),
                    new
                    {
                        error = true,
                        selectChildAcademy = true
                    });
            }

            var allowedProducts = await _settingsApiClient.GetProductsThatOrganisationsCanUpload();

            selectDocumentTypeModel ??= new SelectDocumentType();

            selectDocumentTypeModel.AllowedProducts = allowedProducts.Select(p => new Models.Shared.Product
            {
                Identifier = p.Identifier,
                Name = p.Name
            });

            selectDocumentTypeModel.ShowParentView = await UserInformationProvider.CurrentUserShouldSeeParentView();

            return View(selectDocumentTypeModel);
        }

        /// <summary>
        /// The 'select a document' action.
        /// </summary>
        /// <param name="selectDocumentModel">The 'select a document' page model.</param>
        /// <returns>The 'select a document' page view.</returns>
        [Authorize(Policy = Policies.RequireDocumentExchangeOrganisationDocumentUploaderRole)]
        [SendYourDocumentBreadCrumb(true)]
        [HttpPost]
        public async Task<IActionResult> SelectDocument(SelectDocument selectDocumentModel)
        {
            if (selectDocumentModel?.ProductIdentifier == 0)
            {
                return RedirectToActionPreserveMethod(
                    nameof(SelectDocumentType),
                    NameOf<OrganisationController>(),
                    new
                    {
                        error = true
                    });
            }

            var allowedProducts = await _settingsApiClient.GetProductsThatOrganisationsCanUpload();
            var maxFileUploadSize = await _settingsApiClient.GetMaxFileUploadSize();

            var product = allowedProducts.FirstOrDefault(p => p.Identifier == selectDocumentModel?.ProductIdentifier);

            if (product == null)
            {
                return RedirectToActionPreserveMethod(nameof(SelectDocumentType));
            }

            var nextVersionNumber = await GetNextVersionNumber(selectDocumentModel.UploadingForUkprn, selectDocumentModel.ProductIdentifier.ToString());

            selectDocumentModel.ProductName = product.Name;
            selectDocumentModel.NextVersionNumber = nextVersionNumber;
            selectDocumentModel.AllowedFileExtensions = await GetAllowedFileExtensions();
            selectDocumentModel.MaxFileUploadSize = maxFileUploadSize / FileSizeCoefficient;

            selectDocumentModel.ShowParentView = await UserInformationProvider.CurrentUserShouldSeeParentView();

            return View(selectDocumentModel);
        }

        /// <summary>
        /// Upload a document.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <returns>The relevant redirect result dependent on whether or not the upload succeeded.</returns>
        [Authorize(Policy = Policies.RequireDocumentExchangeOrganisationDocumentUploaderRole)]
        [HttpPost]
        public async Task<ActionResult> UploadDocument(Models.Organisation.UploadDocumentRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Upload document request requires a value.");
            }

            var formFile = request.FileInput;
            var fileName = formFile?.FileName;
            var documentValidationErrors = new List<string>();

            if (formFile == null || formFile.Length == 0)
            {
                documentValidationErrors.Add("Uploaded file is empty");
            }

            var fileSize = formFile?.Length ?? 0;
            var fileExtension = Path.GetExtension(formFile?.FileName ?? string.Empty).Trim('.');

            var maxFileUploadSize = await _settingsApiClient.GetMaxFileUploadSize();
            var allowedFileTypes = await _settingsApiClient.GetFileExtensions();

            var selectedFileType = allowedFileTypes.FirstOrDefault(f => f.Extension == fileExtension);

            if (selectedFileType == null || !CanUserUploadFileType(selectedFileType))
            {
                documentValidationErrors.Add("Select a document with a valid file format");
            }

            if (fileSize > maxFileUploadSize * FileSizeCoefficient)
            {
                documentValidationErrors.Add("Select a document within the file size limit");
            }

            if (documentValidationErrors.Any())
            {
                SelectDocument selectDocumentModel = new SelectDocument
                {
                    DocumentValidationErrors = documentValidationErrors,
                    ProductIdentifier = request.ProductIdentifier,
                    AllowedFileExtensions = allowedFileTypes.Select(f => f.Extension.ToUpperInvariant()).ToList(),
                    MaxFileUploadSize = maxFileUploadSize / FileSizeCoefficient,
                    UploadingForUkprn = request.UploadingForUkprn,
                    NextVersionNumber = await GetNextVersionNumber(request.UploadingForUkprn, request.ProductIdentifier.ToString()),
                    ShowParentView = await UserInformationProvider.CurrentUserShouldSeeParentView()
                };

                return View(nameof(SelectDocument), selectDocumentModel);
            }

            var currentUser = await UserInformationProvider.GetCurrentUserInfo();

            if (currentUser?.OrganisationInfo?.OrganisationIdentifier?.Value == null)
            {
                throw new InvalidOperationException("Unable to retrieve current user's organisation.");
            }

            OrganisationIdentifier fromOrganisationIdentifier = null;
            string fromOrganisationName = null;

            if (!string.IsNullOrEmpty(request.UploadingForUkprn))
            {
                var currentUserChildOrganisations = await UserInformationProvider.GetCurrentUserChildOrganisations();
                currentUserChildOrganisations ??= Enumerable.Empty<Organisation>();

                foreach (var childOrganisation in currentUserChildOrganisations)
                {
                    var matchingIdentifier = childOrganisation.Identifiers.FirstOrDefault(i =>
                        i.Type == OrganisationIdentifierType.Ukprn &&
                        i.Value == request.UploadingForUkprn);

                    if (matchingIdentifier != null)
                    {
                        fromOrganisationIdentifier = matchingIdentifier;
                        fromOrganisationName = childOrganisation.Name;
                        break;
                    }
                }

                if (fromOrganisationIdentifier == null)
                {
                    throw new InvalidOperationException($"Cannot upload document for ukprn: {request.UploadingForUkprn}.");
                }
            }
            else
            {
                fromOrganisationIdentifier = currentUser.OrganisationInfo.OrganisationIdentifier;
                fromOrganisationName = currentUser.OrganisationInfo.Name;
            }

            byte[] fileBytes;

            _logger.LogInformation($"Document upload started for organisation's file {fileName}.");

            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            var serviceUploadDocumentRequest = new Services.Models.UploadDocumentRequest
            {
                FileName = fileName,
                Bytes = fileBytes,
                ProductIdentifier = request.ProductIdentifier,
                UserInfo = currentUser,
                FromOrganisation = fromOrganisationIdentifier
            };

            try
            {
                await _uploadApiClient.UploadDocument(serviceUploadDocumentRequest);

                _logger.LogInformation($"Document upload finished for organisation's file {fileName}.");

                return RedirectToAction(
                    nameof(DocumentUploadComplete),
                    new
                    {
                        DocumentFileName = fileName,
                        OrganisationName = fromOrganisationName,
                        UploadingForUkprn = request.UploadingForUkprn
                    });
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"An error occurred when uploading the organisation's file {fileName}.");

                return RedirectToAction(nameof(DocumentUploadError));
            }
        }

        /// <summary>
        /// The 'document upload complete' action.
        /// </summary>
        /// <param name="documentUploadCompleteModel">The 'document upload complete' page model.</param>
        /// <returns>The 'document upload complete' page view.</returns>
        [HttpGet]
        public async Task<IActionResult> DocumentUploadComplete(DocumentUploadComplete documentUploadCompleteModel)
        {
            var user = await UserInformationProvider.GetCurrentUserInfo();
            var childOrganisations = await UserInformationProvider.GetCurrentUserChildOrganisations();
            var showParentView = await UserInformationProvider.CurrentUserShouldSeeParentView();

            documentUploadCompleteModel.SelectAnOrganisationRequired = showParentView;
            documentUploadCompleteModel.SendNewDocumentActionName = GetSendNewDocumentActionName(showParentView);

            if (!showParentView)
            {
                return View(documentUploadCompleteModel);
            }

            if (string.IsNullOrEmpty(documentUploadCompleteModel.UploadingForUkprn))
            {
                documentUploadCompleteModel.OrganisationName = user.OrganisationInfo.Name;
            }
            else
            {
                var organistion = childOrganisations.First(c => c.Identifiers.Any(i =>
                    i.Type == OrganisationIdentifierType.Ukprn &&
                    i.Value == documentUploadCompleteModel.UploadingForUkprn));
                documentUploadCompleteModel.OrganisationName = organistion.Name;
            }

            return View(documentUploadCompleteModel);
        }

        /// <summary>
        /// The 'document upload error' action.
        /// </summary>
        /// <returns>The 'document upload error' page view.</returns>
        public IActionResult DocumentUploadError()
        {
            // TODO in a future story - implementation of this action is not in scope for the current story.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Action for downloading an exchange document for an external user.
        /// </summary>
        /// <param name="documentReferenceString">The document reference information.</param>
        /// <returns>A <see cref="FileContentResult"/> containing the requested file.</returns>
        public async Task<IActionResult> DownloadExternalExchangeDocument(string documentReferenceString)
        {
            var userInfo = await UserInformationProvider.GetCurrentUserInfo();

            var file = await _exchangeDocumentDownloadService.DownloadExchangeDocumentByReference(
                documentReferenceString,
                userInfo);

            return File(file.Content, file.ContentType, file.Name);
        }

        private static Func<ExchangeDocumentEvent, bool> IsSentByOrganisationEvent()
            => documentEvent => documentEvent.EventType == ExchangeDocumentEventType.SentByOrganisation;

        private string GetSendNewDocumentActionName(bool showParentView)
        {
            return showParentView
                    ? nameof(SelectOrganisation)
                    : nameof(SelectDocumentType);
        }

        private async Task<int> GetNextVersionNumber(string uploadingForUkprn, string productIdentifier)
        {
            var uploadingForOrganisation = string.IsNullOrEmpty(uploadingForUkprn)
               ? await UserInformationProvider.GetCurrentOrganisationIdentifier()
               : CreateOrganisationIdentifierForUkprn(uploadingForUkprn);

            var currentVersion = await _exchangeApiClient.GetCurrentProductVersionForOrganisation(
                uploadingForOrganisation, productIdentifier);

            return currentVersion + 1;
        }

        private async Task<SentDocuments> GetSentDocuments(ListRequest request)
        {
            var currentOrganisationIdentifier = await UserInformationProvider.GetCurrentOrganisationIdentifier();

            var options = new ExchangeListOrganisationDocumentOptions
            {
                PageSize = _configuration.ListPageSize,
                DocumentStatusOption = ExchangeDocumentDirection.SentByOrganisation,
                OrganisationIdentifier = currentOrganisationIdentifier,
                PageNumber = request?.Page ?? 1,
                FilterOptions = _listHelper.GetFilterOptions(request)
            };

            var sentDocuments = await _exchangeApiClient.GetOrganisationDocuments(options);
            return new SentDocuments
            {
                ShowParentView = await UserInformationProvider.CurrentUserShouldSeeParentView(),
                Pagination = _listHelper.GetPaginationViewModel(request, sentDocuments),
                ListItems = CreateSentDocumentsListItems(sentDocuments.Items),
                FilterCategories = _listHelper.GetFilterCategories(sentDocuments),
                AnyDocumentsAvailable = _listHelper.AnyDocumentsAvailable(request, sentDocuments)
            };
        }

        private IEnumerable<SentDocument> CreateSentDocumentsListItems(IEnumerable<ExchangeDocument> exchangeDocuments)
        {
            return exchangeDocuments.Select(exchangeDocument =>
            {
                var sentDocument = CreateSentDocumentFromExchangeDocument(exchangeDocument);

                var oldestVersion = sentDocument.PreviousVersions.LastOrDefault();
                if (oldestVersion != null)
                {
                    oldestVersion.IsOldestVersion = true;
                }

                return sentDocument;
            });
        }

        private SentDocument CreateSentDocumentFromExchangeDocument(ExchangeDocument exchangeDocument)
        {
            var sentEvent = exchangeDocument.EventHistory?.SingleOrDefault(IsSentByOrganisationEvent());
            var sentDateTime = sentEvent?.EventDateTime ?? DateTime.MinValue;

            return new SentDocument
            {
                FileName = exchangeDocument.DocumentReference.FileName,
                ParentBatchIdentifier = exchangeDocument.DocumentReference.ParentBatchIdentifier,
                BatchIdentifier = exchangeDocument.DocumentReference.BatchIdentifier,
                ProductName = exchangeDocument.Product.Name,
                SenderOrganisationName = exchangeDocument.OrganisationInfo.Name,
                SenderName = sentEvent?.UserInfo.FullName ?? sentEvent?.UserInfo.Principal,
                SentDateTime = sentDateTime,
                Version = exchangeDocument.Version,
                DisplaySentDateTime = _dateTimeDisplayHelper.ToSentenceTimeAndDateDisplayString(sentDateTime),
                PreviousVersions = It.IsNull(exchangeDocument.PreviousVersions)
                    ? Collection.Empty<SentDocument>()
                    : exchangeDocument.PreviousVersions.Select(previousVersionDoc =>
                        CreateSentDocumentFromExchangeDocument(previousVersionDoc))
            };
        }

        private async Task<ReceivedDocuments> GetReceivedDocuments(ListRequest request)
        {
            var options = new ExchangeListOrganisationDocumentOptions()
            {
                OrganisationIdentifier = await UserInformationProvider.GetCurrentOrganisationIdentifier(),
                DocumentStatusOption = ExchangeDocumentDirection.PublishedByAgency,
                PageSize = Configuration.ListPageSize,
                PageNumber = request?.Page ?? 1,
                FilterOptions = _listHelper.GetFilterOptions(request)
            };

            var receivedDocuments = await _exchangeApiClient.GetOrganisationDocuments(options);
            var showParentView = await UserInformationProvider.CurrentUserShouldSeeParentView();

            return new ReceivedDocuments
            {
                ShowParentView = showParentView,
                Pagination = _listHelper.GetPaginationViewModel(request, receivedDocuments),
                ListItems = receivedDocuments.Items.Select(
                    document =>
                    {
                        var documentStatusInfo = _documentStatusProvider.GetDownloadStatus(document.EventHistory);
                        var receivedDate = document.EventHistory?.FirstOrDefault(e => e.EventType == ExchangeDocumentEventType.PublishedByAgency)?.EventDateTime ?? DateTime.MinValue;

                        return new ReceivedDocument
                        {
                            FileName = document.DocumentReference.FileName,
                            ProductName = document.Product.Name,
                            BatchIdentifier = document.DocumentReference.BatchIdentifier,
                            ParentBatchIdentifier = document.DocumentReference.ParentBatchIdentifier,
                            RecipientOrganisationName = document.OrganisationInfo.Name,
                            Status = documentStatusInfo.Status,
                            StatusDateTime = documentStatusInfo.DownloadedTime,
                            DownloadedBy = documentStatusInfo.DownloadedBy,
                            ReceivedDateTime = receivedDate,
                            DisplayReceivedDateTime = _dateTimeDisplayHelper.ToTimeAndDateDisplayString(receivedDate)
                        };
                    }).ToList(),
                FilterCategories = _listHelper.GetFilterCategories(receivedDocuments),
                AnyDocumentsAvailable = _listHelper.AnyDocumentsAvailable(request, receivedDocuments)
            };
        }

        private OrganisationIdentifier CreateOrganisationIdentifierForUkprn(string ukprn)
        {
            return new OrganisationIdentifier
            {
                Type = OrganisationIdentifierType.Ukprn,
                Value = ukprn
            };
        }

        private async Task<List<string>> GetAllowedFileExtensions()
        {
            var fileExtensions = await _settingsApiClient.GetFileExtensions();

            return fileExtensions
                    .Where(extension => CanUserUploadFileType(extension))
                    .Select(f => f.Extension.ToUpperInvariant())
                    .ToList();
        }

        private bool CanUserUploadFileType(FileExtensionInfo selectedFileType)
            => UserInformationProvider.IsCurrentUserExternal()
                ? selectedFileType.CanExternalUserUpload
                : selectedFileType.CanInternalUserUpload;
    }
}