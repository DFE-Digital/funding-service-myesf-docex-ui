using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.DocumentExchange.Web.Helpers;

namespace Pds.DocumentExchange.Web.Models.Agency
{
    /// <summary>
    /// View model representing a document stored in an agency file share.
    /// </summary>
    public class FileShareDocument : BaseListItem
    {
        /// <summary>
        /// Gets or sets the reference to this document.
        /// </summary>
        public FileShareDocumentReference DocumentReference { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the document is selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Gets or sets the name of the product that this document belongs to.
        /// For example, "Data and MI report".
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets the markup for displaying the file name.
        /// </summary>
        public string DisplayFileName
            => ContentHelper.InsertBreaksIntoFileName(DocumentReference.FileName);

        /// <summary>
        /// Gets the file extension note to display.
        /// </summary>
        public string DisplayFileExtensionNote
            => ContentHelper.GetFileExtensionNote(DocumentReference.FileName);

        /// <summary>
        /// Gets the document's ID.
        /// </summary>
        public string Id
            => DocumentReference.ToString();
    }
}