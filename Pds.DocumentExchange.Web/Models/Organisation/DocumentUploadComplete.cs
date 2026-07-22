namespace Pds.DocumentExchange.Web.Models.Organisation
{
    /// <summary>
    /// View model for the 'document upload complete' page.
    /// </summary>
    public class DocumentUploadComplete : SendYourDocument
    {
        /// <summary>
        /// Gets or sets the file name of the uploaded document.
        /// </summary>
        public string DocumentFileName { get; set; }

        /// <summary>
        /// Gets or sets the name of the organisation for which the document was uploaded.
        /// </summary>
        public string OrganisationName { get; set; }

        /// <summary>
        /// Gets or sets the name of the MVC action to which the user
        /// should be directed when they want to send another new document.
        /// </summary>
        public string SendNewDocumentActionName { get; set; }

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => false;

        /// <inheritdoc/>
        // Hide the content title in the layout as it is explicitly included in the view.
        public override bool ShowContentTitle => false;

        /// <inheritdoc/>
        protected override string Title
            => "You've sent a document" + (SelectAnOrganisationRequired
                                            ? $" for {OrganisationName}"
                                            : string.Empty);
    }
}