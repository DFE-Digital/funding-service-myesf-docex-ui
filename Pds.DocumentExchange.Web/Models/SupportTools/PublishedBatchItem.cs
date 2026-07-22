using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.DocumentExchange.Web.Controllers;
using System;

namespace Pds.DocumentExchange.Web.Models.SupportTools
{
    /// <summary>
    /// Class representing a published batch of documents.
    /// </summary>
    public class PublishedBatchItem : BaseListItem
    {
        /// <summary>
        /// Gets or sets the parent batch identifier.
        /// </summary>
        public Guid ParentBatchIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user who uploaded the document.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the number of documents.
        /// </summary>
        public int NumberOfDocuments { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the document was uploaded.
        /// </summary>
        public DateTime DateAndTime { get; set; }

        /// <summary>
        /// Gets or sets the number of emails.
        /// </summary>
        public int NumberOfEmails { get; set; }

        /// <summary>
        /// Gets the documents CSV download link.
        /// </summary>
        public string DownloadDocumentsCsvActionLink
            => nameof(SupportToolsController.DownloadDocumentsCsv);

        /// <summary>
        /// Gets the emails CSV download link.
        /// </summary>
        public string DownloadEmailsCsvActionLink
            => nameof(SupportToolsController.DownloadEmailsCsv);
    }
}