using Pds.DocumentExchange.Web.Helpers;
using System.Linq;

namespace Pds.DocumentExchange.Web.Models.Agency
{
    /// <summary>
    /// View model for the "you've deleted N document(s)" page.
    /// </summary>
    public class DeleteDocumentsConfirmation : DeleteDocuments
    {
        /// <inheritdoc/>
        // The title will be explicitly rendered in the view so we will not show the one in the global layout.
        public override bool ShowContentTitle => false;

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => true;

        /// <inheritdoc/>
        protected override string Title
            => ContentHelper.GetDocumentCountMessage(
                ListItems?.Count() ?? 0,
                "You've deleted {count} {document(s)}");
    }
}