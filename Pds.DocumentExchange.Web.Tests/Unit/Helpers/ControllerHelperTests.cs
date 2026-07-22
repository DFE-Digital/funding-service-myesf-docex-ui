using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Tests.Unit.Helpers
{
    [TestClass]
    public class ControllerHelperTests
    {
        [TestMethod, TestCategory("Unit")]
        public void NameOf_TestController_ReturnsExpectedValue()
        {
            // Arrange Act
            var actual = NameOf<TestController>();

            // Assert
            actual.Should().Be("Test");
        }

        [TestMethod, TestCategory("Unit")]
        public void NameOf_ControllerController_ReturnsExpectedValue()
        {
            // Arrange Act
            var actual = NameOf<ControllerController>();

            // Assert
            actual.Should().Be("Controller");
        }

        [TestMethod, TestCategory("Unit")]
        public void NameOf_ControllerTestController_ReturnsExpectedValue()
        {
            // Arrange Act
            var actual = NameOf<ControllerTestController>();

            // Assert
            actual.Should().Be("ControllerTest");
        }

        private class ControllerController : Controller
        {
        }

        private class TestController : Controller
        {
        }

        private class ControllerTestController : Controller
        {
        }
    }
}