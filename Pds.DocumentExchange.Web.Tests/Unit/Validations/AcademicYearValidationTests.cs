using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.DocumentExchange.Web.Validations;

namespace Pds.DocumentExchange.Web.Tests.Unit.Validations
{
    [TestClass, TestCategory("Unit")]
    public class AcademicYearValidationTests
    {
        [TestMethod]
        [DataRow(null, false)]
        [DataRow("", false)]
        [DataRow(" ", false)]
        [DataRow("   ", false)]
        [DataRow("1", false)]
        [DataRow("1920", false)]
        [DataRow("201920", true)]
        [DataRow("2019200", false)]
        public void IsValidAcademicYear_ReturnsExpected(string academicYear, bool expected)
        {
            // Act
            bool result = AcademicYearValidation.IsValidAcademicYear(academicYear);

            // Assert
            result.Should().Be(expected);
        }
    }
}