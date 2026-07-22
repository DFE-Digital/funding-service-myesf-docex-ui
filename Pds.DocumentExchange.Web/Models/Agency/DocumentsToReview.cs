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
    /// View model for the 'documents to review' page.
    /// </summary>
    public class DocumentsToReview : BaseAgencyPageViewModel, IListViewModel, IDocumentListPage
    {
        #region Review page-specific properties

        /// <inheritdoc/>
        public bool AnyDocumentsAvailable { get; set; }

        /// <inheritdoc/>
        public string NoDocumentsPartialName
            => "/Views/Agency/Partial/_DocumentsToReviewNoListItems.cshtml";

        /// <summary>
        /// Gets the caption to display on the table.
        /// </summary>
        public string TableCaption
            => "List of documents to review";

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
            => "/Views/Agency/Partial/_DocumentsToReviewListContainer.cshtml";

        /// <inheritdoc/>
        public string NoListItemsPartialName
            => "/Views/Shared/_NoListItems.cshtml";

        /// <inheritdoc/>
        public string ListItemPartialName
            => "/Views/Agency/Partial/_InvalidFileShareDocument.cshtml";

        /// <inheritdoc/>
        public string ListItemTemplatePartialName
            => "/Views/Agency/Partial/_InvalidFileShareDocumentTemplate.cshtml";

        /// <inheritdoc/>
        public bool ListItemsAreSelectable => true;

        /// <inheritdoc/>
        public string ListItemSelectionInputName
            => nameof(IFileShareDocumentReferenceList.FileShareDocumentReferences);

        /// <summary>
        /// Gets or sets the list of documents to review.
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
            => url.Action(nameof(AgencyController.DocumentsToReviewData));

        #endregion


        #region Base view model overrides

        /// <inheritdoc/>
        // Hide the layout content title as we will render it in the view.
        public override bool ShowContentTitle => false;

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => false;

        /// <inheritdoc/>
        protected override string Title
            => "Review your document names";

        #endregion
    }
}