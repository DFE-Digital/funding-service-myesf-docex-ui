using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.DTOs
{
    /// <summary>
    /// DTO for passing a list of file share document references.
    /// </summary>
    public interface IFileShareDocumentReferenceList
    {
        /// <summary>
        /// Gets or sets the list of file names of the documents.
        /// </summary>
        IEnumerable<string> FileShareDocumentReferences { get; set; }
    }
}