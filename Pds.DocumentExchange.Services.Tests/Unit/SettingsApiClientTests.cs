using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.ApiClient.Exceptions;
using Pds.Core.Logging;
using Pds.DocumentExchange.Services.Configuration;
using Pds.DocumentExchange.Services.Implementations;
using Pds.DocumentExchange.Services.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Tests.Unit
{
    [TestClass]
    public class SettingsApiClientTests : BaseApiClientTests
    {
        private const string GetProductsThatOrganisationsCanUploadRoute
            = "api/settings/products-that-organisations-can-upload";

        private const string GetAllProductsRoute
            = "api/settings/products";

        private const string GetProductRouteByIdentifier
            = "api/settings/product/1";

        private const string AddOrUpdateProductRoute
            = "api/settings/products/1";

        private const string GetTeamsRoute
            = "api/settings/teams";

        private const string GetFileExtensionsRoute
            = "api/settings/file-extensions";

        private const string GetMaxFileUploadSizeRoute
            = "api/settings/max-file-upload-size";

        private readonly ILoggerAdapter<SettingsApiClient> _logger
            = Mock.Of<ILoggerAdapter<SettingsApiClient>>();

        [TestMethod, TestCategory("Unit")]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(10, 1)]
        [DataRow(100, 1)]
        [DataRow(1, 10)]
        [DataRow(10, 10)]
        [DataRow(100, 10)]
        [DataRow(1, 100)]
        [DataRow(10, 100)]
        [DataRow(100, 100)]
        public async Task GetProductsThatOrganisationsCanUpload_OnSuccess_ReturnsTheProducts(int numberOfProducts, int numberOfAgencies)
        {
            // Arrange
            var expectedProducts = numberOfProducts == 0
                ? Enumerable.Empty<Product>()
                : Enumerable.Range(1, numberOfProducts)
                    .Select(n => new Product
                    {
                        Identifier = n,
                        Name = $"product {n}",
                        CanOrganisationsUpload = true,
                        AgencyTeams = Enumerable.Range(1, numberOfAgencies)
                                        .Select(a => $"agency {a}")
                    });

            SetupMessageHandler(HttpStatusCode.OK, expectedProducts);

            var client = GetSettingsApiClient();

            // Act
            var result = await client.GetProductsThatOrganisationsCanUpload();

            // Assert
            result.Should().BeEquivalentTo(expectedProducts);
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + GetProductsThatOrganisationsCanUploadRoute);
        }

        [TestMethod, TestCategory("Unit")]
        public void GetProductsThatOrganisationsCanUpload_OnFailure_LogsTheException()
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.InternalServerError);

            var client = GetSettingsApiClient();

            // Act
            Func<Task> act = async () => await client.GetProductsThatOrganisationsCanUpload();

            // Assert
            act.Should().ThrowAsync<ApiGeneralException>();
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + GetProductsThatOrganisationsCanUploadRoute);
            Mock.Get(_logger)
                .Verify(l => l.LogError(It.IsAny<ApiGeneralException>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(10, 1)]
        [DataRow(100, 1)]
        [DataRow(1, 10)]
        [DataRow(10, 10)]
        [DataRow(100, 10)]
        [DataRow(1, 100)]
        [DataRow(10, 100)]
        [DataRow(100, 100)]
        public async Task GetAllProducts_OnSuccess_ReturnsTheProducts(int numberOfProducts, int numberOfAgencies)
        {
            // Arrange
            var expectedProducts = numberOfProducts == 0
                ? Enumerable.Empty<Product>()
                : Enumerable.Range(1, numberOfProducts)
                    .Select(n => new Product
                    {
                        Identifier = n,
                        Name = $"product {n}",
                        CanOrganisationsUpload = true,
                        AgencyTeams = Enumerable.Range(1, numberOfAgencies)
                                        .Select(a => $"agency {a}")
                    });

            SetupMessageHandler(HttpStatusCode.OK, expectedProducts);

            var client = GetSettingsApiClient();

            // Act
            var result = await client.GetAllProducts();

            // Assert
            result.Should().BeEquivalentTo(expectedProducts);
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + GetAllProductsRoute);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task GetProduct_OnSuccess_ReturnsTheProduct()
        {
            // Arrange
            var expectedProduct = new Product
            {
                Identifier = 1,
                Name = $"product1",
                CanOrganisationsUpload = true,
                AgencyTeams = new[] { "test" }
            };

            SetupMessageHandler(HttpStatusCode.OK, expectedProduct);

            var client = GetSettingsApiClient();

            // Act
            var result = await client.GetProduct(1);

            // Assert
            result.Should().BeEquivalentTo(expectedProduct);
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + GetProductRouteByIdentifier);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task AddOrUpdateProduct_OnSuccess_ReturnsTheProduct()
        {
            // Arrange
            var expectedProduct = new Product
            {
                Identifier = 1,
                Name = $"product1",
                CanOrganisationsUpload = true,
                AgencyTeams = new[] { "test" }
            };

            SetupMessageHandler(HttpStatusCode.OK, expectedProduct);

            var client = GetSettingsApiClient();

            // Act
            var result = await client.AddOrUpdateProduct(1, expectedProduct);

            // Assert
            result.Should().BeEquivalentTo(expectedProduct);
            VerifyMessageHandler(HttpMethod.Put, TestBaseAddress + AddOrUpdateProductRoute);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(100)]
        public async Task GetTeams_OnSuccess_ReturnsTheTeams(int numberOfTeams)
        {
            // Arrange
            var expectedTeams = numberOfTeams == 0
                ? Enumerable.Empty<AgencyTeam>()
                : Enumerable.Range(1, numberOfTeams)
                    .Select(n => new AgencyTeam
                    {
                        Identifier = $"identifier {n}",
                        Name = $"name {n}"
                    });

            SetupMessageHandler(HttpStatusCode.OK, expectedTeams);

            var client = GetSettingsApiClient();

            // Act
            var result = await client.GetTeams();

            // Assert
            result.Should().BeEquivalentTo(expectedTeams);
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + GetTeamsRoute);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(100)]
        public async Task GetFileExtensions_OnSuccess_ReturnsFileExtensions(int numberOfExtensions)
        {
            // Arrange
            var expectedExtensions = numberOfExtensions == 0
                ? Enumerable.Empty<FileExtensionInfo>()
                : Enumerable.Range(1, numberOfExtensions)
                    .Select(n => new FileExtensionInfo
                    {
                        Identifier = $"identifier {n}",
                        Extension = $"identifier {n}",
                        ProductIdentifiers = Enumerable.Range(1, 3)
                    });

            SetupMessageHandler(HttpStatusCode.OK, expectedExtensions);

            var client = GetSettingsApiClient();

            // Act
            var result = await client.GetFileExtensions();

            // Assert
            result.Should().BeEquivalentTo(expectedExtensions);
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + GetFileExtensionsRoute);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(0)]
        [DataRow(1000000)]
        [DataRow(5000000)]
        public async Task GetMaxFileUploadSize_OnSuccess_ReturnsFileSize(int maxFileSize)
        {
            // Arrange
            SetupMessageHandler(HttpStatusCode.OK, maxFileSize);

            var client = GetSettingsApiClient();

            // Act
            var result = await client.GetMaxFileUploadSize();

            // Assert
            result.Should().Be(maxFileSize);
            VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + GetMaxFileUploadSizeRoute);
        }

        private SettingsApiClient GetSettingsApiClient()
            => new SettingsApiClient(GetMockAuthenticationService<DataApiClientConfiguration>().Object, GetHttpClient(), Options.Create(GetServicesConfiguration()), _logger);
    }
}