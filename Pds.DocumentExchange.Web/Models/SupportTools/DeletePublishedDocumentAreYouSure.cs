using Pds.DocumentExchange.Web.Models.Agency;

namespace Pds.DocumentExchange.Web.Models.SupportTools
{
    /// <summary>
    /// View model for the "are you sure you want to delete these documents?" page.
    /// </summary>
    public class DeletePublishedDocumentAreYouSure : DeleteDocuments
    {
        /// <inheritdoc/>
        // The title will be explicitly rendered in the view so we will not show the one in the global layout.
        public override bool ShowContentTitle => false;

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => true;

        /// <summary>
        /// Gets or sets the selected document for delete.
        /// </summary>
        public string SelectedDocument { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the page has a validation error.
        /// </summary>
        public bool Error { get; set; }

        /// <summary>
        /// Gets or sets the action that caused the validation error.
        /// </summary>
        public string ErrorAction { get; set; }

        /// <inheritdoc/>
        protected override string Title
            => "Delete a published document";
    }
}