using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.DTOs
{
    /// <summary>
    /// Interface to a DTO for passing a list of document references.
    /// </summary>
    /// <remarks>A document reference is a string containing the file name, batch identifier
    /// and parent batch identifier separated by vertical bar characters.</remarks>
    public interface IDocumentReferenceList
    {
        /// <summary>
        /// Gets or sets the list of document references.
        /// </summary>
        IEnumerable<string> DocumentReferences { get; set; }
    }
}