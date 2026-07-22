using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.DocumentExchange.Services.Implementations;
using Pds.DocumentExchange.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pds.DocumentExchange.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class DocumentReferenceServiceTests
    {
        private readonly DocumentReferenceService _documentReferenceService = new DocumentReferenceService();

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void CreateDocumentReferenceFromString_WhenDocumentReferenceIsNullOrEmpty_ThrowsArgumentNullexception(string documentReference)
        {
            // Act
            Func<DocumentReference> func = () => _documentReferenceService.CreateDocumentReferenceFromString(documentReference);

            // Assert
            func.Should().Throw<ArgumentNullException>();
            func.Should().Throw<ArgumentNullException>().Which.Message.Contains("The document reference cannot be empty.");
        }

        [TestMethod]
        [DataRow("file-name.pdf", "[\"file-name.pdf\"]")]
        [DataRow("file-name.pdf|batch-id", "[\"file-name.pdf\",\"batch-id\"]")]
        [DataRow("file-name.pdf|batch-id|parent-batch-id|extra-part", "[\"file-name.pdf\",\"batch-id\",\"parent-batch-id\",\"extra-part\"]")]
        public void CreateDocumentReferenceFromString_WhenDocumentReferenceFormatIsIncorrect_ThrowsFormatException(string documentReference, string result)
        {
            // Act
            Func<DocumentReference> func = () => _documentReferenceService.CreateDocumentReferenceFromString(documentReference);

            // Assert
            func.Should().Throw<FormatException>();
            func.Should().Throw<FormatException>().Which.Message.Contains("The document reference format should be: {file-name}|{batch-id}|{parent-batch-id}. But the document reference format was: " + result);
        }

        [TestMethod]
        public void CreateDocumentReferenceFromString_WhenDocumentReferenceIsValid_ReturnsDocumentReference()
        {
            // Arrange
            var fileName = "file-name.pdf";
            var batchId = "batch-id";
            var parentBatchId = "parent-batch-id";

            var documentReferenceString = $"{fileName}|{batchId}|{parentBatchId}";

            var expectedDocumentReference = new DocumentReference
            {
                FileName = fileName,
                BatchIdentifier = batchId,
                ParentBatchIdentifier = parentBatchId
            };

            // Act
            var result = _documentReferenceService.CreateDocumentReferenceFromString(documentReferenceString);

            // Assert
            result.Should().BeEquivalentTo(expectedDocumentReference);
        }

        [TestMethod]
        public void CreateDocumentReferencesWithPreviousVersionsFromStrings_ForNullCollection_ThrowsArgumentNullexception()
        {
            // Act
            Action act = () => _documentReferenceService.CreateDocumentReferencesWithPreviousVersionsFromStrings(null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
            act.Should().Throw<ArgumentNullException>().Which.Message.Contains("The document reference relationships collection cannot be null.");
        }

        [TestMethod]
        public void CreateDocumentReferencesWithPreviousVersionsFromStrings_ForCollectionWithNullElements_ThrowsArgumentNullexception()
        {
            // Act
            Action act = () => _documentReferenceService.CreateDocumentReferencesWithPreviousVersionsFromStrings(new[] { (string)null });

            // Assert
            act.Should().Throw<ArgumentNullException>();
            act.Should().Throw<ArgumentNullException>().Which.Message.Contains("The document reference relationship collection cannot contain null entries.");
        }

        [TestMethod]
        [DataRow("", "[]")]
        [DataRow("::", "[]")]
        [DataRow("::::", "[]")]
        [DataRow("a|b|c::d|e|f::g|h|i", "[\"a|b|c\",\"d|e|f\",\"g|h|i\"]")]
        public void CreateDocumentReferencesWithPreviousVersionsFromStrings_WhenRelationshipFormatIsIncorrect_ThrowsFormatException(string relationship, string result)
        {
            // Arrange
            var relationshipArray = new[] { relationship };

            // Act
            Action act = () => _documentReferenceService.CreateDocumentReferencesWithPreviousVersionsFromStrings(relationshipArray);

            // Assert
            act.Should().Throw<FormatException>();
            act.Should().Throw<FormatException>().Which.Message.Contains("The document reference format should be: {file-name}|{batch-id}|{parent-batch-id}. But the document reference format was: " + result);
        }

        [TestMethod, DynamicData(nameof(CreateDocumentReferencesWithPreviousVersionsFromStrings_TestData))]
        public void CreateDocumentReferencesWithPreviousVersionsFromStrings_ForValidInputData_ReturnsExpectedResult(
            IEnumerable<string> relationships,
            IEnumerable<DocumentReferenceWithPreviousVersions> expected)
        {
            // Act
            var actual = _documentReferenceService.CreateDocumentReferencesWithPreviousVersionsFromStrings(relationships);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        private static IEnumerable<object[]> CreateDocumentReferencesWithPreviousVersionsFromStrings_TestData
        {
            get
            {
                yield return new object[]
                {
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<DocumentReferenceWithPreviousVersions>()
                };
                yield return new object[]
                {
                    new[] { "a|b|c" },
                    new[]
                    {
                        new DocumentReferenceWithPreviousVersions(
                            new DocumentReference
                            {
                                FileName = "a",
                                BatchIdentifier = "b",
                                ParentBatchIdentifier = "c"
                            })
                    }
                };
                yield return new object[]
                {
                    new[]
                    {
                        "a|b|c",
                        "a|b|c::d|e|f"
                    },
                    new[]
                    {
                        new DocumentReferenceWithPreviousVersions(
                            new DocumentReference
                            {
                                FileName = "a",
                                BatchIdentifier = "b",
                                ParentBatchIdentifier = "c"
                            })
                        {
                            PreviousVersions = new[]
                            {
                                new DocumentReference
                                {
                                    FileName = "d",
                                    BatchIdentifier = "e",
                                    ParentBatchIdentifier = "f"
                                }
                            }
                        }
                    }
                };
                yield return new object[]
                {
                    new[]
                    {
                        "a|b|c",
                        "a|b|c::d|e|f",
                        "a|b|c::g|h|i",
                        "j|k|l",
                        "j|k|l::m|n|o",
                        "j|k|l::p|q|r",
                    },
                    new[]
                    {
                        new DocumentReferenceWithPreviousVersions(
                            new DocumentReference
                            {
                                FileName = "a",
                                BatchIdentifier = "b",
                                ParentBatchIdentifier = "c"
                            })
                        {
                            PreviousVersions = new[]
                            {
                                new DocumentReference
                                {
                                    FileName = "d",
                                    BatchIdentifier = "e",
                                    ParentBatchIdentifier = "f"
                                },
                                new DocumentReference
                                {
                                    FileName = "g",
                                    BatchIdentifier = "h",
                                    ParentBatchIdentifier = "i"
                                }
                            }
                        },
                        new DocumentReferenceWithPreviousVersions(
                            new DocumentReference
                            {
                                FileName = "j",
                                BatchIdentifier = "k",
                                ParentBatchIdentifier = "l"
                            })
                        {
                            PreviousVersions = new[]
                            {
                                new DocumentReference
                                {
                                    FileName = "m",
                                    BatchIdentifier = "n",
                                    ParentBatchIdentifier = "o"
                                },
                                new DocumentReference
                                {
                                    FileName = "p",
                                    BatchIdentifier = "q",
                                    ParentBatchIdentifier = "r"
                                }
                            }
                        }
                    }
                };
            }
        }
    }
}