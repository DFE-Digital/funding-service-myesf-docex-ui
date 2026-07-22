using Microsoft.AspNetCore.Mvc;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Web.Models;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Areas.Admin.Models
{
    /// <summary>
    /// View model for the products page.
    /// </summary>
    public class Products : BaseDocumentExchangePageViewModel, IListViewModel
    {
        /// <summary>
        /// Gets the partial view that should be used to render the table header.
        /// </summary>
        public string TableHeaderPartialName
            => "/Areas/Admin/Views/Settings/Partial/_ProductsTableHeaderView.cshtml";

        /// <inheritdoc/>
        public string ListContainerPartialName
            => "/Areas/Admin/Views/Settings/Partial/_ProductsListContainer.cshtml";

        /// <inheritdoc/>
        public string NoListItemsPartialName
             => "/Areas/Admin/Views/Shared/_NoListItems.cshtml";

        /// <inheritdoc/>
        public string ListItemPartialName
            => "/Areas/Admin/Views/Settings/Partial/_Product.cshtml";

        /// <inheritdoc/>
        public string ListItemTemplatePartialName
            => "/Areas/Admin/Views/Settings/Partial/_ProductsViewTemplate.cshtml";

        /// <inheritdoc/>
        public bool ListItemsAreSelectable
            => false;

        /// <inheritdoc/>
        public string ListItemSelectionInputName
            => null;

        /// <summary>
        /// Gets or sets the list of received products.
        /// </summary>
        public IEnumerable<BaseListItem> ListItems { get; set; }

        /// <inheritdoc/
        public IEnumerable<IFilterCategoryViewModel> FilterCategories { get; set; }

        /// <inheritdoc/>
        public PaginationViewModel Pagination { get; set; }

        /// <inheritdoc/>
        public string ItemTypeSingular
            => "product";

        /// <inheritdoc/>
        public string ItemTypePlural
            => "products";

        /// <inheritdoc/>
        public string ListItemAsyncDataEndPoint(IUrlHelper url)
        {
            return null;
        }

        #region Base view model overrides

        /// <inheritdoc/>
        protected override string Title
            => "Product settings";

        #endregion
    }
}