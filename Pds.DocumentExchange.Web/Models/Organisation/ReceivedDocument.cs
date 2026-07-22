using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.Models.Shared;
using System;

namespace Pds.DocumentExchange.Web.Models.Organisation
{
    /// <summary>
    /// View model representing a document that has been received.
    /// </summary>
    public class ReceivedDocument : ExchangeDocument
    {
        /// <summary>
        /// Gets or sets the name of the user who uploaded this document.
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// Gets or sets the date and time at which this document was received.
        /// </summary>
        public DateTime ReceivedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the product version.
        /// </summary>
        public int Version { get; set; }

        /// <inheritdoc/>
        public override string DownloadActionLink
            => nameof(OrganisationController.DownloadExternalExchangeDocument);

        /// <summary>
        /// Gets or sets the name of the organisation receiving this document.
        /// </summary>
        public string RecipientOrganisationName { get; set; }

        /// <summary>
        /// Gets or sets the markup for displaying the received date/time.
        /// </summary>
        public string DisplayReceivedDateTime { get; set; }
    }
}