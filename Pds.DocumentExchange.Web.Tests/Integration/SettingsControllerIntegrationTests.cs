using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Web.Administration.Models;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.Areas.Admin.Controllers;
using Pds.DocumentExchange.Web.Areas.Admin.DTOs;
using Pds.DocumentExchange.Web.Areas.Admin.Implementations;
using Pds.DocumentExchange.Web.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Product = Pds.DocumentExchange.Web.Areas.Admin.Models.Product;
using ServiceProduct = Pds.DocumentExchange.Services.Models.Product;

namespace Pds.DocumentExchange.Web.Tests.Integration
{
    [TestClass, TestCategory("Integration")]
    public class SettingsControllerIntegrationTests : BaseControllerIntegrationTests
    {
        private readonly ISettingsApiClient _settingsApiClient
            = Mock.Of<ISettingsApiClient>(MockBehavior.Strict);

        [TestMethod]
        public void Settings_IndexPage_Test()
        {
            // Arrange
            var controller = GetSettingsController();

            // Act
            var result = controller.Index();

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<IndexViewModel>();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(100)]
        public async Task Settings_ProductsPage_Test(int totalProducts)
        {
            // Arrange
            var mockSettingsClient = Mock.Get(_settingsApiClient);
            mockSettingsClient
                .Setup(a => a.GetAllProducts())
                .ReturnsAsync(GetTestSertviceProducts(totalProducts));

            mockSettingsClient
                .Setup(a => a.GetTeams())
                .ReturnsAsync(GetTestTeams(totalProducts));

            var controller = GetSettingsController();

            // Act
            var result = await controller.Products();

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<Products>()
                .Which.Should().BeEquivalentTo(
                    new Products
                    {
                        ListItems = GetTestProducts(totalProducts)
                            .OrderBy(p => p.LastUpdated?.Date ?? DateTime.MinValue)
                            .ThenBy(p => p.Identifier)
                    }, opt => opt.WithStrictOrderingFor(p => p.ListItems));

            Mock.VerifyAll(mockSettingsClient);
        }

        [TestMethod]
        public async Task Settings_AddProduct_Get_Test()
        {
            // Arrange
            var controller = GetSettingsController();

            var numberOfTeams = 4;

            var mockSettingsClient = Mock.Get(_settingsApiClient);
            mockSettingsClient
                .Setup(a => a.GetTeams())
                .ReturnsAsync(GetTestTeams(numberOfTeams));

            var setting = new SettingViewModel();
            var addEditProduct = new AddEditProduct(setting);
            var dto = new ConfirmAddEditProductPageDto
            {
                Error = false
            };

            // Act
            var result = await controller.AddProduct(dto);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<AddEditPageViewModel>()
                .Which.Should().BeEquivalentTo(
                    new AddEditPageViewModel(addEditProduct, GetTestTeams(numberOfTeams), true));

            Mock.VerifyAll(mockSettingsClient);
        }

        [TestMethod]
        [DataRow(-1, 1, "Product Identifier Must be a 5 digit number in range", "", "Product name should not be empty or should not exceed 40 characters", "", "Product plural name should not be empty or should not exceed 40 characters")]
        [DataRow(-1, 100000, "Product Identifier Must be a 5 digit number in range", "Product name is over 40 characters ssssssssssssss", "Product name should not be empty or should not exceed 40 characters", "Product plural name is over 40 characters sssssss", "Product plural name should not be empty or should not exceed 40 characters")]
        [DataRow(10001, 10001, "Product Identifier Number already in use<br>", "Product name %", "Product name cannot contain special characters<br>", "Product plural name ^", "Product plural name cannot contain special characters<br>")]
        [DataRow(10001, 10001, "Product Identifier Number already in use<br>", "Product name has special characters & exceeds 40 characters", "Product name cannot contain special characters<br>Product name should not be empty or should not exceed 40 characters", "Product plural name has special characters & exceeds 40 characters", "Product plural name cannot contain special characters<br>Product plural name should not be empty or should not exceed 40 characters")]
        public async Task Settings_AddProduct_FieldValidation_WithErrors_Tests(int apiIdentifier, int identifier, string identifierErrorMessage, string name, string nameErrorMessage, string pluralName, string pluralNameErrorMessage)
        {
            // Arrange
            var controller = GetSettingsController();

            var numberOfTeams = 4;

            var mockSettingsClient = Mock.Get(_settingsApiClient);
            mockSettingsClient
                .Setup(a => a.GetTeams())
                .ReturnsAsync(GetTestTeams(numberOfTeams));

            var product = new ServiceProduct()
            {
                Identifier = apiIdentifier,
            };

            mockSettingsClient
                .Setup(a => a.GetProduct(identifier))
                .ReturnsAsync(product);

            var setting = new SettingViewModel();
            var addEditProduct = new AddEditProduct(setting);
            var dto = new ConfirmAddEditProductPageDto
            {
                Identifier = identifier,
                Name = name,
                PluralName = pluralName,
                IsAddPage = true,
                Error = true,
                AgencyTeam = "Agency Team",
                CanOrganisationsUpload = "true",
            };

            // Act
            var result = await controller.AddProduct(dto);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<AddEditPageViewModel>()
                .Which.Should().BeEquivalentTo(
                    new AddEditPageViewModel(addEditProduct, GetTestTeams(numberOfTeams), true, dto.Identifier.ToString())
                    {
                        IdentifierError = !string.IsNullOrEmpty(identifierErrorMessage),
                        IdentifierErrorMessage = identifierErrorMessage,
                        Name = name,
                        NameErrorMessage = nameErrorMessage,
                        NameError = !string.IsNullOrEmpty(nameErrorMessage),
                        PluralName = pluralName,
                        PluralNameErrorMessage = pluralNameErrorMessage,
                        PluralNameError = !string.IsNullOrEmpty(pluralNameErrorMessage),
                        Error = true,
                        AgencyTeam = "Agency Team",
                        CanOrganisationsUpload = "true"
                    });

            Mock.VerifyAll(mockSettingsClient);
        }

        [TestMethod]
        public async Task Settings_EditProduct_Get_Test()
        {
            // Arrange
            var controller = GetSettingsController();

            var numberOfTeams = 4;

            var mockSettingsClient = Mock.Get(_settingsApiClient);
            mockSettingsClient
                .Setup(a => a.GetTeams())
                .ReturnsAsync(GetTestTeams(numberOfTeams));

            var identifier = 10001;
            var product = new ServiceProduct
            {
                Identifier = identifier,
                Name = "Name",
                PluralName = "Plural Name",
                AgencyTeams = new List<string> { "AgencyTeam" },
                CanOrganisationsUpload = true
            };

            mockSettingsClient
                .Setup(a => a.GetProduct(identifier))
                .ReturnsAsync(product);

            var setting = new SettingViewModel();
            var addEditProduct = new AddEditProduct(setting);
            var dto = new ConfirmAddEditProductPageDto
            {
                OldIdentifier = identifier,
                Identifier = identifier,
                Error = false
            };

            // Act
            var result = await controller.EditProduct(dto);

            // Assert
            result.Should().BeViewResult()
                    .Model.Should().BeOfType<AddEditPageViewModel>()
                    .Which.Should().BeEquivalentTo(
                        new AddEditPageViewModel(addEditProduct, GetTestTeams(numberOfTeams), false, identifier.ToString())
                        {
                            OldIdentifier = identifier.ToString(),
                            Identifier = product.Identifier.ToString(),
                            Name = product.Name,
                            AgencyTeam = product.AgencyTeams.FirstOrDefault(),
                            CanOrganisationsUpload = product.CanOrganisationsUpload.ToString(),
                            PluralName = product.PluralName
                        });

            Mock.VerifyAll(mockSettingsClient);
        }

        [TestMethod]
        [DataRow(-1, 1, 1, "Product Identifier Must be a 5 digit number in range", "", "Product name should not be empty or should not exceed 40 characters", "", "Product plural name should not be empty or should not exceed 40 characters")]
        [DataRow(-1, 100000, 100000, "Product Identifier Must be a 5 digit number in range", "Product name is over 40 characters ssssssssssssss", "Product name should not be empty or should not exceed 40 characters", "Product plural name is over 40 characters sssssss", "Product plural name should not be empty or should not exceed 40 characters")]
        [DataRow(10001, 10000, 10001, "Product Identifier Number already in use<br>", "Product name % ", "Product name cannot contain special characters<br>", "Product plural name ^", "Product plural name cannot contain special characters<br>")]
        [DataRow(10001, 10000, 10001, "Product Identifier Number already in use<br>", "Product name has special characters & exceeds 40 characters", "Product name cannot contain special characters<br>Product name should not be empty or should not exceed 40 characters", "Product plural name has special characters & exceeds 40 characters", "Product plural name cannot contain special characters<br>Product plural name should not be empty or should not exceed 40 characters")]
        public async Task Settings_EditProduct_FieldValidation_WithErrors_Tests(int apiIdentifier, int oldIdentifier, int identifier, string identifierErrorMessage, string name, string nameErrorMessage, string pluralName, string pluralNameErrorMessage)
        {
            // Arrange
            var controller = GetSettingsController();

            var numberOfTeams = 4;

            var mockSettingsClient = Mock.Get(_settingsApiClient);
            mockSettingsClient
                .Setup(a => a.GetTeams())
                .ReturnsAsync(GetTestTeams(numberOfTeams));

            var product = new ServiceProduct()
            {
                Identifier = apiIdentifier,
            };

            mockSettingsClient
                .Setup(a => a.GetProduct(identifier))
                .ReturnsAsync(product);

            var setting = new SettingViewModel();
            var addEditProduct = new AddEditProduct(setting);
            var dto = new ConfirmAddEditProductPageDto
            {
                OldIdentifier = oldIdentifier,
                Identifier = identifier,
                Name = name,
                PluralName = pluralName,
                IsAddPage = false,
                Error = true,
                AgencyTeam = "Agency Team",
                CanOrganisationsUpload = "true",
            };

            // Act
            var result = await controller.EditProduct(dto);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<AddEditPageViewModel>()
                .Which.Should().BeEquivalentTo(
                    new AddEditPageViewModel(addEditProduct, GetTestTeams(numberOfTeams), false, dto.Identifier.ToString())
                    {
                        OldIdentifier = identifier.ToString(),
                        IdentifierError = !string.IsNullOrEmpty(identifierErrorMessage),
                        IdentifierErrorMessage = identifierErrorMessage,
                        Name = name,
                        NameErrorMessage = nameErrorMessage,
                        NameError = !string.IsNullOrEmpty(nameErrorMessage),
                        PluralName = pluralName,
                        PluralNameErrorMessage = pluralNameErrorMessage,
                        PluralNameError = !string.IsNullOrEmpty(pluralNameErrorMessage),
                        Error = true,
                        AgencyTeam = "Agency Team",
                        CanOrganisationsUpload = "true"
                    });

            Mock.VerifyAll(mockSettingsClient);
        }

        [TestMethod]
        public async Task Settings_ConfirmAddEditProduct_EditPage_Test()
        {
            // Arrange
            var controller = GetSettingsController();

            var numberOfTeams = 4;

            var mockSettingsClient = Mock.Get(_settingsApiClient);
            mockSettingsClient
                .Setup(a => a.GetTeams())
                .ReturnsAsync(GetTestTeams(numberOfTeams));

            var identifier = 10001;

            var pageData = new ConfirmAddEditProductPageDto
            {
                Identifier = identifier,
                Name = "Name",
                PluralName = "Plural Name",
                AgencyTeam = "agencyTeam1",
                CanOrganisationsUpload = "true",
                OldIdentifier = identifier,
                IsAddPage = false
            };

            var confirmEditViewModel = new ConfirmAddEditPageViewModel()
            {
                Product = new Product
                {
                    Identifier = identifier,
                    Name = "Name",
                    PluralName = "Plural Name",
                    AgencyTeam = "agencyTeam1",
                    CanOrganisationsUpload = true
                },
                AgencyTeamFriendlyName = "friendlyName1",
                OldIdentifier = pageData.Identifier,
                IsAddPage = false
            };

            // Act
            var result = await controller.ConfirmAddEditProduct(pageData);

            // Assert
            result.Should().BeViewResult()
               .Model.Should().BeOfType<ConfirmAddEditPageViewModel>()
               .Which.Should().BeEquivalentTo(confirmEditViewModel);

            Mock.VerifyAll(mockSettingsClient);
        }

        [TestMethod]
        public async Task Settings_ConfirmAddEditProduct_AddPage_ProductIdentifierNotExisting_Test()
        {
            // Arrange
            var controller = GetSettingsController();

            var numberOfTeams = 4;

            var mockSettingsClient = Mock.Get(_settingsApiClient);
            mockSettingsClient
                .Setup(a => a.GetTeams())
                .ReturnsAsync(GetTestTeams(numberOfTeams));

            var identifier = 10001;

            var pageData = new ConfirmAddEditProductPageDto
            {
                Identifier = identifier,
                Name = "Name",
                PluralName = "Plural Name",
                AgencyTeam = "agencyTeam1",
                CanOrganisationsUpload = "true",
                OldIdentifier = identifier,
                IsAddPage = true
            };

            var product = new ServiceProduct()
            {
                Identifier = -1,
            };

            mockSettingsClient
                .Setup(a => a.GetProduct(identifier))
                .ReturnsAsync(product);

            var confirmEditViewModel = new ConfirmAddEditPageViewModel
            {
                Product = new Product
                {
                    Identifier = identifier,
                    Name = "Name",
                    PluralName = "Plural Name",
                    AgencyTeam = "agencyTeam1",
                    CanOrganisationsUpload = true
                },
                AgencyTeamFriendlyName = "friendlyName1",
                OldIdentifier = pageData.Identifier,
                IsAddPage = true
            };

            // Act
            var result = await controller.ConfirmAddEditProduct(pageData);

            // Assert
            result.Should().BeViewResult()
               .Model.Should().BeOfType<ConfirmAddEditPageViewModel>()
               .Which.Should().BeEquivalentTo(confirmEditViewModel);

            Mock.VerifyAll(mockSettingsClient);
        }

        [TestMethod]
        public async Task Settings_ConfirmAddEditProduct_EditPage_ProductExists_Test()
        {
            // Arrange
            var controller = GetSettingsController();
            var mockSettingsClient = Mock.Get(_settingsApiClient);
            var identifier = 10001;

            var pageData = new ConfirmAddEditProductPageDto
            {
                Identifier = identifier,
                Name = "Name",
                PluralName = "Plural Name",
                AgencyTeam = "agencyTeam1",
                CanOrganisationsUpload = "true",
                OldIdentifier = identifier + 1,
                IsAddPage = false
            };

            var product = new ServiceProduct()
            {
                Identifier = identifier,
            };

            mockSettingsClient
                .Setup(a => a.GetProduct(identifier))
                .ReturnsAsync(product);

            var returnDto = new ConfirmAddEditProductPageDto
            {
                Identifier = identifier,
                Name = "Name",
                PluralName = "Plural Name",
                AgencyTeam = "agencyTeam1",
                CanOrganisationsUpload = "true",
                OldIdentifier = pageData.Identifier,
                IsAddPage = false,
                Error = true
            };

            // Act
            var result = await controller.ConfirmAddEditProduct(pageData);

            // Assert
            result.Should()
               .BeRedirectToActionResult(nameof(controller.EditProduct), "Settings", returnDto);

            Mock.VerifyAll(mockSettingsClient);
        }

        [TestMethod]
        public async Task Settings_ConfirmAddEditProduct_AddPage_ProductExists_Test()
        {
            // Arrange
            var controller = GetSettingsController();
            var mockSettingsClient = Mock.Get(_settingsApiClient);
            var identifier = 10001;

            var pageData = new ConfirmAddEditProductPageDto
            {
                Identifier = identifier,
                Name = "Name",
                PluralName = "Plural Name",
                AgencyTeam = "agencyTeam1",
                CanOrganisationsUpload = "true",
                OldIdentifier = identifier + 1,
                IsAddPage = true
            };

            var product = new ServiceProduct()
            {
                Identifier = identifier,
            };

            mockSettingsClient
                .Setup(a => a.GetProduct(identifier))
                .ReturnsAsync(product);

            var returnDto = new ConfirmAddEditProductPageDto
            {
                Identifier = identifier,
                Name = "Name",
                PluralName = "Plural Name",
                AgencyTeam = "agencyTeam1",
                CanOrganisationsUpload = "true",
                OldIdentifier = pageData.Identifier,
                IsAddPage = true,
                Error = true
            };

            // Act
            var result = await controller.ConfirmAddEditProduct(pageData);

            // Assert
            result.Should()
               .BeRedirectToActionResult(nameof(controller.AddProduct), "Settings", returnDto);

            Mock.VerifyAll(mockSettingsClient);
        }

        [TestMethod]
        public async Task Settings_ConfirmAddEditProduct_EditPage_ProductFieldsAreInvalid_Test()
        {
            // Arrange
            var controller = GetSettingsController();
            var mockSettingsClient = Mock.Get(_settingsApiClient);
            var identifier = 1000;

            var pageData = new ConfirmAddEditProductPageDto
            {
                Identifier = identifier,
                Name = "Name is over 40 characters ssssssssssssss",
                PluralName = "Plural Name has special* characters",
                AgencyTeam = "agencyTeam1",
                CanOrganisationsUpload = "true",
                OldIdentifier = identifier + 1,
                IsAddPage = false
            };

            var product = new ServiceProduct()
            {
                Identifier = -1,
            };

            mockSettingsClient
                .Setup(a => a.GetProduct(identifier))
                .ReturnsAsync(product);

            var returnDto = new ConfirmAddEditProductPageDto
            {
                Identifier = identifier,
                Name = pageData.Name,
                PluralName = pageData.PluralName,
                AgencyTeam = "agencyTeam1",
                CanOrganisationsUpload = "true",
                OldIdentifier = pageData.OldIdentifier,
                IsAddPage = false,
                Error = true
            };

            // Act
            var result = await controller.ConfirmAddEditProduct(pageData);

            // Assert
            result.Should()
               .BeRedirectToActionResult(nameof(controller.EditProduct), "Settings", returnDto);

            Mock.VerifyAll(mockSettingsClient);
        }

        [TestMethod]
        public async Task Settings_ConfirmAddEditProduct_AddPage_ProductFieldsAreInvalid_Test()
        {
            // Arrange
            var controller = GetSettingsController();
            var mockSettingsClient = Mock.Get(_settingsApiClient);
            var identifier = 1000;

            var pageData = new ConfirmAddEditProductPageDto
            {
                Identifier = identifier,
                Name = "Name is over 40 characters ssssssssssssss",
                PluralName = "Plural Name has special* characters",
                AgencyTeam = "agencyTeam1",
                CanOrganisationsUpload = "true",
                OldIdentifier = identifier + 1,
                IsAddPage = true
            };

            var product = new ServiceProduct()
            {
                Identifier = -1,
            };

            mockSettingsClient
                .Setup(a => a.GetProduct(identifier))
                .ReturnsAsync(product);

            var returnDto = new ConfirmAddEditProductPageDto
            {
                Identifier = identifier,
                Name = pageData.Name,
                PluralName = pageData.PluralName,
                AgencyTeam = "agencyTeam1",
                CanOrganisationsUpload = "true",
                OldIdentifier = pageData.OldIdentifier,
                IsAddPage = true,
                Error = true
            };

            // Act
            var result = await controller.ConfirmAddEditProduct(pageData);

            // Assert
            result.Should()
               .BeRedirectToActionResult(nameof(controller.AddProduct), "Settings", returnDto);

            Mock.VerifyAll(mockSettingsClient);
        }

        [TestMethod]
        public async Task Settings_AddOrUpdateProduct_Test()
        {
            // Arrange
            var controller = GetSettingsController();

            var identifier = 10001;

            var pageData = new ConfirmAddEditProductPageDto
            {
                Identifier = identifier,
                Name = "Name",
                PluralName = "Plural Name",
                AgencyTeam = "agencyTeam1",
                CanOrganisationsUpload = "true",
                OldIdentifier = identifier
            };

            var expectedServiceProduct = new ServiceProduct
            {
                Identifier = pageData.Identifier,
                Name = pageData.Name,
                PluralName = pageData.PluralName,
                AgencyTeams = new List<string> { pageData.AgencyTeam },
                CanOrganisationsUpload = bool.Parse(pageData.CanOrganisationsUpload)
            };

            var actualServiceProduct = new ServiceProduct
            {
                Identifier = pageData.Identifier,
                Name = pageData.Name,
                PluralName = pageData.PluralName,
                AgencyTeams = new List<string> { pageData.AgencyTeam },
                CanOrganisationsUpload = bool.Parse(pageData.CanOrganisationsUpload)
            };

            var mockSettingsClient = Mock.Get(_settingsApiClient);
            mockSettingsClient
                .Setup(a => a.AddOrUpdateProduct(identifier, It.IsAny<ServiceProduct>()))
                .ReturnsAsync((int identifier, ServiceProduct actualServiceProduct) => { return actualServiceProduct; });

            // Act
            var result = await controller.AddOrUpdateProduct(pageData);

            // Assert
            actualServiceProduct.Should().BeEquivalentTo(expectedServiceProduct);
            result.Should()
                .BeRedirectToActionResult(nameof(Products), "Settings", new { });

            Mock.VerifyAll(mockSettingsClient);
        }

        private SettingsController GetSettingsController()
        {
            return new SettingsController(
                    UserInfoProvider,
                    Options.Create(TestConfiguration),
                    _settingsApiClient,
                    new ValidationService());
        }

        private IEnumerable<Product> GetTestProducts(int totalProducts)
        {
            return Enumerable
                 .Range(1, totalProducts)
                 .Select(p => new Product
                 {
                     Identifier = 10000 + p,
                     Name = $"name{p}",
                     PluralName = $"pluralName{p}",
                     AgencyTeam = $"friendlyName{p}",
                     CanOrganisationsUpload = (p % 2) == 0,
                     LastUpdated = (p % 2) == 0 ? new DateTime(2020, 1, (p % 31) + 1, 12, 34, 56) : (DateTime?)null
                 });
        }

        private IEnumerable<ServiceProduct> GetTestSertviceProducts(int totalProducts)
        {
            return Enumerable
                .Range(1, totalProducts)
                .Select(p => new ServiceProduct
                {
                    Identifier = 10000 + p,
                    Name = $"name{p}",
                    PluralName = $"pluralName{p}",
                    AgencyTeams = new List<string> { $"agencyTeam{p}" },
                    CanOrganisationsUpload = (p % 2) == 0,
                    LastUpdated = (p % 2) == 0 ? new DateTime(2020, 1, (p % 31) + 1, 12, 34, 56) : (DateTime?)null
                });
        }

        private IEnumerable<AgencyTeam> GetTestTeams(int totalTeams)
        {
            return Enumerable
               .Range(1, totalTeams)
               .Select(t => new AgencyTeam
               {
                   Identifier = $"agencyTeam{t}",
                   Name = $"friendlyName{t}"
               });
        }
    }
}