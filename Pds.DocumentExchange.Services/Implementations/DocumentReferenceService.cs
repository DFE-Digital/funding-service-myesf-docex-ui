using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Pds.DocumentExchange.Services.Implementations
{
    /// <summary>
    /// The document reference service.
    /// </summary>
    public class DocumentReferenceService : IDocumentReferenceService
    {
        private const int NumberOfDocumentReferenceParts = 3;

        /// <inheritdoc/>
        public DocumentReference CreateDocumentReferenceFromString(string documentReference)
        {
            if (string.IsNullOrWhiteSpace(documentReference))
            {
                throw new ArgumentNullException(nameof(documentReference), "The document reference cannot be empty.");
            }

            var documentReferenceParts = documentReference.Split('|');

            if (documentReferenceParts.Length != NumberOfDocumentReferenceParts)
            {
                throw new FormatException("The document reference format should be: {file-name}|{batch-id}|{parent-batch-id}. But the document reference format was: " + JsonSerializer.Serialize(documentReferenceParts));
            }

            return new DocumentReference
            {
                FileName = documentReferenceParts[(int)DocumentReferenceParts.FileName],
                BatchIdentifier = documentReferenceParts[(int)DocumentReferenceParts.BatchIdentifier],
                ParentBatchIdentifier = documentReferenceParts[(int)DocumentReferenceParts.ParentBatchIdentifier]
            };
        }

        /// <inheritdoc/>
        public IEnumerable<DocumentReferenceWithPreviousVersions> CreateDocumentReferencesWithPreviousVersionsFromStrings(
            IEnumerable<string> documentReferenceRelationships)
        {
            if (documentReferenceRelationships == null)
            {
                throw new ArgumentNullException(nameof(documentReferenceRelationships), "The document reference relationships collection cannot be null.");
            }

            var resultDict = new Dictionary<string, DocumentReferenceWithPreviousVersions>();

            foreach (var documentReferenceRelationship in documentReferenceRelationships)
            {
                if (documentReferenceRelationship == null)
                {
                    throw new ArgumentNullException(nameof(documentReferenceRelationships), "The document reference relationship collection cannot contain null entries.");
                }

                var relationshipParts = documentReferenceRelationship.Split("::", StringSplitOptions.RemoveEmptyEntries);
                if (relationshipParts.Length > 2 || relationshipParts.Length == 0)
                {
                    throw new FormatException("The document reference relationship format should be: {documentReference}::{previousVersionDocumentReference}, or a single document reference. But the document reference relationship format was : " + JsonSerializer.Serialize(relationshipParts));
                }

                var documentReferenceString = relationshipParts[0];
                DocumentReferenceWithPreviousVersions documentReference;
                if (resultDict.ContainsKey(documentReferenceString))
                {
                    documentReference = resultDict[documentReferenceString];
                }
                else
                {
                    var sourceDocumentReference = CreateDocumentReferenceFromString(documentReferenceString);
                    documentReference = new DocumentReferenceWithPreviousVersions(sourceDocumentReference);
                    resultDict.Add(documentReferenceString, documentReference);
                }

                if (relationshipParts.Length == 2)
                {
                    var previousVersionReference = CreateDocumentReferenceFromString(relationshipParts[1]);
                    documentReference.PreviousVersions = documentReference.PreviousVersions.Append(previousVersionReference);
                }
            }

            return resultDict.Values;
        }
    }
}