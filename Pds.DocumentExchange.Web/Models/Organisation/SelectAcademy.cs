using Pds.DocumentExchange.Web.Models.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Pds.DocumentExchange.Web.Models.Organisation
{
    /// <summary>
    /// View model for the 'select an academy' page.
    /// </summary>
    public class SelectAcademy : BaseOrganisationPageViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether the user chose to select a child academy.
        /// </summary>
        public bool? SelectChildAcademy { get; set; }

        /// <summary>
        /// Gets or sets the selected page number.
        /// (To support non-JS users).
        /// </summary>
        public int? PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the user's search term for filtering the list of academies.
        /// (To support non-JS users).
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user pressed the 'clear search' button.
        /// (To support postback for non-JS users).
        /// </summary>
        public bool ClearSearch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user pressed the search button.
        /// (To support postback for non-JS users).
        /// </summary>
        public bool NewSearch { get; set; }

        /// <summary>
        /// Gets or sets the list of child academies.
        /// </summary>
        public IEnumerable<IEnumerable<ChildOrganisationInfo>> ChildAcademyPages { get; set; }
            = Enumerable.Empty<IEnumerable<ChildOrganisationInfo>>();

        /// <summary>
        /// Gets the view model for the pagination controls.
        /// </summary>
        public Pagination Pagination
        {
            get
            {
                var pages = ChildAcademyPages?.Select(p => p.ToList()).ToList()
                    ?? Enumerable.Empty<List<ChildOrganisationInfo>>();
                return new Pagination
                {
                    PageSize = pages.Any() ? pages.First().Count() : 0,
                    TotalItems = pages.Sum(p => p.Count()),
                    TotalPages = pages.Count(),
                    SelectedPageNumber = PageNumber,
                    BuildPageLinkRouteValues = page => new
                    {
                        SelectChildAcademy = true,
                        PageNumber = page,
                        SearchTerm = SearchTerm
                    }
                };
            }
        }

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => true;

        /// <inheritdoc/>
        // Hide the content title in the layout as it is explicitly included in the view.
        public override bool ShowContentTitle => false;

        /// <inheritdoc/>
        protected override string Title
            => "Select which academy you want to send a document for";
    }
}