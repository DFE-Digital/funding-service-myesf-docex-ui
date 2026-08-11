using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.DocumentExchange.Web.Extensions;
using ServicesProduct = Pds.DocumentExchange.Services.Models.Product;
using WebProduct = Pds.DocumentExchange.Web.Models.Shared.Product;

namespace Pds.DocumentExchange.Web.Tests.Unit.Extensions
{
    [TestClass]
    [TestCategory("Unit")]
    public class ProductMappingTests
    {
        [TestMethod]
        public void ToWebProduct_ReturnsWebProduct()
        {
            // Arrange
            var sourceProduct = new ServicesProduct
            {
                Identifier = 1001,
                Name = "product 1",
                PluralName = "product 1's"
            };

            var expectedDestinationWebProduct = new WebProduct
            {
                Identifier = sourceProduct.Identifier,
                Name = sourceProduct.Name,
                PluralName = sourceProduct.PluralName
            };

            // Act
            var result = sourceProduct.ToWebProduct();

            // Assert
            result.Should().BeEquivalentTo(expectedDestinationWebProduct);
        }
    }
}