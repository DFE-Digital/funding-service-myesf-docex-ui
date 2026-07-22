using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Pds.Core.Utils.Helpers;
using Pds.Core.Utils.Interfaces;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs.SupportTools;
using Pds.DocumentExchange.Web.Interfaces.Coordinators;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models;
using Pds.DocumentExchange.Web.Models.Agency;
using Pds.DocumentExchange.Web.Models.SupportTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Pds.DocumentExchange.Web.Helpers.AuthorizationHelper;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;
using static Pds.DocumentExchange.Web.Validations.AcademicYearValidation;
using static Pds.DocumentExchange.Web.Validations.UkprnValidation;

namespace Pds.DocumentExchange.Web.Controllers
{
    /// <summary>
    /// The support tool controller containing actions that can be accessed by agency, advanced and admin users.
    /// </summary>
    [Authorize(Policy = Policies.RequireAnyDocumentExchangeInternalUserRoles)]
    public class SupportToolsController : BaseDocumentExchangeController
    {
        private const string SelectDocumentTypeDropDownValue = "-1";

        private readonly ISupportToolsApiClient _supportToolsApiClient;
        private readonly ISettingsApiClient _settingsApiClient;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IAgencyExchangeDeletionRequestCoordinator _deletionRequestCoordinator;
        private readonly IListHelper _listHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportToolsController"/> class.
        /// </summary>
        /// <param name="userInformationProvider"><see cref="IUserInformationProvider"/>.</param>
        /// <param name="supportToolsApiClient">The support tools API client.</param>.
        /// <param name="settingsApiClient">The settings API client.</param>
        /// <param name="dateTimeProvider">The date time provider.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="deletionRequestCoordinator"><see cref="IAgencyExchangeDeletionRequestCoordinator"/>.</param>
        /// <param name="listHelper"><see cref="IListHelper"/>.</param>
        public SupportToolsController(
            IUserInformationProvider userInformationProvider,
            ISupportToolsApiClient supportToolsApiClient,
            ISettingsApiClient settingsApiClient,
            IDateTimeProvider dateTimeProvider,
            IOptions<DocumentExchangeConfiguration> configurationOptions,
            IAgencyExchangeDeletionRequestCoordinator deletionRequestCoordinator,
            IListHelper listHelper)
            : base(userInformationProvider, configurationOptions)
        {
            _supportToolsApiClient = supportToolsApiClient;
            _settingsApiClient = settingsApiClient;
            _dateTimeProvider = dateTimeProvider;
            _deletionRequestCoordinator = deletionRequestCoordinator;
            _listHelper = listHelper;
        }

        /// <summary>
        /// The index action.
        /// </summary>
        /// <returns>The index page view.</returns>
        [SupportToolsBreadCrumb(true)]
        public async Task<IActionResult> SupportTools()
        {
            var model = new SupportToolsModel
            {
                IsUserAdvancedOrAdmin = await UserInformationProvider.CurrentUserIsAdvancedAgencyUser() || await UserInformationProvider.CurrentUserIsAdminUser(),
                IsAnyDocumentExchangeInternalUser = await UserInformationProvider.CurrentUserCanAccessToSupportTools()
            };

            return View(model);
        }

        /// <summary>
        /// The documents published by DfE page action.
        /// </summary>
        /// <param name="request">The list request.</param>
        /// <returns>The index page view.</returns>
        [DocumentsPublishedByDfeBreadCrumb(true)]
        [Route("/[controller]/[action]")]
        [Authorize(Policy = Policies.RequireDocumentExchangeAdvancedOrAdminRoles)]
        public async Task<IActionResult> DocumentsPublishedByDfe(ListRequest request)
        {
            var publishedBatchesListResult = await _supportToolsApiClient.GetDocumentsPublishedByDfE(request.Page, Configuration.ListPageSize);

            var publishedBatchesItems = It.IsNull(publishedBatchesListResult.Items)
                ? Collection.EmptyAndReadOnly<PublishedBatchItem>()
                : publishedBatchesListResult.Items.Select(p => new PublishedBatchItem
                {
                    DateAndTime = p.DateAndTime,
                    ParentBatchIdentifier = p.ParentBatchIdentifier,
                    EmailAddress = p.EmailAddress,
                    NumberOfDocuments = p.NumberOfDocuments,
                    NumberOfEmails = p.NumberOfEmails
                }).AsSafeReadOnlyList();

            var viewModel = new PublishedBatchesViewModel
            {
                ListItems = publishedBatchesItems,
                Pagination = _listHelper.GetPaginationViewModel(request, publishedBatchesListResult)
            };

            return View(viewModel);
        }

        /// <summary>
        /// Action for downloading the published documents as a CSV file.
        /// </summary>
        /// <param name="parentBatchId">The parent batch ID.</param>
        /// <returns>A <see cref="FileContentResult"/> containing the requested file.</returns>
        [Authorize(Policy = Policies.RequireDocumentExchangeAdvancedOrAdminRoles)]
        public Task<IActionResult> DownloadDocumentsCsv(Guid parentBatchId)
            => DownloadCsv("Documents", () => _supportToolsApiClient.DownloadDocumentsPublishedCsv(parentBatchId));

        /// <summary>
        /// Action for downloading the email recipients as a CSV file.
        /// </summary>
        /// <param name="parentBatchId">The parent batch ID.</param>
        /// <returns>A <see cref="FileContentResult"/> containing the requested file.</returns>
        [Authorize(Policy = Policies.RequireDocumentExchangeAdvancedOrAdminRoles)]
        public Task<IActionResult> DownloadEmailsCsv(Guid parentBatchId)
            => DownloadCsv("Emails", () => _supportToolsApiClient.DownloadNotificationRecipientsCsv(parentBatchId));

        #region Reports

        /// <summary>
        /// Reports page action.
        /// </summary>
        /// <returns>The reports page view.</returns>
        [ReportsBreadCrumb(true)]
        [Route("/[controller]/[action]")]
        [Authorize(Policy = Policies.RequireAnyDocumentExchangeInternalUserRoles)]
        public IActionResult Reports()
        {
            return View(new ReportsViewModel());
        }

        /// <summary>
        /// Management information page action.
        /// </summary>
        /// <returns>The management information report page view.</returns>
        [Authorize(Policy = Policies.RequireAnyDocumentExchangeInternalUserRoles)]
        [ManagementInformationReport(true)]
        [Route("/[controller]/reports/[action]")]
        public IActionResult ManagementInformation()
        {
            return View(new ManagementInformationReportViewModel());
        }

        /// <summary>
        /// Action for downloading the management information report.
        /// </summary>
        /// <param name="model">The management information report view model.</param>
        /// <returns>Downloads MI report and returns management information report page view.</returns>
        [HttpPost]
        [ManagementInformationReport(true)]
        [Route("/[controller]/reports/[action]")]
        [Authorize(Policy = Policies.RequireAnyDocumentExchangeInternalUserRoles)]
        public async Task<IActionResult> ManagementInformation(ManagementInformationReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                var fromDate = new DateTime(model.FromYear.Value, model.FromMonth.Value, model.FromDay.Value);
                var toDate = new DateTime(model.ToYear.Value, model.ToMonth.Value, model.ToDay.Value);

                var result = await _supportToolsApiClient.DownloadMIReport(fromDate, toDate);
                return File(result, "application/vnd.oasis.opendocument.spreadsheet", $"Document exchange MI report {DateTime.UtcNow.ToString("dd-MM-yyyy")}.ods");
            }

            return View(model);
        }

        #endregion

        #region Delete

        /// <summary>
        /// The delete documents action.
        /// </summary>
        /// <param name="errorMessages">The error messages.</param>
        /// <param name="exchangeDocumentDirection">The exchange document direction.</param>
        /// <param name="ukprn">The UKPRN.</param>
        /// <param name="period">The period.</param>
        /// <param name="documentType">The document type.</param>
        /// <returns>The delete documents page view.</returns>
        [DeleteDocumentBreadCrumb(true)]
        [Route("/[controller]/[action]")]
        [Authorize(Policy = Policies.RequireDocumentExchangeAdvancedOrAdminRoles)]
        public async Task<IActionResult> DeleteDocuments(
            IEnumerable<string> errorMessages,
            ExchangeDocumentDirection exchangeDocumentDirection = ExchangeDocumentDirection.PublishedByAgency,
            string ukprn = "",
            string period = "",
            string documentType = SelectDocumentTypeDropDownValue)
        {
            var products = await _settingsApiClient.GetAllProducts();

            var productsDropDownList = new List<SelectListItem>();
            var selectDocumentTypeDropDownItem = new SelectListItem("Select document type", SelectDocumentTypeDropDownValue, false, true);

            productsDropDownList.Add(selectDocumentTypeDropDownItem);
            productsDropDownList.AddRange(products.OrderBy(p => p.Name).Select(p => new SelectListItem(p.Name, p.Identifier.ToString())));

            var selectedItem = productsDropDownList.SingleOrDefault(item => item.Value == documentType);
            if (selectedItem == null)
            {
                selectedItem = selectDocumentTypeDropDownItem;
            }

            selectedItem.Selected = true;

            return View(new DeleteDocumentsViewModel
            {
                ExchangeDocumentDirection = exchangeDocumentDirection,
                Ukprn = ukprn,
                DocumentType = selectedItem.Value,
                Period = period,
                DocumentTypes = productsDropDownList,
                ErrorMessages = errorMessages
            });
        }

        /// <summary>
        /// The select documents to delete action.
        /// </summary>
        /// <param name="model">The delete documents view model.</param>
        /// <returns>The delete documents page view.</returns>
        [DeleteDocumentBreadCrumb(true)]
        [Route("/[controller]/[action]")]
        [Authorize(Policy = Policies.RequireDocumentExchangeAdvancedOrAdminRoles)]
        public async Task<IActionResult> SelectDocumentsToDelete(DeleteDocumentsViewModel model)
        {
            var errorMessages = new List<string>();

            if (!model.Ukprn.IsValidUkprn())
            {
                errorMessages.Add("The UKPRN must be an 8 digits number.");
            }

            if (string.IsNullOrWhiteSpace(model.DocumentType) || model.DocumentType == SelectDocumentTypeDropDownValue)
            {
                errorMessages.Add("The document type must be selected.");
            }


            if (model.ExchangeDocumentDirection == ExchangeDocumentDirection.PublishedByAgency && !model.Period.IsValidAcademicYear())
            {
                errorMessages.Add("The period must be a 6 digits number (i.e. 202021).");
            }

            if (errorMessages.Any())
            {
                return RedirectToActionPreserveMethod(
                    nameof(DeleteDocuments),
                    NameOf<SupportToolsController>(),
                    new { errorMessages });
            }

            var deleteFileInfo = await _deletionRequestCoordinator.GetDocumentsToDelete(model.ExchangeDocumentDirection, int.Parse(model.Ukprn), model.DocumentType, model.Period);

            if (!deleteFileInfo?.Any() ?? true)
            {
                errorMessages.Add("Could not find any documents matching submitted criteria!");
            }
            else if (deleteFileInfo.Count() > 1)
            {
                errorMessages.Add("Submitted criteria returned more than 1 document!");
            }

            if (errorMessages.Any())
            {
                return RedirectToActionPreserveMethod(
                    nameof(DeleteDocuments),
                    NameOf<SupportToolsController>(),
                    new { errorMessages });
            }

            var documentToDelete = deleteFileInfo.Single();

            if (!documentToDelete.PreviousVersions?.Any() ?? true)
            {
                DeletePublishedDocumentAreYouSure actionData = new DeletePublishedDocumentAreYouSure();
                actionData.DocumentReferences = new List<string> { documentToDelete.Id };
                actionData.SelectedDocument = documentToDelete.Id;

                actionData.ListItems = deleteFileInfo;

                return View(nameof(DeletePublishedDocumentAreYouSure), actionData);
            }
            else
            {
                DeleteDocumentsVersionSelect actionData = new DeleteDocumentsVersionSelect();
                actionData.DocumentReferences = new List<string> { documentToDelete.Id };

                actionData.ListItems = deleteFileInfo;

                return View(nameof(DeleteDocumentsVersionSelect), actionData);
            }
        }

        /// <summary>
        /// Action for deleting the selected document.
        /// </summary>
        /// <param name="actionData">An object containing the id of the document to be deleted.</param>
        /// <returns>Deletes the selected document and returns the deleted versions page view.</returns>
        [HttpPost]
        [DeletePublishedDocumentAreYouSureBreadCrumb(true)]
        [Authorize(Policy = Policies.RequireDocumentExchangeAdvancedOrAdminRoles)]
        public async Task<IActionResult> DeletePublishedDocument(
        DeletePublishedDocumentAreYouSure actionData)
        {
            var selectedDocument = actionData?.SelectedDocument;

            if (string.IsNullOrEmpty(selectedDocument))
            {
                return RedirectToAction(
                     nameof(SupportToolsController.SupportTools),
                     NameOf<SupportToolsController>(),
                     new
                     {
                         error = true,
                         errorAction = nameof(DeletePublishedDocument)
                     });
            }

            var deletedDocument = await _deletionRequestCoordinator.DeleteDocumentVersion(selectedDocument);

            var mappedActionData = MapToDeleteDocumentVersionsConfirmation(new List<AgencyExchangeDocument>() { deletedDocument }, false);

            return View("DeleteDocumentVersionsConfirmation", mappedActionData);
        }

        /// <summary>
        /// Action for deleting the selected document.
        /// </summary>
        /// <param name="actionData">An object containing the id of the document to be deleted.</param>
        /// <returns>Deletes the selected version and returns the deleted versions page view.</returns>
        [HttpPost]
        [DeleteDocumentsSelectVersionBreadCrumb(true)]
        [Authorize(Policy = Policies.RequireDocumentExchangeAdvancedOrAdminRoles)]
        public async Task<IActionResult> DeleteSelectedDocumentVersion(
        DeleteDocumentsVersionSelect actionData)
        {
            var selectedVersion = actionData?.SelectedVersion;

            if (string.IsNullOrEmpty(selectedVersion))
            {
                return RedirectToAction(
                     nameof(SupportToolsController.SupportTools),
                     NameOf<SupportToolsController>(),
                     new
                     {
                         error = true,
                         errorAction = nameof(DeleteSelectedDocumentVersion)
                     });
            }

            var mappedActionData = new DeleteDocumentVersionsConfirmation();
            var deletedDocumentsData = new List<AgencyExchangeDocument>();

            if (selectedVersion.StartsWith("AllVersions"))
            {
                if (actionData?.DocumentReferences?.Any() != true)
                {
                    return RedirectToAction(
                        nameof(SupportToolsController.SupportTools),
                        NameOf<SupportToolsController>(),
                        new
                        {
                            error = true,
                            errorAction = nameof(DeleteSelectedDocumentVersion)
                        });
                }

                var deletedDocuments = await _deletionRequestCoordinator.DeleteDocuments(actionData.DocumentReferences);
                deletedDocumentsData = deletedDocuments.ToList();
                mappedActionData = MapToDeleteDocumentVersionsConfirmation(deletedDocumentsData, true);
            }
            else
            {
                var deletedDocument = await _deletionRequestCoordinator.DeleteDocumentVersion(selectedVersion);
                deletedDocumentsData.Add(deletedDocument);
                mappedActionData = MapToDeleteDocumentVersionsConfirmation(deletedDocumentsData, false);
            }

            return View("DeleteDocumentVersionsConfirmation", mappedActionData);
        }

        /// <summary>
        /// Maps list of AgencyExchangeDocument to DeleteDocumentVersionsConfirmation model.
        /// </summary>
        /// <param name="deletedDocuments">list of deleted documents.</param>
        /// <param name="isAllVersions">bool for if all versions are deleted.</param>
        /// <returns>DeleteDocumentVersionsConfirmation model.</returns>
        private DeleteDocumentVersionsConfirmation MapToDeleteDocumentVersionsConfirmation(IEnumerable<AgencyExchangeDocument> deletedDocuments, bool isAllVersions)
        {
            var firstDocument = deletedDocuments.First();

            var joinedVersions = string.Empty;

            if (isAllVersions)
            {
                var listVersions = firstDocument.PreviousVersions == null ? new List<string>() : firstDocument.PreviousVersions.Select(y => y.VersionNumber.ToString()).ToList();
                listVersions.Add(firstDocument.VersionNumber.ToString());
                joinedVersions = string.Join(", ", listVersions.OrderBy(x => x));
            }
            else
            {
                joinedVersions = firstDocument.VersionNumber.ToString();
            }

            var confirmationModel = new DeleteDocumentVersionsConfirmation()
            {
                IsAllVersions = isAllVersions,
                ProviderName = firstDocument.ProviderName,
                ProductName = firstDocument.ProductName,
                ProviderUkprn = firstDocument.ProviderUkprn,
                Version = joinedVersions
            };

            return confirmationModel;
        }

        private async Task<IActionResult> DownloadCsv(string filePrefix, Func<Task<byte[]>> downloadCsvAction)
        {
            var fileContent = await downloadCsvAction();
            var fileName = $"{filePrefix}_{_dateTimeProvider.Now()}.csv";

            return File(fileContent, "text/csv", fileName);
        }


        #endregion
    }
}