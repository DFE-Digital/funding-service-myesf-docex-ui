using Pds.DocumentExchange.Services.Models;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Services.Interfaces
{
    /// <summary>
    /// Interface providing methods to create document references.
    /// </summary>
    public interface IDocumentReferenceService
    {
        /// <summary>
        /// Creates a document reference object from a string.
        /// </summary>
        /// <param name="documentReference">The document reference as a string.</param>
        /// <returns>A document reference object.</returns>
        DocumentReference CreateDocumentReferenceFromString(string documentReference);

        /// <summary>
        /// Creates a collection of document references with previous versions from a collection of strings.
        /// </summary>
        /// <param name="documentReferenceRelationships">A collection of strings representing related document references.</param>
        /// <returns>A document reference object.</returns>
        IEnumerable<DocumentReferenceWithPreviousVersions> CreateDocumentReferencesWithPreviousVersionsFromStrings(
            IEnumerable<string> documentReferenceRelationships);
    }
}