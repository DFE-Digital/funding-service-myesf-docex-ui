using Microsoft.AspNetCore.Mvc;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Web.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace Pds.DocumentExchange.Web.Models.SupportTools
{
    /// <summary>
    /// View model for the 'published batches' page.
    /// </summary>
    public class PublishedBatchesViewModel : BaseDocumentExchangePageViewModel, IListViewModel
    {
        /// <summary>
        /// Gets the caption to display on the table.
        /// </summary>
        public string TableCaption
            => AddHTMLExpansions("Documents published by DfE");

        /// <summary>
        /// Gets the partial view that should be used to render the table header.
        /// </summary>
        public string TableHeaderPartialName
            => "/Views/SupportTools/Partial/_PublishedBatchesTableHeader.cshtml";

        /// <inheritdoc/>
        public string ListContainerPartialName => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public string NoListItemsPartialName
            => "/Views/SupportTools/Partial/_PublishedBatchesNoListItems.cshtml";

        /// <inheritdoc/>
        public string ListItemPartialName
            => "/Views/SupportTools/Partial/_PublishedBatchesItemView.cshtml";

        /// <inheritdoc/>
        public string ListItemTemplatePartialName
            => "/Views/SupportTools/Partial/_PublishedBatchesItemView.cshtml";

        /// <inheritdoc/>
        public bool ListItemsAreSelectable => false;

        /// <inheritdoc/>
        public string ListItemSelectionInputName => string.Empty;

        /// <inheritdoc/>
        public IEnumerable<BaseListItem> ListItems { get; set; }

        /// <inheritdoc/>
        public IEnumerable<IFilterCategoryViewModel> FilterCategories { get; set; }

        /// <inheritdoc/>
        public PaginationViewModel Pagination { get; set; }

        /// <inheritdoc/>
        public string ItemTypeSingular => ServiceConstants.BatchSingular;

        /// <inheritdoc/>
        public string ItemTypePlural => ServiceConstants.BatchesPlural;

        /// <inheritdoc/>
        protected override string Title
            => "Documents published by DfE";

        /// <inheritdoc/>
        public string ListItemAsyncDataEndPoint(IUrlHelper url)
            => url.Action(nameof(SupportToolsController.DocumentsPublishedByDfe));

        /// <summary>
        /// Gets a value indicating whether the list item list contains at least one element.
        /// </summary>
        public bool AnyListItems
            => ListItems?.Any() == true;
    }
}