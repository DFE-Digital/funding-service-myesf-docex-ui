using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.DTOs
{
    /// <inheritdoc cref="IFileShareDocumentReferenceList"/>
    public class FileShareDocumentReferenceList : IFileShareDocumentReferenceList
    {
        /// <inheritdoc/>
        public IEnumerable<string> FileShareDocumentReferences { get; set; }
    }
}