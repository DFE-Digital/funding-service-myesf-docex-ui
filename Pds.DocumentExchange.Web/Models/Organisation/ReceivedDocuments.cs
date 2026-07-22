using Microsoft.AspNetCore.Mvc;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.Core.Web.Models;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Models.Shared;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Models.Organisation
{
    /// <summary>
    /// View model for the 'documents received' page.
    /// </summary>
    public class ReceivedDocuments : BaseOrganisationPageViewModel, IListViewModel, IDocumentListPage
    {
        #region Documents Received page-specific properties

        /// <inheritdoc/>
        public bool AnyDocumentsAvailable { get; set; }

        /// <inheritdoc/>
        public string NoDocumentsPartialName
            => "/Views/Organisation/Partial/_ReceivedDocumentsNoListItems.cshtml";

        /// <summary>
        /// Gets the caption to display on the table.
        /// </summary>
        public string TableCaption =>
            BasePageViewModel.AddHTMLExpansions("List of documents received from DfE");

        /// <summary>
        /// Gets the partial view that should be used to render the table header.
        /// </summary>
        public string TableHeaderPartialName
            => ShowParentView
                ? "/Views/Organisation/Partial/_ReceivedDocumentsTableHeaderParentView.cshtml"
                : "/Views/Organisation/Partial/_ReceivedDocumentsTableHeaderChildView.cshtml";

        /// <summary>
        /// Gets or sets a value indicating whether to show the parent view.
        /// </summary>
        public bool ShowParentView { get; set; }

        /// <summary>
        /// Gets or sets the action that caused the validation error.
        /// </summary>
        public string ErrorAction { get; set; }

        /// <inheritdoc/>
        public string ListContainerPartialName
            => "/Views/Organisation/Partial/_ReceivedDocumentsListContainer.cshtml";

        /// <inheritdoc/>
        public string NoListItemsPartialName
            => "/Views/Shared/_NoListItems.cshtml";

        /// <inheritdoc/>
        public string ListItemPartialName
            => ShowParentView
                ? "/Views/Organisation/Partial/_ReceivedDocumentParentView.cshtml"
                : "/Views/Organisation/Partial/_ReceivedDocumentChildView.cshtml";

        /// <inheritdoc/>
        public string ListItemTemplatePartialName
            => ShowParentView
                ? "/Views/Organisation/Partial/_ReceivedDocumentParentViewTemplate.cshtml"
                : "/Views/Organisation/Partial/_ReceivedDocumentChildViewTemplate.cshtml";

        /// <inheritdoc/>
        public bool ListItemsAreSelectable => ShowParentView;

        /// <inheritdoc/>
        public string ListItemSelectionInputName
            => nameof(IDocumentReferenceList.DocumentReferences);

        /// <summary>
        /// Gets or sets the list of received documents.
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
        {
            return url.Action(nameof(OrganisationController.ReceivedDocumentsData));
        }

        #endregion

        #region Base view model overrides

        /// <inheritdoc/>
        public override string ContentTitle
            => ShowParentView
            ? BasePageViewModel.AddHTMLExpansions("Documents received from DfE")
            : BasePageViewModel.AddHTMLExpansions("Documents you've received from DfE");

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => false;

        /// <inheritdoc/>
        // Hide the layout content title as we will render it in the view.
        public override bool ShowContentTitle => false;

        #endregion
    }
}
