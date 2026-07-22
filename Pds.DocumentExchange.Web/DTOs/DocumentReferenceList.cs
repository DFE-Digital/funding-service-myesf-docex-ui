using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.DTOs
{
    /// <summary>
    /// DTO for passing a list of document references.
    /// </summary>
    /// <remarks>A document reference is a string containing the file name, batch identifier
    /// and parent batch identifier separated by vertical bar characters.</remarks>
    public class DocumentReferenceList : IDocumentReferenceList
    {
        /// <inheritdoc/>
        public IEnumerable<string> DocumentReferences { get; set; }
    }
}