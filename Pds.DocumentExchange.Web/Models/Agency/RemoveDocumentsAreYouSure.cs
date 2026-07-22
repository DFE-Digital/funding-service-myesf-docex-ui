using Pds.DocumentExchange.Web.Helpers;
using System.Linq;

namespace Pds.DocumentExchange.Web.Models.Agency
{
    /// <summary>
    /// View model for the 'are you sure' page within the remove documents journey.
    /// </summary>
    public class RemoveDocumentsAreYouSure : RemoveDocuments
    {
        /// <summary>
        /// Gets or sets a value indicating whether the page has a validation error.
        /// </summary>
        public bool Error { get; set; }

        /// <inheritdoc/>
        // The title will be explicitly rendered in the view so we will not show the one in the global layout.
        public override bool ShowContentTitle => false;

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => true;

        /// <inheritdoc/>
        protected override string Title
            => ContentHelper.GetDocumentCountMessage(
                FileShareDocumentReferences?.Count() ?? 0,
                "Are you sure you want to remove {this/these} {document(s)}?");
    }
}