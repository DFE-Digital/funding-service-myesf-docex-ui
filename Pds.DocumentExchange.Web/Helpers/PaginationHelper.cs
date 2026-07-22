using System;
using System.Collections.Generic;
using System.Linq;

namespace Pds.DocumentExchange.Web.Helpers
{
    /// <summary>
    /// Helper class for paginating enumerable collections of items.
    /// </summary>
    public static class PaginationHelper
    {
        /// <summary>
        /// Paginates the provided enumerable collection to the given page size.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="items">The input collection of items.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>A collection of collections of items, where each nested collection
        /// has no more elements than the page size.</returns>
        public static IEnumerable<IEnumerable<T>> Paginate<T>(this IEnumerable<T> items, int pageSize)
        {
            if (pageSize < 1)
            {
                throw new ArgumentException("Page size must be at least 1", nameof(pageSize));
            }

            var result = new List<List<T>>();
            var currentPage = new List<T>();

            foreach (var item in items)
            {
                currentPage.Add(item);
                if (currentPage.Count() == pageSize)
                {
                    result.Add(currentPage);
                    currentPage = new List<T>();
                }
            }

            if (currentPage.Any())
            {
                result.Add(currentPage);
            }

            return result;
        }
    }
}