using Pds.DocumentExchange.Web.Models;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Areas.ViewAsOrganisation.Models
{
    /// <summary>
    /// View model for the organisation search results.
    /// </summary>
    public class OrganisationSearchResults : BaseDocumentExchangePageViewModel
    {
        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the organisations.
        /// </summary>
        public IReadOnlyCollection<(string Ukprn, string Name)> Organisations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the search term could have more results that were truncated.
        /// </summary>
        public bool HasMoreResults { get; set; }

        /// <inheritdoc/>
        protected override string Title => "Organisation search results";
    }
}