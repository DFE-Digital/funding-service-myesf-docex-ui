using Pds.Core.Web.Components.Areas.Lists.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Pds.DocumentExchange.Web.DTOs
{
    /// <summary>
    /// Class containing the data required to update a list of documents.
    /// </summary>
    public class DocumentListUpdateData : IListUpdateData
    {
        /// <summary>
        /// Gets or sets the updated list of documents.
        /// </summary>
        public IEnumerable<BaseListItem> ListItems { get; set; }

        /// <inheritdoc/>
        public PaginationUpdateData Pagination { get; set; }

        /// <inheritdoc/>
        public IEnumerable<PageUpdateItem> PageUpdateItems { get; set; } = Enumerable.Empty<PageUpdateItem>();
    }
}