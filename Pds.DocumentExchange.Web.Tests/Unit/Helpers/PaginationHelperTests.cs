using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.DocumentExchange.Web.Helpers;
using System.Linq;

namespace Pds.DocumentExchange.Web.Tests.Unit.Helpers
{
    [TestClass]
    public class PaginationHelperTests
    {
        [TestMethod, TestCategory("Unit")]
        [DataRow(1, 1)]
        [DataRow(2, 1)]
        [DataRow(1, 2)]
        [DataRow(10, 5)]
        [DataRow(100, 99)]
        [DataRow(100, 100)]
        [DataRow(100, 101)]
        public void Paginate_ReturnsExpectedPages(int numberOfItems, int pageSize)
        {
            // Arrange
            var items = Enumerable
                .Range(0, numberOfItems)
                .Select(i => (char)('a' + i));

            // Act
            var actualPages = items.Paginate(pageSize);

            // Assert
            actualPages.First().First().Should().Be('a');

            if (numberOfItems > pageSize)
            {
                actualPages.Should().HaveCountGreaterThan(1);
                actualPages.First().Last().Should().Be((char)('a' + pageSize - 1));
                actualPages.Skip(1).First().First().Should().Be((char)('a' + pageSize));
            }
            else
            {
                actualPages.Should().HaveCount(1);
                actualPages.First().Last().Should().Be((char)('a' + numberOfItems - 1));
            }

            actualPages.Last().Last().Should().Be((char)('a' + numberOfItems - 1));
        }
    }
}