using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Models.Organisation
{
    /// <summary>
    /// View model for the 'select a document' page.
    /// </summary>
    public class SelectDocument : SendYourDocument
    {
        /// <summary>
        /// Gets or sets the product identifier of the document being uploaded.
        /// </summary>
        public int ProductIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the product name of the document being uploaded.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the next version number for the product being uploaded.
        /// </summary>
        public int NextVersionNumber { get; set; }

        /// <summary>
        /// Gets or sets the list of allowed file extensions for the upload.
        /// </summary>
        public List<string> AllowedFileExtensions { get; set; }

        /// <summary>
        /// Gets or sets the max file size that can be uploaded in bytes.
        /// </summary>
        public int MaxFileUploadSize { get; set; }

        /// <summary>
        /// Gets or sets the document validation errors.
        /// </summary>
        public List<string> DocumentValidationErrors { get; set; }

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => true;

        /// <inheritdoc/>
        public override bool ShowContentTitle => false;

        /// <inheritdoc/>
        protected override string Title
            => "Send your document";
    }
}