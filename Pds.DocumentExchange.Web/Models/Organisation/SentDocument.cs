using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.Models.Shared;
using System;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Models.Organisation
{
    /// <summary>
    /// View model representing a document that has been sent.
    /// </summary>
    public class SentDocument : ExchangeDocument
    {
        /// <summary>
        /// Gets or sets the name of the user who uploaded this document.
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// Gets or sets the date and time at which this document was sent.
        /// </summary>
        public DateTime SentDateTime { get; set; }

        /// <summary>
        /// Gets or sets the organisation name on behalf of which this document was sent.
        /// </summary>
        public string SenderOrganisationName { get; set; }

        /// <inheritdoc/>
        public override string DownloadActionLink => nameof(OrganisationController.DownloadExternalExchangeDocument);

        /// <summary>
        /// Gets or sets the product version.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the markup for displaying the sent date/time.
        /// </summary>
        public string DisplaySentDateTime { get; set; }

        /// <summary>
        /// Gets or sets the collection of previous versions of the document.
        /// </summary>
        public IEnumerable<SentDocument> PreviousVersions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it is the oldest version of the document.
        /// </summary>
        public bool IsOldestVersion { get; set; }
    }
}