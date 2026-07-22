using System;

namespace Pds.DocumentExchange.Web.Models.Shared
{
    /// <summary>
    /// A view model for pagination controls.
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// Gets or sets the maximum number of items per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total number of items.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the selected page number.
        /// </summary>
        public int? SelectedPageNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the top margin should be included.
        /// </summary>
        public bool ShowTopMargin { get; set; } = false;

        /// <summary>
        /// Gets the page number being displayed.
        /// </summary>
        public int VisiblePageNumber
            => SelectedPageNumber.GetValueOrDefault(1);

        /// <summary>
        /// Gets the first item number on the selected page.
        /// </summary>
        public int FirstItemNumberOnSelectedPage
        {
            get
            {
                if (FirstPageIsSelected())
                {
                    return 1;
                }

                return ((VisiblePageNumber - 1) * PageSize) + 1;
            }
        }

        /// <summary>
        /// Gets the last item number on the selected page.
        /// </summary>
        public int LastItemNumberOnSelectedPage
        {
            get
            {
                if (FirstPageIsSelected())
                {
                    return Math.Min(TotalItems, PageSize);
                }

                if (LastPageIsSelected())
                {
                    return TotalItems;
                }

                return PageSize * VisiblePageNumber;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to show a control linking to the previous page.
        /// </summary>
        public bool ShowPrevious
            => VisiblePageNumber > 1;

        /// <summary>
        /// Gets a value indicating whether to show a control linking to the next page.
        /// </summary>
        public bool ShowNext
            => VisiblePageNumber < TotalPages;

        /// <summary>
        /// Gets or sets a function that takes a page number as a parameter and returns the route values
        /// that should be included when building the links to the other pages.
        /// </summary>
        public Func<int, object> BuildPageLinkRouteValues { get; set; }
            = page => new { SelectedPageNumber = page };

        private bool FirstPageIsSelected()
            => VisiblePageNumber == 1;

        private bool LastPageIsSelected()
            => VisiblePageNumber == TotalPages;
    }
}