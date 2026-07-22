using Pds.DocumentExchange.Services.Enums;
using System;

namespace Pds.DocumentExchange.Services.Models
{
    /// <summary>
    /// Class representing a document's status.
    /// </summary>
    public class DocumentStatusInfo
    {
        /// <summary>
        /// Gets or sets the download status of the document. Typically New or Downloaded.
        /// </summary>
        public DownloadDocumentStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the time stamp associated with the download action.
        /// </summary>
        public DateTime DownloadedTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the user who downloaded the document.
        /// </summary>
        public string DownloadedBy { get; set; }
    }
}
