namespace Pds.DocumentExchange.Web.DTOs
{
    /// <summary>
    /// Structure to hold data for the agency landing page (agency home).
    /// </summary>
    public class AgencyHomePageData
    {
        /// <summary>
        /// Gets or sets a value indicating whether the user should see the options
        /// for the file share and download documents.
        /// </summary>
        public bool ShowDocumentOptions { get; set; }

        /// <summary>
        /// Gets or sets the total count of documents in the file share.
        /// </summary>
        public int TotalCountOfDocumentsInFileShare { get; set; }

        /// <summary>
        /// Gets or sets the count of new documents to download.
        /// </summary>
        public int CountOfNewDocuments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user should see the 'View as organisation' option.
        /// </summary>
        public bool ShowViewAsOrganisation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user should see the settings option.
        /// </summary>
        public bool ShowSettingsOption { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user should see the tools option.
        /// </summary>
        public bool ShowToolsOption { get; set; }
    }
}