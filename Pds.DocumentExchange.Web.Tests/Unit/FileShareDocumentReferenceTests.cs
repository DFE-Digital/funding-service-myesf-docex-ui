using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.DocumentExchange.Web.Models.Agency;

namespace Pds.DocumentExchange.Web.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public sealed class FileShareDocumentReferenceTests
    {
        [TestMethod]
        [DataRow((string)null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("  ")]
        [DataRow("::")]
        [DataRow("a::")]
        [DataRow("::a")]
        [DataRow("a::a::a")]
        public void TryParse_ForInvalidInputs_ReturnsExpected(string input)
        {
            // Act
            var actual = FileShareDocumentReference.TryParse(input, out var documentReference);

            // Assert
            actual.Should().BeFalse();
            documentReference.Should().BeNull();
        }

        [TestMethod]
        [DataRow("team::fileName", "team", "fileName")]
        [DataRow("team2::fileName2", "team2", "fileName2")]
        public void TryParse_ForValidInputs_ReturnsExpected(string input, string expectedTeam, string expectedFileName)
        {
            // Act
            var actual = FileShareDocumentReference.TryParse(input, out var documentReference);

            // Assert
            actual.Should().BeTrue();
            documentReference.Should().BeEquivalentTo(new FileShareDocumentReference { Team = expectedTeam, FileName = expectedFileName });
        }

        [TestMethod]
        [DataRow("team", "fileName", "team::fileName")]
        [DataRow("team2", "fileName2", "team2::fileName2")]
        public void ToString_ReturnsExpected(string team, string fileName, string expected)
        {
            // Arrange
            var documentReference = new FileShareDocumentReference { Team = team, FileName = fileName };

            // Act
            var actual = documentReference.ToString();

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        [DataRow("team", "fileName")]
        [DataRow("team2", "fileName2")]
        public void ToString_ThenTryParse_Succeeds(string team, string fileName)
        {
            // Arrange
            var documentReference = new FileShareDocumentReference { Team = team, FileName = fileName };

            // Act
            var toStringResult = documentReference.ToString();
            var tryParseResult = FileShareDocumentReference.TryParse(toStringResult, out var parsedDocumentReference);

            // Assert
            tryParseResult.Should().BeTrue();
            parsedDocumentReference.Should().BeEquivalentTo(documentReference);
        }
    }
}