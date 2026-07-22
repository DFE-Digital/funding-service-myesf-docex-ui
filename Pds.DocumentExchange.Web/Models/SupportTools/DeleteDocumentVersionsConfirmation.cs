using Pds.DocumentExchange.Web.Helpers;
using Pds.DocumentExchange.Web.Models.Agency;

namespace Pds.DocumentExchange.Web.Models.SupportTools
{
    /// <summary>
    /// View model for the "you've deleted N document(s)" page.
    /// </summary>
    public class DeleteDocumentVersionsConfirmation : BaseAgencyPageViewModel
    {
        /// <inheritdoc/>
        // The title will be explicitly rendered in the view so we will not show the one in the global layout.
        public override bool ShowContentTitle => false;

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => false;

        /// <summary>
        /// Gets or sets the product name.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the provider UKPRN.
        /// </summary>
        public string ProviderUkprn { get; set; }

        /// <summary>
        /// Gets or sets the document Version(s).
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the value true or false be it all versions or not.
        /// </summary>
        public bool IsAllVersions { get; set; }

        /// <inheritdoc/>
        protected override string Title
            => ContentHelper.GetDocumentVersionMessage(
                IsAllVersions,
                Version,
                "You've deleted {versions} of the document");
    }
}