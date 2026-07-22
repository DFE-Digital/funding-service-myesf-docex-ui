using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Web.Helpers;
using System;

namespace Pds.DocumentExchange.Web.Models.Shared
{
    /// <summary>
    /// View model representing an exchanged document.
    /// </summary>
    public abstract class ExchangeDocument : BaseListItem
    {
        /// <summary>
        /// Gets or sets a value indicating whether the document is selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the batch identifier.
        /// </summary>
        public string BatchIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the parent batch identifier.
        /// </summary>
        public string ParentBatchIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the name of the product that this document belongs to.
        /// For example, "Data and MI report".
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the document status.
        /// </summary>
        public DownloadDocumentStatus Status { get; set; }

        /// <summary>
        /// Gets or sets when the document status was set.
        /// </summary>
        public DateTime? StatusDateTime { get; set; }

        /// <summary>
        /// Gets or sets who has downloaded the document (if applicable).
        /// </summary>
        public string DownloadedBy { get; set; }

        /// <summary>
        /// Gets the name of the action link to be used to download the document.
        /// </summary>
        public abstract string DownloadActionLink { get; }

        /// <summary>
        /// Gets the markup for displaying the file name.
        /// </summary>
        public string DisplayFileName
            => ContentHelper.InsertBreaksIntoFileName(FileName);

        /// <summary>
        /// Gets the file extension note to display.
        /// </summary>
        public string DisplayFileExtensionNote
            => ContentHelper.GetFileExtensionNote(FileName);

        /// <summary>
        /// Gets or sets the markup for displaying the status date/time.
        /// </summary>
        public string DisplayStatusDateTime { get; set; }

        /// <summary>
        /// Gets or sets who has published the document (if applicable).
        /// </summary>
        public string PublishedBy { get; set; }

        /// <summary>
        /// Gets or sets the published date and time.
        /// </summary>
        public DateTime PublishedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the published date and time formatted for display purposes.
        /// </summary>
        public string DisplayPublishedDateTime { get; set; }

        /// <summary>
        /// Gets the document's ID.
        /// </summary>
        public string Id
            => $"{FileName}|{BatchIdentifier}|{ParentBatchIdentifier}";

        /// <summary>
        /// Gets a value indicating whether the document is new.
        /// </summary>
        public bool IsNew
            => Status == DownloadDocumentStatus.New;
    }
}