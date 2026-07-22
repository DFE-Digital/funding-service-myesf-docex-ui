using Pds.DocumentExchange.Web.Models;

namespace Pds.DocumentExchange.Web.Areas.Admin.Models
{
    /// <summary>
    /// View model for the confirm add/edit page.
    /// </summary>
    public class ConfirmAddEditPageViewModel : BaseDocumentExchangePageViewModel
    {
        /// <summary>
        /// Gets or sets the product to be edited/added.
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the page is add or edit.
        /// </summary>
        public bool IsAddPage { get; set; }

        /// <summary>
        /// Gets or sets the team's friendly name.
        /// </summary>
        public string AgencyTeamFriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the old identifier value.
        /// </summary>
        public int OldIdentifier { get; set; }

        /// <inheritdoc/>
        protected override string Title => IsAddPage ? "Confirm Add a New Product" : "Confirm Edit Product";
    }
}