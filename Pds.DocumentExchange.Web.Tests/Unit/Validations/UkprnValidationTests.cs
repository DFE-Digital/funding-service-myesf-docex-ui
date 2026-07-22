using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.DocumentExchange.Web.Validations;

namespace Pds.DocumentExchange.Web.Tests.Unit.Validations
{
    [TestClass, TestCategory("Unit")]
    public class UkprnValidationTests
    {
        [TestMethod]
        [DataRow(null, false)]
        [DataRow("", false)]
        [DataRow(" ", false)]
        [DataRow("   ", false)]
        [DataRow("1", false)]
        [DataRow("1234567", false)]
        [DataRow("123456789", false)]
        [DataRow("1234567890", false)]
        [DataRow("12345678", true)]
        public void IsValidUkprn_ReturnsExpected(string ukprn, bool expected)
        {
            // Act
            bool result = UkprnValidation.IsValidUkprn(ukprn);

            // Assert
            result.Should().Be(expected);
        }
    }
}