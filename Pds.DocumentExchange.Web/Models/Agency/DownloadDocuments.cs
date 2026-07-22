using Microsoft.AspNetCore.Mvc;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Models.Shared;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Models.Agency
{
    /// <summary>
    /// View model for the 'manage documents' page.
    /// </summary>
    public class DownloadDocuments : BaseAgencyPageViewModel, IListViewModel, IDocumentListPage
    {
        #region Download page-specific properties

        /// <inheritdoc/>
        public bool AnyDocumentsAvailable { get; set; }

        /// <inheritdoc/>
        public string NoDocumentsPartialName
            => "/Views/Agency/Partial/_DownloadDocumentsNoListItems.cshtml";

        /// <summary>
        /// Gets the caption to display on the table.
        /// </summary>
        public string TableCaption
            => "List of documents to download";

        /// <summary>
        /// Gets or sets a value indicating whether the page has a validation error.
        /// </summary>
        public bool Error { get; set; }

        /// <summary>
        /// Gets or sets the action that caused the validation error.
        /// </summary>
        public string ErrorAction { get; set; }

        /// <inheritdoc/>
        public string ListContainerPartialName
            => "/Views/Agency/Partial/_DownloadDocumentsListContainer.cshtml";

        /// <inheritdoc/>
        public string NoListItemsPartialName
            => "/Views/Shared/_NoListItems.cshtml";

        /// <inheritdoc/>
        public string ListItemPartialName
            => "/Views/Agency/Partial/_AgencyExchangeDocument.cshtml";

        /// <inheritdoc/>
        public string ListItemTemplatePartialName
            => "/Views/Agency/Partial/_AgencyExchangeDocumentTemplate.cshtml";

        /// <inheritdoc/>
        public bool ListItemsAreSelectable => true;

        /// <inheritdoc/>
        public string ListItemSelectionInputName
            => nameof(IDocumentReferenceList.DocumentReferences);

        /// <summary>
        /// Gets or sets the list of documents to download.
        /// </summary>
        public IEnumerable<BaseListItem> ListItems { get; set; }

        /// <inheritdoc/>
        public IEnumerable<IFilterCategoryViewModel> FilterCategories { get; set; }

        /// <inheritdoc/>
        public PaginationViewModel Pagination { get; set; }

        /// <inheritdoc/>
        public string ItemTypeSingular => ServiceConstants.DocumentSingular;

        /// <inheritdoc/>
        public string ItemTypePlural => ServiceConstants.DocumentsPlural;

        /// <inheritdoc/>
        public string ListItemAsyncDataEndPoint(IUrlHelper url)
            => url.Action(nameof(AgencyController.DocumentsToDownloadData));

        /// <summary>
        /// Gets or sets a value indicating whether the user can access advanced functionality e.g. delete documents.
        /// </summary>
        public bool UserIsAdvancedAgencyUser { get; set; }

        #endregion


        #region Base view model overrides

        /// <inheritdoc/>
        // Hide the layout content title as we will render it in the view.
        public override bool ShowContentTitle => false;

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => false;

        /// <inheritdoc/>
        protected override string Title
            => "Download your documents";

        #endregion
    }
}