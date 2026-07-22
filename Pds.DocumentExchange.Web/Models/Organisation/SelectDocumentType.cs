using Pds.DocumentExchange.Web.Models.Shared;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Models.Organisation
{
    /// <summary>
    /// View model for the 'select a document type' page.
    /// </summary>
    public class SelectDocumentType : SendYourDocument
    {
        /// <summary>
        /// Gets or sets the collection of document exchange products that are allowed to be uploaded.
        /// </summary>
        public IEnumerable<Product> AllowedProducts { get; set; }

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => true;

        /// <inheritdoc/>
        // Hide the content title in the layout as it is explicitly included in the view.
        public override bool ShowContentTitle => false;

        /// <inheritdoc/>
        protected override string Title
            => "Which type of document do you want to send?";
    }
}