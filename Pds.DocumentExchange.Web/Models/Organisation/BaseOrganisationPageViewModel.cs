namespace Pds.DocumentExchange.Web.Models.Organisation
{
    /// <summary>
    /// A base view model for holding the shared properties of the pages in the organisation/external area.
    /// </summary>
    public abstract class BaseOrganisationPageViewModel : BaseDocumentExchangePageViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether the page has a validation error.
        /// </summary>
        public bool Error { get; set; }
    }
}