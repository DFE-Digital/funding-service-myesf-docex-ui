using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Models.DocumentExchange
{
    /// <summary>
    /// View model for the account settings page.
    /// </summary>
    public class AccountSettings : BaseDocumentExchangePageViewModel
    {
        /// <summary>
        /// Gets or sets the list of the user's permissions.
        /// </summary>
        public IEnumerable<string> Permissions { get; set; }

        /// <inheritdoc />
        public override bool IsTwoThirdsLayout => true;

        /// <inheritdoc/>
        protected override string Title => "Settings";
    }
}