using Microsoft.AspNetCore.Mvc;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Web.DTOs;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Models.Agency
{
    /// <summary>
    /// Shared view model for pages within the delete documents journey.
    /// </summary>
    public class DeleteDocuments : BaseAgencyPageViewModel, IDocumentReferenceList, IListViewModel
    {
        /// <inheritdoc/>
        public IEnumerable<string> DocumentReferences { get; set; }

        /// <inheritdoc/>
        public string ListContainerPartialName
            => "/Views/Agency/Partial/_DeleteDocumentsListContainer.cshtml";

        /// <inheritdoc/>
        public string NoListItemsPartialName
            => "/Views/Shared/_NoListItems.cshtml";

        /// <inheritdoc/>
        public string ListItemPartialName
            => "/Views/Agency/Partial/_DeleteAgencyExchangeDocument.cshtml";

        /// <inheritdoc/>
        public string ListItemTemplatePartialName
            => "/Views/Agency/Partial/_DeleteAgencyExchangeDocumentTemplate.cshtml";

        /// <inheritdoc/>
        public IEnumerable<BaseListItem> ListItems { get; set; }

        /// <inheritdoc/>
        public bool ListItemsAreSelectable => false;

        /// <inheritdoc/>
        public string ListItemSelectionInputName => null;

        /// <inheritdoc/>
        public IEnumerable<IFilterCategoryViewModel> FilterCategories { get; set; }

        /// <summary>
        /// Gets or sets who has published the documents (if applicable).
        /// </summary>
        public IEnumerable<string> Publishers { get; set; }

        /// <inheritdoc/>
        public PaginationViewModel Pagination { get; set; }

        /// <inheritdoc/>
        public string ItemTypeSingular => ServiceConstants.DocumentSingular;

        /// <inheritdoc/>
        public string ItemTypePlural => ServiceConstants.DocumentsPlural;

        /// <inheritdoc/>
        public string ListItemAsyncDataEndPoint(IUrlHelper url) => null;
    }
}