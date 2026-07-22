using Microsoft.AspNetCore.Mvc;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.Core.Web.Models;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.Models.Shared;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Models.Organisation
{
    /// <summary>
    /// View model for the 'sent documents' page.
    /// </summary>
    public class SentDocuments : BaseOrganisationPageViewModel, IListViewModel, IDocumentListPage
    {
        #region Sent documents page-specific properties

        /// <inheritdoc/>
        public bool AnyDocumentsAvailable { get; set; }

        /// <inheritdoc/>
        public string NoDocumentsPartialName
            => "/Views/Organisation/Partial/_SentDocumentsNoListItems.cshtml";

        /// <summary>
        /// Gets the caption to display on the table.
        /// </summary>
        public string TableCaption
            => BasePageViewModel.AddHTMLExpansions("List of documents sent to DfE");

        /// <summary>
        /// Gets the partial view that should be used to render the table header.
        /// </summary>
        public string TableHeaderPartialName
            => ShowParentView
                ? "/Views/Organisation/Partial/_SentDocumentsTableHeaderParentView.cshtml"
                : "/Views/Organisation/Partial/_SentDocumentsTableHeaderChildView.cshtml";

        /// <summary>
        /// Gets or sets a value indicating whether to show the parent view.
        /// </summary>
        public bool ShowParentView { get; set; }

        /// <inheritdoc/>
        public string ListContainerPartialName
            => "/Views/Organisation/Partial/_SentDocumentsListContainer.cshtml";

        /// <inheritdoc/>
        public string NoListItemsPartialName
            => "/Views/Shared/_NoListItems.cshtml";

        /// <inheritdoc/>
        public string ListItemPartialName
            => ShowParentView
                ? "/Views/Organisation/Partial/_SentDocumentParentView.cshtml"
                : "/Views/Organisation/Partial/_SentDocumentChildView.cshtml";

        /// <inheritdoc/>
        public string ListItemTemplatePartialName
            => ShowParentView
                ? "/Views/Organisation/Partial/_SentDocumentParentViewTemplate.cshtml"
                : "/Views/Organisation/Partial/_SentDocumentChildViewTemplate.cshtml";

        /// <inheritdoc/>
        public bool ListItemsAreSelectable => false;

        /// <inheritdoc/>
        // Not applicable as this is not a selectable list.
        public string ListItemSelectionInputName => null;

        /// <summary>
        /// Gets or sets the list of sent documents.
        /// </summary>
        public IEnumerable<BaseListItem> ListItems { get; set; }

        /// <inheritdoc/>
        public IEnumerable<IFilterCategoryViewModel> FilterCategories { get; set; }

        /// <inheritdoc/>
        public string ItemTypeSingular => ServiceConstants.DocumentSingular;

        /// <inheritdoc/>
        public string ItemTypePlural => ServiceConstants.DocumentsPlural;

        /// <inheritdoc/>
        public PaginationViewModel Pagination { get; set; }

        /// <inheritdoc/>
        public string ListItemAsyncDataEndPoint(IUrlHelper url)
        {
            return url.Action(nameof(OrganisationController.SentDocumentsData));
        }

        #endregion


        #region Base view model overrides

        /// <inheritdoc/>
        // Hide the layout content title as we will render it in the view.
        public override bool ShowContentTitle => false;

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => false;

        /// <inheritdoc/>
        protected override string Title
            => "Documents sent to DfE";

        #endregion
    }
}