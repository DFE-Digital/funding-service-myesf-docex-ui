using Pds.DocumentExchange.Web.Helpers;
using System.Linq;

namespace Pds.DocumentExchange.Web.Models.Agency
{
    /// <summary>
    /// View model for the "are you sure you want to delete these documents?" page.
    /// </summary>
    public class DeleteDocumentsAreYouSure : DeleteDocuments
    {
        /// <summary>
        /// Gets the selection count message.
        /// </summary>
        public string SelectionCountMessage
            => ContentHelper.GetDocumentCountMessage(
                ListItems?.Count() ?? 0,
                "You have selected <strong class=\"bold-small\">{count}</strong> {document(s)} for deletion.");

        /// <inheritdoc/>
        // The title will be explicitly rendered in the view so we will not show the one in the global layout.
        public override bool ShowContentTitle => false;

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => true;

        /// <inheritdoc/>
        protected override string Title
            => ContentHelper.GetDocumentCountMessage(
                ListItems?.Count() ?? 0,
                "Are you sure you want to delete {this/these} {document(s)}?");
    }
}