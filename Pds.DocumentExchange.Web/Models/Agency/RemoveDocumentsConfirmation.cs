using Pds.DocumentExchange.Web.Helpers;
using System.Linq;

namespace Pds.DocumentExchange.Web.Models.Agency
{
    /// <summary>
    /// View model for the confirmation page within the remove documents journey.
    /// </summary>
    public class RemoveDocumentsConfirmation : RemoveDocuments
    {
        /// <summary>
        /// Gets or sets a value indicating whether the user confirmed that the documents should be removed.
        /// </summary>
        public bool? RemovalConfirmed { get; set; }

        /// <inheritdoc/>
        // The title will be explicitly rendered in the view so we will not show the one in the global layout.
        public override bool ShowContentTitle => false;

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => false;

        /// <inheritdoc/>
        protected override string Title
            => ContentHelper.GetDocumentCountMessage(
                FileShareDocumentReferences?.Count() ?? 0,
                "You've removed {count} {document(s)}");
    }
}