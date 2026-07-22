using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.DocumentExchange.Web.Helpers;

namespace Pds.DocumentExchange.Web.Tests.Unit.Helpers
{
    [TestClass]
    public class ContentHelperTests
    {
        [TestMethod, TestCategory("Unit")]

        // Using {count}:
        [DataRow(0, "the count of documents is {count}", "the count of documents is 0")]
        [DataRow(1, "the count of documents is {count}", "the count of documents is 1")]
        [DataRow(2, "the count of documents is {count}", "the count of documents is 2")]
        [DataRow(3, "the count of documents is {count}", "the count of documents is 3")]
        [DataRow(12345678, "the count of documents is {count}", "the count of documents is 12345678")]

        // Using {count} and {document(s)}:
        [DataRow(0, "you have got {count} {document(s)}", "you have got 0 documents")]
        [DataRow(1, "you have got {count} {document(s)}", "you have got 1 document")]
        [DataRow(2, "you have got {count} {document(s)}", "you have got 2 documents")]
        [DataRow(3, "you have got {count} {document(s)}", "you have got 3 documents")]
        [DataRow(12345678, "you have got {count} {document(s)}", "you have got 12345678 documents")]

        // Using {is/are}, {count} and {document(s)}:
        [DataRow(0, "there {is/are} {count} new {document(s)}", "there are 0 new documents")]
        [DataRow(1, "there {is/are} {count} new {document(s)}", "there is 1 new document")]
        [DataRow(2, "there {is/are} {count} new {document(s)}", "there are 2 new documents")]
        [DataRow(3, "there {is/are} {count} new {document(s)}", "there are 3 new documents")]
        [DataRow(12345678, "there {is/are} {count} new {document(s)}", "there are 12345678 new documents")]

        // Using {this/these}, {count} and {document(s)}:
        [DataRow(0, "you have {this/these} {count} new {document(s)}", "you have these 0 new documents")]
        [DataRow(1, "you have {this/these} {count} new {document(s)}", "you have this 1 new document")]
        [DataRow(2, "you have {this/these} {count} new {document(s)}", "you have these 2 new documents")]
        [DataRow(3, "you have {this/these} {count} new {document(s)}", "you have these 3 new documents")]
        [DataRow(12345678, "you have {this/these} {count} new {document(s)}", "you have these 12345678 new documents")]
        public void GetDocumentCountMessage_ReturnsExpectedMessage(int count, string messageFormat, string expectedMessage)
        {
            // Arrange / Act
            var actualMessage = ContentHelper.GetDocumentCountMessage(count, messageFormat);

            // Assert
            actualMessage.Should().Be(expectedMessage);
        }

        // Using {versions}:
        [TestMethod, TestCategory("Unit")]
        [DataRow("0", false, "You've deleted {versions} of the document", "You've deleted version 0 of the document")]
        [DataRow("1", false, "You've deleted {versions} of the document", "You've deleted version 1 of the document")]
        [DataRow("2", false, "You've deleted {versions} of the document", "You've deleted version 2 of the document")]
        [DataRow("3", false, "You've deleted {versions} of the document", "You've deleted version 3 of the document")]
        [DataRow("1, 2, 3", true, "You've deleted {versions} of the document", "You've deleted all versions of the document")]
        public void GetDocumentVersionMessage_ReturnsExpectedMessage(string version, bool isAllVersions, string messageFormat, string expectedMessage)
        {
            // Arrange / Act
            var actualMessage = ContentHelper.GetDocumentVersionMessage(isAllVersions, version, messageFormat);

            // Assert
            actualMessage.Should().Be(expectedMessage);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(null, "")]
        [DataRow(".doc", "(DOC)")]
        [DataRow(".dOC", "(DOC)")]
        [DataRow(".Doc", "(DOC)")]
        [DataRow("file.doc", "(DOC)")]
        [DataRow("file.with.many.dots.dOC", "(DOC)")]
        [DataRow("cheese.Doc", "(DOC)")]
        [DataRow(".pdf", "(PDF)")]
        [DataRow("file.pdf", "(PDF)")]
        [DataRow("file.with.many.dots.pDf", "(PDF)")]
        [DataRow("cheese.Pdf", "(PDF)")]
        [DataRow("file", "")]
        [DataRow("file.with.many.dots", "(DOTS)")]
        [DataRow("cheese", "")]
        [DataRow("file.", "")]
        [DataRow("file.with.many.dots.", "")]
        [DataRow("cheese.", "")]
        public void GetFileExtensionNote_ReturnsExpectedNote(string fileName, string expectedNote)
        {
            // Arrange / Act
            var actualNote = ContentHelper.GetFileExtensionNote(fileName);

            // Assert
            actualNote.Should().Be(expectedNote);
        }
    }
}