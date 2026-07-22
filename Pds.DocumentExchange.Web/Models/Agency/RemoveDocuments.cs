using Pds.DocumentExchange.Web.DTOs;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Models.Agency
{
    /// <summary>
    /// Shared view model for pages within the remove documents journey.
    /// </summary>
    public class RemoveDocuments : BaseAgencyPageViewModel, IFileShareDocumentReferenceList
    {
        /// <inheritdoc/>
        public IEnumerable<string> FileShareDocumentReferences { get; set; }

        /// <summary>
        /// Gets or sets the collection of file names of the documents.
        /// </summary>
        public IEnumerable<string> FileNames { get; set; }

        /// <summary>
        /// Gets or sets the team.
        /// </summary>
        public string Team { get; set; }

        /// <summary>
        /// Gets or sets the entry point from which this journey has been accessed (i.e. the review or publish page).
        /// </summary>
        public string EntryAction { get; set; }
    }
}