using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.Models.Shared;
using System;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Models.Agency
{
    /// <summary>
    /// View model representing an exchanged document accessed by the agency.
    /// </summary>
    public class AgencyExchangeDocument : ExchangeDocument
    {
        /// <inheritdoc/>
        public override string DownloadActionLink => nameof(AgencyController.DownloadExchangeDocument);

        /// <summary>
        /// Gets or sets the collection of previous versions of this document.
        /// </summary>
        public IEnumerable<AgencyExchangeDocument> PreviousVersions { get; set; }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the provider UKPRN.
        /// </summary>
        public string ProviderUkprn { get; set; }

        /// <summary>
        /// Gets or sets a csv string representing the list of versions of this document with hyperlinks.
        /// </summary>
        public string Versions { get; set; }

        /// <summary>
        /// Gets or sets a csv string representing the list of versions of this document without hyperlinks.
        /// </summary>
        public string VersionsWithoutHyperlinks { get; set; }

        /// <summary>
        /// Gets or sets a csv string representing the version of this document.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets a csv string representing the version of this document with hyperlink.
        /// </summary>
        public string VersionWithHyperlink { get; set; }

        /// <summary>
        /// Gets or sets the version of this document.
        /// </summary>
        public int VersionNumber { get; set; }

        /// <summary>
        /// Gets or sets the uploaded date and time.
        /// </summary>
        public DateTime UploadedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the uploaded date and time formatted for display purposes.
        /// </summary>
        public string DisplayUploadedDateTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the document has been deleted.
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }
}