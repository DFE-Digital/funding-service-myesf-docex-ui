using System.Collections.Generic;
using System.Linq;

namespace Pds.DocumentExchange.Services.Models
{
    /// <summary>
    /// The reference information of a document.
    /// </summary>
    public class DocumentReferenceWithPreviousVersions : DocumentReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentReferenceWithPreviousVersions"/> class
        /// using an existing <see cref="DocumentReference"/> as a source.
        /// </summary>
        /// <param name="sourceDocumentReference">The source document reference.</param>
        public DocumentReferenceWithPreviousVersions(DocumentReference sourceDocumentReference)
        {
            ParentBatchIdentifier = sourceDocumentReference.ParentBatchIdentifier;
            BatchIdentifier = sourceDocumentReference.BatchIdentifier;
            FileName = sourceDocumentReference.FileName;
        }

        /// <summary>
        /// Gets or sets the collection of document references for previous versions of this document.
        /// </summary>
        public IEnumerable<DocumentReference> PreviousVersions { get; set; } = Enumerable.Empty<DocumentReference>();
    }
}