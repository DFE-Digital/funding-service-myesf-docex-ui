using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Services.Models.Filters;
using Pds.DocumentExchange.Web.DTOs;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Interfaces.Helpers
{
    /// <summary>
    /// Helper functions for processing data for list pages.
    /// </summary>
    public interface IListHelper
    {
        /// <summary>
        /// Gets the document list update data from the list view model.
        /// </summary>
        /// <param name="viewModel">The list view model.</param>
        /// <returns>The document list update data.</returns>
        DocumentListUpdateData GetDocumentListUpdateData(IListViewModel viewModel);

        /// <summary>
        /// Gets a value indicating whether any documents are available.
        /// </summary>
        /// <typeparam name="T">The type of the list items.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="listResult">The list result.</param>
        /// <returns>A value indicating whether and documents are available.</returns>
        bool AnyDocumentsAvailable<T>(ListRequest request, ListResult<T> listResult);

        /// <summary>
        /// Gets the filter options from the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="uses6MonthLimit">Bool to determine if the date range should use the 6 month limit as default.</param>
        /// <returns>The filter options.</returns>
        IEnumerable<IFilterOption> GetFilterOptions(ListRequest request, bool uses6MonthLimit = false);

        /// <summary>
        /// Gets the pagination view model.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="listResult">The list result.</param>
        /// <returns>The pagination view model.</returns>
        PaginationViewModel GetPaginationViewModel<T>(ListRequest request, ListResult<T> listResult);

        /// <summary>
        /// Gets the filter categories from the list result.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="listResult">The list result.</param>
        /// <returns>The list of filter categories.</returns>
        IEnumerable<IFilterCategoryViewModel> GetFilterCategories<T>(ListResult<T> listResult);
    }
}