using Pds.DocumentExchange.Web.Models;

namespace Pds.DocumentExchange.Web.Areas.ViewAsOrganisation.Models
{
    /// <summary>
    /// View model for the organisation search input page.
    /// </summary>
    public class SearchForAnOrganisation : BaseDocumentExchangePageViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether there is a validation error.
        /// </summary>
        public bool Error { get; set; }

        /// <inheritdoc/>
        protected override string Title => "Search for an organisation";
    }
}