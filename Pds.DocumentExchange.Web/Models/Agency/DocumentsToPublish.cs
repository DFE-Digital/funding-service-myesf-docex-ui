using Microsoft.AspNetCore.Mvc;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.Models.Shared;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Models.Agency
{
    /// <summary>
    /// View model for the 'documents to publish' page.
    /// </summary>
    public class DocumentsToPublish : BaseAgencyPageViewModel, IListViewModel, IDocumentListPage
    {
        #region Publish page-specific properties

        /// <inheritdoc/>
        public bool AnyDocumentsAvailable { get; set; }

        /// <inheritdoc/>
        public string NoDocumentsPartialName
            => "/Views/Agency/Partial/_DocumentsToPublishNoListItems.cshtml";

        /// <summary>
        /// Gets the caption to display on the table.
        /// </summary>
        public string TableCaption
            => "List of documents to publish";

        /// <summary>
        /// Gets or sets the currently selected product.
        /// </summary>
        public Product SelectedProduct { get; set; }

        /// <summary>
        /// Gets or sets the currently selected team.
        /// </summary>
        public string SelectedTeam { get; set; }

        /// <inheritdoc/>
        public string ListContainerPartialName
            => "/Views/Agency/Partial/_DocumentsToPublishListContainer.cshtml";

        /// <inheritdoc/>
        public string NoListItemsPartialName
            => "/Views/Shared/_NoListItems.cshtml";

        /// <inheritdoc/>
        public string ListItemPartialName
            => "/Views/Agency/Partial/_FileShareDocument.cshtml";

        /// <inheritdoc/>
        public string ListItemTemplatePartialName
            => "/Views/Agency/Partial/_FileShareDocumentTemplate.cshtml";

        /// <inheritdoc/>
        public bool ListItemsAreSelectable => false;

        /// <inheritdoc/>
        public string ListItemSelectionInputName => null;

        /// <summary>
        /// Gets or sets the list of documents to publish.
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
            return url.Action(nameof(AgencyController.DocumentsToPublishData));
        }

        #endregion


        #region Base view model overrides

        /// <inheritdoc/>
        public override string ContentTitle
            => "Publish your documents";

        /// <inheritdoc/>
        // Hide the layout content title as we will render it in the view.
        public override bool ShowContentTitle => false;

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => false;

        #endregion
    }
}