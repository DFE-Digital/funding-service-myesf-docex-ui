//using AutoMapper;
using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Common.Organisation.Enums;
using Pds.Core.Common.Organisation.Models;
using Pds.Core.Logging;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Services.Models.Filters;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Helpers;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using Pds.DocumentExchange.Web.Models.Organisation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Tests.Unit.Controllers
{
    [TestClass]
    public class OrganisationControllerTests : BaseControllerUnitTests
    {
        private const string FakeMimeTypeString = "application/fake";

        private readonly ISettingsApiClient _settingsApiClient
            = Mock.Of<ISettingsApiClient>(MockBehavior.Strict);

        private readonly IUploadApiClient _uploadApiClient
            = Mock.Of<IUploadApiClient>(MockBehavior.Strict);

        private readonly ILoggerAdapter<OrganisationController> _logger
            = Mock.Of<ILoggerAdapter<OrganisationController>>();

        private readonly IDateTimeDisplayHelper _dateTimeDisplayHelper
            = Mock.Of<IDateTimeDisplayHelper>(MockBehavior.Strict);

        private readonly IListHelper _listHelper
            = Mock.Of<IListHelper>(MockBehavior.Strict);

        #region Home

        [TestMethod, TestCategory("Unit")]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(null)]
        [DataRow(23424)]
        [DataRow(787)]
        [DataRow(14342)]
        [DataRow(25346)]
        public async Task Home_ForParentOrgUser_MakesExpectedApiCallsAndReturnsExpectedView(int? newDocuments)
        {
            // Arrange
            var testOrganisationUserInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(i => i.GetCurrentUserInfo())
                .ReturnsAsync(testOrganisationUserInfo);

            Mock.Get(UserInformationProvider)
                .Setup(i => i.CurrentUserShouldSeeParentView())
                .ReturnsAsync(true);

            Mock.Get(ExchangeApiClient)
                .Setup(a => a.GetOrganisationUserSummary(testOrganisationUserInfo))
                .ReturnsAsync(new Summary
                {
                    CountOfNewDocuments = newDocuments
                });

            var controller = GetOrganisationController();

            // Act
            var result = await controller.Home();

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<Home>()
                .Which.Should().BeEquivalentTo(
                    new Home
                    {
                        CountOfNewDocuments = newDocuments ?? 0,
                        SendNewDocumentActionName = nameof(OrganisationController.SelectOrganisation)
                    });

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(ExchangeApiClient));
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(null)]
        [DataRow(23424)]
        [DataRow(787)]
        [DataRow(14342)]
        [DataRow(25346)]
        public async Task Home_ForChildOrgUser_MakesExpectedApiCallsAndReturnsExpectedView(int? newDocuments)
        {
            // Arrange
            var testOrganisationUserInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(i => i.GetCurrentUserInfo())
                .ReturnsAsync(testOrganisationUserInfo);

            Mock.Get(UserInformationProvider)
                .Setup(i => i.CurrentUserShouldSeeParentView())
                .ReturnsAsync(false);

            Mock.Get(ExchangeApiClient)
                .Setup(a => a.GetOrganisationUserSummary(testOrganisationUserInfo))
                .ReturnsAsync(new Summary
                {
                    CountOfNewDocuments = newDocuments
                });

            var controller = GetOrganisationController();

            // Act
            var result = await controller.Home();

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<Home>()
                .Which.Should().BeEquivalentTo(
                    new Home
                    {
                        CountOfNewDocuments = newDocuments ?? 0,
                        SendNewDocumentActionName = nameof(OrganisationController.SelectDocumentType)
                    });

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(ExchangeApiClient));
        }

        #endregion


        #region SelectOrganisation

        [TestMethod, TestCategory("Unit")]
        [DataRow(true)]
        [DataRow(false)]
        public async Task SelectOrganisation_ForParentOrganisationUser_ReturnsTheView(bool error)
        {
            // Arrange
            Mock.Get(UserInformationProvider)
                .Setup(i => i.CurrentUserShouldSeeParentView())
                .ReturnsAsync(true);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.SelectOrganisation(error);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(new SelectOrganisation { Error = error });

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SelectOrganisation_ForChildOrganisationUser_RedirectsToSelectDocumentType()
        {
            // Arrange
            Mock.Get(UserInformationProvider)
                .Setup(i => i.CurrentUserShouldSeeParentView())
                .ReturnsAsync(false);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.SelectOrganisation(false);

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(OrganisationController.SelectDocumentType));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        #endregion


        #region SelectAcademy

        [TestMethod, TestCategory("Unit")]
        public async Task SelectAcademy_ForChildOrganisationUser_RedirectsToSelectDocumentType()
        {
            // Arrange
            Mock.Get(UserInformationProvider)
                .Setup(i => i.GetCurrentUserChildOrganisations())
                .ReturnsAsync(Enumerable.Empty<Organisation>());

            var controller = GetOrganisationController();

            // Act
            var result = await controller.SelectAcademy(null);

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(OrganisationController.SelectDocumentType));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SelectAcademy_ForNoOptionSelected_RedirectsToSelectOrganisation()
        {
            // Arrange
            var controller = GetOrganisationController();

            // Act
            var result = await controller.SelectAcademy(new SelectAcademy());

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithPreserveMethod(true)
                .WithActionName(nameof(OrganisationController.SelectOrganisation))
                .WithControllerName(NameOf<OrganisationController>())
                .WithRouteValue("error", true);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SelectAcademy_WhenUserDidNotSelectSendForChildOrganisation_RedirectsToSelectDocumentType()
        {
            // Arrange
            var controller = GetOrganisationController();

            // Act
            var result = await controller.SelectAcademy(new SelectAcademy
            {
                SelectChildAcademy = false
            });

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(OrganisationController.SelectDocumentType));
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(224)]
        [DataRow(787)]
        [DataRow(142)]
        [DataRow(253)]
        public async Task SelectAcademy_WhenUserSelectedSendForChildOrganisation_ReturnsTheViewModelWithTheChildAcademies(
            int numberOfChildAcademies)
        {
            // Arrange
            var testParentOrganisation = GetTestParentOrganisation(numberOfChildAcademies);

            Mock.Get(UserInformationProvider)
                .Setup(i => i.GetCurrentUserChildOrganisations())
                .ReturnsAsync(testParentOrganisation.ChildOrganisations);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.SelectAcademy(new SelectAcademy
            {
                SelectChildAcademy = true
            });

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(
                    new SelectAcademy
                    {
                        SelectChildAcademy = true,
                        ChildAcademyPages = testParentOrganisation.ChildOrganisations
                        .Select(c =>
                            new ChildOrganisationInfo
                            {
                                Name = c.Name,
                                Ukprn = c.Identifiers.First().Value
                            })
                        .Paginate(TestConfiguration.ListPageSize),
                    },
                    options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(224)]
        [DataRow(787)]
        [DataRow(142)]
        [DataRow(246)]
        public async Task SelectAcademy_WhenPerformingNewSearch_FiltersTheResultsAndResetsThePagination(
            int numberOfChildAcademies)
        {
            // Arrange
            const string ChildOrgSearchTerm = "child org 1";

            var testParentOrganisation = GetTestParentOrganisation(numberOfChildAcademies);

            Mock.Get(UserInformationProvider)
                .Setup(i => i.GetCurrentUserChildOrganisations())
                .ReturnsAsync(testParentOrganisation.ChildOrganisations);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.SelectAcademy(new SelectAcademy
            {
                SelectChildAcademy = true,
                SearchTerm = ChildOrgSearchTerm,
                PageNumber = 2,
                NewSearch = true
            });

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(
                    new SelectAcademy
                    {
                        SelectChildAcademy = true,
                        SearchTerm = ChildOrgSearchTerm,
                        PageNumber = null,
                        NewSearch = true,
                        ChildAcademyPages = testParentOrganisation.ChildOrganisations
                            .Where(c => c.Name.StartsWith(ChildOrgSearchTerm))
                            .Select(c =>
                                new ChildOrganisationInfo
                                {
                                    Name = c.Name,
                                    Ukprn = c.Identifiers.First().Value
                                })
                            .Paginate(TestConfiguration.ListPageSize),
                    },
                    options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(424)]
        [DataRow(787)]
        [DataRow(342)]
        [DataRow(253)]
        public async Task SelectAcademy_WhenChangingPageWithoutSearch_MaintainsThePagination(
            int numberOfChildAcademies)
        {
            // Arrange
            const int SelectedPageNumber = 3;

            var testParentOrganisation = GetTestParentOrganisation(numberOfChildAcademies);

            Mock.Get(UserInformationProvider)
                .Setup(i => i.GetCurrentUserChildOrganisations())
                .ReturnsAsync(testParentOrganisation.ChildOrganisations);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.SelectAcademy(new SelectAcademy
            {
                SelectChildAcademy = true,
                PageNumber = SelectedPageNumber
            });

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(
                    new SelectAcademy
                    {
                        SelectChildAcademy = true,
                        PageNumber = SelectedPageNumber,
                        ChildAcademyPages = testParentOrganisation.ChildOrganisations
                            .Select(c =>
                                new ChildOrganisationInfo
                                {
                                    Name = c.Name,
                                    Ukprn = c.Identifiers.First().Value
                                })
                            .Paginate(TestConfiguration.ListPageSize),
                    },
                    options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(224)]
        [DataRow(787)]
        [DataRow(142)]
        [DataRow(246)]
        public async Task SelectAcademy_WhenChangingPageWithinExistingSearch_FiltersTheResultsAndMaintainsThePagination(
            int numberOfChildAcademies)
        {
            // Arrange
            const string ChildOrgSearchTerm = "child org 1";
            const int SelectedPageNumber = 3;

            var testParentOrganisation = GetTestParentOrganisation(numberOfChildAcademies);

            Mock.Get(UserInformationProvider)
                .Setup(i => i.GetCurrentUserChildOrganisations())
                .ReturnsAsync(testParentOrganisation.ChildOrganisations);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.SelectAcademy(new SelectAcademy
            {
                SelectChildAcademy = true,
                SearchTerm = ChildOrgSearchTerm,
                PageNumber = SelectedPageNumber
            });

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(
                    new SelectAcademy
                    {
                        SelectChildAcademy = true,
                        SearchTerm = ChildOrgSearchTerm,
                        PageNumber = SelectedPageNumber,
                        ChildAcademyPages = testParentOrganisation.ChildOrganisations
                            .Where(c => c.Name.StartsWith(ChildOrgSearchTerm))
                            .Select(c =>
                                new ChildOrganisationInfo
                                {
                                    Name = c.Name,
                                    Ukprn = c.Identifiers.First().Value
                                })
                            .Paginate(TestConfiguration.ListPageSize),
                    },
                    options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(224)]
        [DataRow(787)]
        [DataRow(142)]
        [DataRow(246)]
        public async Task SelectAcademy_WhenClearingTheSearchTerm_ClearsTheSearchTermAndReturnsUnfilteredResults(
            int numberOfChildAcademies)
        {
            // Arrange
            var testParentOrganisation = GetTestParentOrganisation(numberOfChildAcademies);

            Mock.Get(UserInformationProvider)
                .Setup(i => i.GetCurrentUserChildOrganisations())
                .ReturnsAsync(testParentOrganisation.ChildOrganisations);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.SelectAcademy(new SelectAcademy
            {
                SelectChildAcademy = true,
                SearchTerm = "fake search term",
                ClearSearch = true,
                PageNumber = 2
            });

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(
                    new SelectAcademy
                    {
                        SelectChildAcademy = true,
                        SearchTerm = string.Empty,
                        ClearSearch = true,
                        PageNumber = null,
                        ChildAcademyPages = testParentOrganisation.ChildOrganisations
                        .Select(c =>
                            new ChildOrganisationInfo
                            {
                                Name = c.Name,
                                Ukprn = c.Identifiers.First().Value
                            })
                        .Paginate(TestConfiguration.ListPageSize),
                    },
                    options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        #endregion


        #region SelectDocumentType

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(224)]
        [DataRow(787)]
        [DataRow(142)]
        [DataRow(346)]
        public async Task SelectDocumentType_ForNullInputViewModel_ReturnsTheExpectedViewModel(
            int numberOfAllowedProducts)
        {
            // Arrange
            var testProducts = GetTestProducts(numberOfAllowedProducts);

            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetProductsThatOrganisationsCanUpload())
                .ReturnsAsync(testProducts);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.CurrentUserShouldSeeParentView())
                .ReturnsAsync(false);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.SelectDocumentType(null);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(new SelectDocumentType
                {
                    AllowedProducts = testProducts.Select(p => new Models.Shared.Product
                    {
                        Identifier = p.Identifier,
                        Name = p.Name
                    })
                });

            Mock.VerifyAll(
                Mock.Get(_settingsApiClient));
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SelectDocumentType_WhenSelectAnOrganisationRequiredButUploadingForUkprnIsNull_RedirectsToSelectAcademy()
        {
            // Arrange
            var controller = GetOrganisationController();
            var inputViewModel = new SelectDocumentType
            {
                SelectAnOrganisationRequired = true,
                UploadingForUkprn = null
            };

            // Act
            var result = await controller.SelectDocumentType(inputViewModel);

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithPreserveMethod(true)
                .WithActionName(nameof(OrganisationController.SelectAcademy))
                .WithControllerName(NameOf<OrganisationController>())
                .WithRouteValue("error", true);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(234)]
        [DataRow(787)]
        [DataRow(143)]
        [DataRow(256)]
        public async Task SelectDocumentType_ForPopulatedInputViewModel_ReturnsTheExpectedViewModel(
            int numberOfAllowedProducts)
        {
            // Arrange
            var testProducts = GetTestProducts(numberOfAllowedProducts);

            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetProductsThatOrganisationsCanUpload())
                .ReturnsAsync(testProducts);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.CurrentUserShouldSeeParentView())
                .ReturnsAsync(false);

            var controller = GetOrganisationController();

            var inputViewModel = new SelectDocumentType
            {
                UploadingForUkprn = "87654321",
                SelectAnOrganisationRequired = true
            };

            // Act
            var result = await controller.SelectDocumentType(inputViewModel);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(new SelectDocumentType
                {
                    UploadingForUkprn = inputViewModel.UploadingForUkprn,
                    SelectAnOrganisationRequired = inputViewModel.SelectAnOrganisationRequired,
                    AllowedProducts = testProducts.Select(p => new Models.Shared.Product
                    {
                        Identifier = p.Identifier,
                        Name = p.Name
                    })
                });

            Mock.VerifyAll(
                Mock.Get(_settingsApiClient));
        }

        #endregion


        #region SelectDocument

        [TestMethod, TestCategory("Unit")]
        public async Task SelectDocument_ForNullInputViewModel_RedirectsToSelectDocumentType()
        {
            // Arrange
            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetProductsThatOrganisationsCanUpload())
                .ReturnsAsync(Enumerable.Empty<Product>());

            SetupSettingsApiClientGetMaxFileUploadSize();

            var controller = GetOrganisationController();

            // Act
            var result = await controller.SelectDocument(null);

            // Assert
            result.Should()
                .BeRedirectToActionResult()
                .WithPreserveMethod(true)
                .WithActionName(nameof(OrganisationController.SelectDocumentType));

            Mock.VerifyAll(
                Mock.Get(_settingsApiClient));
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SelectDocument_WhenProductIdentifierNotSelected_RedirectsToSelectDocumentType()
        {
            // Arrange
            var controller = GetOrganisationController();

            // Act
            var result = await controller.SelectDocument(new SelectDocument { ProductIdentifier = 0 });

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithPreserveMethod(true)
                .WithActionName(nameof(OrganisationController.SelectDocumentType))
                .WithControllerName(NameOf<OrganisationController>())
                .WithRouteValue("error", true);
        }

        [TestMethod, TestCategory("Unit")]
        public async Task SelectDocument_WhenProductNotAllowed_RedirectsToSelectDocumentType()
        {
            // Arrange
            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetProductsThatOrganisationsCanUpload())
                .ReturnsAsync(Enumerable.Empty<Product>());

            SetupSettingsApiClientGetMaxFileUploadSize();

            var controller = GetOrganisationController();

            // Act
            var result = await controller.SelectDocument(new SelectDocument
            {
                ProductIdentifier = 1
            });

            // Assert
            result.Should()
                .BeRedirectToActionResult()
                .WithPreserveMethod(true)
                .WithActionName(nameof(OrganisationController.SelectDocumentType));

            Mock.VerifyAll(
                Mock.Get(_settingsApiClient));
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(23424)]
        [DataRow(787)]
        [DataRow(14342)]
        [DataRow(25346)]
        public async Task SelectDocument_WhenNotUploadingForUkprn_GetsVersionForTheCurrentUsersOrganisation(
            int numberOfAllowedProducts)
        {
            // Arrange
            var testProducts = GetTestProducts(numberOfAllowedProducts).ToList();
            var testSelectedProduct = testProducts.Last();
            var testOrganisationId = TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier;
            var testCurrentVersion = 123;
            SetUpSettingsApiClient();

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentOrganisationIdentifier())
                .ReturnsAsync(testOrganisationId);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.IsCurrentUserExternal())
                .Returns(true);

            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetProductsThatOrganisationsCanUpload())
                .ReturnsAsync(testProducts);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.CurrentUserShouldSeeParentView())
                .ReturnsAsync(false);

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.GetCurrentProductVersionForOrganisation(
                    testOrganisationId,
                    testSelectedProduct.Identifier.ToString()))
                .ReturnsAsync(testCurrentVersion);

            var controller = GetOrganisationController();

            var inputViewModel = new SelectDocument
            {
                ProductIdentifier = testSelectedProduct.Identifier
            };

            // Act
            var result = await controller.SelectDocument(inputViewModel);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(new SelectDocument
                {
                    ProductIdentifier = inputViewModel.ProductIdentifier,
                    UploadingForUkprn = inputViewModel.UploadingForUkprn,
                    ProductName = testSelectedProduct.Name,
                    NextVersionNumber = testCurrentVersion + 1,
                    AllowedFileExtensions = GetFileExtensionInfo().Select(f => f.Extension.ToUpperInvariant()).ToList(),
                    MaxFileUploadSize = 1
                });

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(_settingsApiClient),
                Mock.Get(ExchangeApiClient));
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(23424)]
        [DataRow(787)]
        [DataRow(14342)]
        [DataRow(25346)]
        public async Task SelectDocument_WhenUploadingForUkprn_GetsVersionForThePassedOrganisation(
            int numberOfAllowedProducts)
        {
            // Arrange
            var testProducts = GetTestProducts(numberOfAllowedProducts).ToList();
            var testSelectedProduct = testProducts.Last();
            var testUkprn = "12345678";
            var testCurrentVersion = 123;
            SetUpSettingsApiClient();

            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetProductsThatOrganisationsCanUpload())
                .ReturnsAsync(testProducts);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.CurrentUserShouldSeeParentView())
                .ReturnsAsync(true);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.IsCurrentUserExternal())
                .Returns(true);

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.GetCurrentProductVersionForOrganisation(
                    It.Is<OrganisationIdentifier>(o =>
                        o.Value == testUkprn &&
                        o.Type == OrganisationIdentifierType.Ukprn),
                    testSelectedProduct.Identifier.ToString()))
                .ReturnsAsync(testCurrentVersion);

            var controller = GetOrganisationController();

            var inputViewModel = new SelectDocument
            {
                ProductIdentifier = testSelectedProduct.Identifier,
                UploadingForUkprn = testUkprn
            };

            // Act
            var result = await controller.SelectDocument(inputViewModel);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(new SelectDocument
                {
                    ProductIdentifier = inputViewModel.ProductIdentifier,
                    UploadingForUkprn = inputViewModel.UploadingForUkprn,
                    ProductName = testSelectedProduct.Name,
                    NextVersionNumber = testCurrentVersion + 1,
                    ShowParentView = true,
                    AllowedFileExtensions = GetFileExtensionInfo().Select(f => f.Extension.ToUpperInvariant()).ToList(),
                    MaxFileUploadSize = 1
                });

            Mock.VerifyAll(
                Mock.Get(ExchangeApiClient),
                Mock.Get(_settingsApiClient));
        }

        #endregion


        #region DocumentUploadComplete

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(23424)]
        [DataRow(787)]
        [DataRow(14342)]
        [DataRow(25346)]
        public async Task DocumentUploadComplete_ForParentOrganisationUser_SendingAsSelf_SetsTheParentOrganisationName(
            int numberOfChildOrganisations)
        {
            // Arrange
            var testOrganisationUserInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserInfo())
                .ReturnsAsync(testOrganisationUserInfo);

            var testParentOrganisation = GetTestParentOrganisation(numberOfChildOrganisations);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserChildOrganisations())
                .ReturnsAsync(testParentOrganisation.ChildOrganisations);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.CurrentUserShouldSeeParentView())
                .ReturnsAsync(true);

            var controller = GetOrganisationController();

            var inputViewModel = new DocumentUploadComplete
            {
                DocumentFileName = "test file name 1.pdf"
            };

            // Act
            var result = await controller.DocumentUploadComplete(inputViewModel);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(new DocumentUploadComplete
                {
                    DocumentFileName = inputViewModel.DocumentFileName,
                    SelectAnOrganisationRequired = true,
                    OrganisationName = testOrganisationUserInfo.OrganisationInfo.Name,
                    SendNewDocumentActionName = nameof(OrganisationController.SelectOrganisation)
                });

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(1)]
        [DataRow(23424)]
        [DataRow(787)]
        [DataRow(14342)]
        [DataRow(25346)]
        public async Task DocumentUploadComplete_ForParentOrganisationUser_SendingAsChild_SetsTheChildOrganisationName(
            int numberOfChildOrganisations)
        {
            // Arrange
            var testOrganisationUserInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserInfo())
                .ReturnsAsync(testOrganisationUserInfo);

            var testParentOrganisation = GetTestParentOrganisation(numberOfChildOrganisations);
            var testChildOrganisation = testParentOrganisation.ChildOrganisations.Last();

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserChildOrganisations())
                .ReturnsAsync(testParentOrganisation.ChildOrganisations);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.CurrentUserShouldSeeParentView())
                .ReturnsAsync(true);

            var controller = GetOrganisationController();

            var inputViewModel = new DocumentUploadComplete
            {
                DocumentFileName = "test file name 1.pdf",
                UploadingForUkprn = testChildOrganisation.Identifiers.First().Value
            };

            // Act
            var result = await controller.DocumentUploadComplete(inputViewModel);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(new DocumentUploadComplete
                {
                    DocumentFileName = inputViewModel.DocumentFileName,
                    UploadingForUkprn = inputViewModel.UploadingForUkprn,
                    SelectAnOrganisationRequired = true,
                    OrganisationName = testChildOrganisation.Name,
                    SendNewDocumentActionName = nameof(OrganisationController.SelectOrganisation)
                });

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        [TestMethod, TestCategory("Unit")]
        public async Task DocumentUploadComplete_ForChildOrganisationUser_DoesNotSetTheOrganisationName()
        {
            // Arrange
            var testOrganisationUserInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserInfo())
                .ReturnsAsync(testOrganisationUserInfo);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserChildOrganisations())
                .ReturnsAsync(Enumerable.Empty<Organisation>());

            Mock.Get(UserInformationProvider)
                .Setup(u => u.CurrentUserShouldSeeParentView())
                .ReturnsAsync(false);

            var controller = GetOrganisationController();

            var inputViewModel = new DocumentUploadComplete
            {
                DocumentFileName = "test file name 1.pdf"
            };

            // Act
            var result = await controller.DocumentUploadComplete(inputViewModel);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(new DocumentUploadComplete
                {
                    DocumentFileName = inputViewModel.DocumentFileName,
                    SelectAnOrganisationRequired = false,
                    OrganisationName = null,
                    SendNewDocumentActionName = nameof(OrganisationController.SelectDocumentType)
                });

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        #endregion


        #region SentDocuments

        [TestMethod, TestCategory("Unit")]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 91)]
        [DataRow(123, 43)]
        [DataRow(234, 64)]
        [DataRow(234, 82)]
        public async Task SentDocuments_ForParentOrganisation_MakesExpectedApiCallsAndReturnsExpectedView(
            int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var testOrganisationUserInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentOrganisationIdentifier())
                .ReturnsAsync(testOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.CurrentUserShouldSeeParentView())
                .ReturnsAsync(true);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();
            var testViewModelFilters = GetTestViewModelFilterCategories(numberOfFilters).ToList();

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockExchangeClient = Mock.Get(ExchangeApiClient);

            var listResult = new ListResult<ExchangeDocument>
            {
                Items = GetTestExchangeDocuments(numberOfPages, true),
                Filters = testServiceFilters,
                TotalPages = numberOfPages,
                TotalItems = expectedNumberOfItems
            };

            mockExchangeClient
                .Setup(a => a.GetOrganisationDocuments(
                    It.Is<ExchangeListOrganisationDocumentOptions>(options =>
                        options.DocumentStatusOption == ExchangeDocumentDirection.SentByOrganisation
                        && options.PageSize == TestConfiguration.ListPageSize
                        && options.OrganisationIdentifier == testOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier)))
                .ReturnsAsync(listResult);

            var expectedPagination = new PaginationViewModel
            {
                TotalItems = expectedNumberOfItems,
                TotalPages = numberOfPages,
                Page = 1,
                PageSize = TestConfiguration.ListPageSize
            };

            var listHelper = Mock.Get(_listHelper);
            listHelper
                .Setup(l => l.GetFilterOptions(null, false))
                .Returns((IEnumerable<IFilterOption>)null);

            listHelper
                .Setup(l => l.GetPaginationViewModel(null, listResult))
                .Returns(expectedPagination);

            listHelper
                .Setup(l => l.GetFilterCategories(listResult))
                .Returns(testViewModelFilters);

            listHelper
                .Setup(l => l.AnyDocumentsAvailable(null, listResult))
                .Returns(expectedNumberOfItems > 0);

            SetUpDateTimeDisplayHelper(true);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.SentDocuments(null);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<SentDocuments>()
                .Which.Should().BeEquivalentTo(
                    new SentDocuments
                    {
                        ListItems = GetTestSentDocuments(numberOfPages),
                        FilterCategories = testViewModelFilters,
                        ShowParentView = true,
                        Pagination = expectedPagination,
                        AnyDocumentsAvailable = expectedNumberOfItems > 0
                    }, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                mockExchangeClient,
                listHelper);

            //Mock.Get(Mapper),
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 91)]
        [DataRow(123, 43)]
        [DataRow(234, 64)]
        [DataRow(234, 82)]
        public async Task SentDocuments_ForChildOrganisation_MakesExpectedApiCallsAndReturnsExpectedView(
            int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var testOrganisationUserInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentOrganisationIdentifier())
                .ReturnsAsync(testOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.CurrentUserShouldSeeParentView())
                .ReturnsAsync(false);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();
            var testViewModelFilters = GetTestViewModelFilterCategories(numberOfFilters).ToList();

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockExchangeClient = Mock.Get(ExchangeApiClient);

            var listResult = new ListResult<ExchangeDocument>
            {
                Items = GetTestExchangeDocuments(numberOfPages, true),
                Filters = testServiceFilters,
                TotalPages = numberOfPages,
                TotalItems = expectedNumberOfItems
            };

            mockExchangeClient
                .Setup(a => a.GetOrganisationDocuments(
                    It.Is<ExchangeListOrganisationDocumentOptions>(options =>
                        options.DocumentStatusOption == ExchangeDocumentDirection.SentByOrganisation
                        && options.PageSize == TestConfiguration.ListPageSize
                        && options.OrganisationIdentifier == testOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier)))
                .ReturnsAsync(listResult);

            var expectedPagination = new PaginationViewModel
            {
                TotalItems = expectedNumberOfItems,
                TotalPages = numberOfPages,
                Page = 1,
                PageSize = TestConfiguration.ListPageSize
            };

            var listHelper = Mock.Get(_listHelper);
            listHelper
                .Setup(l => l.GetFilterOptions(null, false))
                .Returns((IEnumerable<IFilterOption>)null);

            listHelper
                .Setup(l => l.GetPaginationViewModel(null, listResult))
                .Returns(expectedPagination);

            listHelper
                .Setup(l => l.GetFilterCategories(listResult))
                .Returns(testViewModelFilters);

            listHelper
                .Setup(l => l.AnyDocumentsAvailable(null, listResult))
                .Returns(expectedNumberOfItems > 0);

            SetUpDateTimeDisplayHelper(true);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.SentDocuments(null);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<SentDocuments>()
                .Which.Should().BeEquivalentTo(
                    new SentDocuments
                    {
                        ListItems = GetTestSentDocuments(numberOfPages),
                        FilterCategories = testViewModelFilters,
                        ShowParentView = false,
                        Pagination = expectedPagination,
                        AnyDocumentsAvailable = expectedNumberOfItems > 0
                    }, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                mockExchangeClient,
                listHelper);

            //Mock.Get(Mapper),
        }

        #endregion


        #region SentDocumentsData

        [TestMethod, TestCategory("Unit")]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 91)]
        [DataRow(123, 43)]
        [DataRow(234, 64)]
        [DataRow(234, 82)]
        public async Task SentDocumentsData_ForParentOrganisation_MakesExpectedApiCallsAndReturnsExpectedJson(
            int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var testOrganisationUserInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentOrganisationIdentifier())
                .ReturnsAsync(testOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.CurrentUserShouldSeeParentView())
                .ReturnsAsync(true);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockExchangeClient = Mock.Get(ExchangeApiClient);

            var listResult = new ListResult<ExchangeDocument>
            {
                Items = GetTestExchangeDocuments(numberOfPages, true),
                Filters = testServiceFilters,
                TotalPages = numberOfPages,
                TotalItems = expectedNumberOfItems
            };

            mockExchangeClient
                .Setup(a => a.GetOrganisationDocuments(
                    It.Is<ExchangeListOrganisationDocumentOptions>(options =>
                        options.DocumentStatusOption == ExchangeDocumentDirection.SentByOrganisation
                        && options.PageSize == TestConfiguration.ListPageSize
                        && options.OrganisationIdentifier == testOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier)))
                .ReturnsAsync(listResult);

            var expectedData = new DocumentListUpdateData
            {
                ListItems = GetTestSentDocuments(numberOfPages),
                Pagination = GetExpectedPaginationUpdateData(expectedNumberOfItems, numberOfPages)
            };

            var listHelper = Mock.Get(_listHelper);
            listHelper
                .Setup(l => l.GetFilterOptions(null, false))
                .Returns((IEnumerable<IFilterOption>)null);

            listHelper
                .Setup(l => l.GetPaginationViewModel(null, listResult))
                .Returns((PaginationViewModel)null);

            listHelper
                .Setup(l => l.GetFilterCategories(listResult))
                .Returns((IEnumerable<IFilterCategoryViewModel>)null);

            listHelper
                .Setup(l => l.AnyDocumentsAvailable(null, listResult))
                .Returns(expectedNumberOfItems > 0);

            listHelper
                .Setup(l => l.GetDocumentListUpdateData(It.IsAny<IListViewModel>()))
                .Returns(expectedData);

            SetUpDateTimeDisplayHelper(true);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.SentDocumentsData(null);

            // Assert
            result.Should().BeJsonResult()
                .Value.Should().BeOfType<DocumentListUpdateData>()
                .Which.Should().BeEquivalentTo(expectedData);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                mockExchangeClient,
                listHelper);

            //Mock.Get(Mapper),
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 91)]
        [DataRow(123, 43)]
        [DataRow(234, 64)]
        [DataRow(234, 82)]
        public async Task SentDocumentsData_ForChildOrganisation_MakesExpectedApiCallsAndReturnsExpectedJson(
            int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var testOrganisationUserInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentOrganisationIdentifier())
                .ReturnsAsync(testOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.CurrentUserShouldSeeParentView())
                .ReturnsAsync(false);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var listResult = new ListResult<ExchangeDocument>
            {
                Items = GetTestExchangeDocuments(numberOfPages, true),
                Filters = testServiceFilters,
                TotalPages = numberOfPages,
                TotalItems = expectedNumberOfItems
            };

            var mockExchangeClient = Mock.Get(ExchangeApiClient);
            mockExchangeClient
                .Setup(a => a.GetOrganisationDocuments(
                    It.Is<ExchangeListOrganisationDocumentOptions>(options =>
                        options.DocumentStatusOption == ExchangeDocumentDirection.SentByOrganisation
                        && options.PageSize == TestConfiguration.ListPageSize
                        && options.OrganisationIdentifier == testOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier)))
                .ReturnsAsync(listResult);

            var expectedData = new DocumentListUpdateData
            {
                ListItems = GetTestSentDocuments(numberOfPages),
                Pagination = GetExpectedPaginationUpdateData(expectedNumberOfItems, numberOfPages)
            };

            var listHelper = Mock.Get(_listHelper);
            listHelper
                .Setup(l => l.GetFilterOptions(null, false))
                .Returns((IEnumerable<IFilterOption>)null);

            listHelper
                .Setup(l => l.GetPaginationViewModel(null, listResult))
                .Returns((PaginationViewModel)null);

            listHelper
                .Setup(l => l.GetFilterCategories(listResult))
                .Returns((IEnumerable<IFilterCategoryViewModel>)null);

            listHelper
                .Setup(l => l.AnyDocumentsAvailable(null, listResult))
                .Returns(expectedNumberOfItems > 0);

            listHelper
                .Setup(l => l.GetDocumentListUpdateData(It.IsAny<IListViewModel>()))
                .Returns(expectedData);

            SetUpDateTimeDisplayHelper(true);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.SentDocumentsData(null);

            // Assert
            result.Should().BeJsonResult()
                .Value.Should().BeOfType<DocumentListUpdateData>()
                .Which.Should().BeEquivalentTo(expectedData);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                mockExchangeClient,
                listHelper);

            //Mock.Get(Mapper),
        }

        #endregion


        #region UploadDocument

        [TestMethod, TestCategory("Unit")]
        public void UploadDocument_ArgumentNullCheckTest()
        {
            // Arrange
            var controller = GetOrganisationController();

            // Act
            Func<Task> act = async () => await controller.UploadDocument(null);

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>();
            act.Should().ThrowAsync<ArgumentNullException>().Where(e => e.Message.Contains("Upload document request requires a value."));
        }

        [TestMethod, TestCategory("Unit")]
        public void UploadDocument_WhenUserOrgIsUnknown_ThrowsException()
        {
            // Arrange
            var testUserInfo = TestNoOrganisationUserInfo;
            var file = GetMockFile();
            SetUpSettingsApiClient();

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserInfo())
                .ReturnsAsync(testUserInfo);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.IsCurrentUserExternal())
                .Returns(true);

            var controller = GetOrganisationController();

            // Act
            Func<Task> act = async () => await controller.UploadDocument(new Models.Organisation.UploadDocumentRequest() { FileInput = file });

            // Assert
            act.Should().ThrowAsync<InvalidOperationException>();
            Mock.VerifyAll(
                Mock.Get(UserInformationProvider));
        }

        [TestMethod, TestCategory("Unit")]
        public async Task UploadDocument_WhenUserUploadDocument_UploadingForUkprnIsNull_ReturnOkTest()
        {
            // Arrange
            var file = GetMockFile();
            SetUpSettingsApiClient();

            var testUserInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserInfo())
                .ReturnsAsync(testUserInfo);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.IsCurrentUserExternal())
                .Returns(true);

            var apiMock = Mock.Get(_uploadApiClient);

            apiMock
                .Setup(d => d.UploadDocument(It.Is<Services.Models.UploadDocumentRequest>(udr =>
                    udr.FromOrganisation.Value == TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier.Value)))
                .Returns(Task.CompletedTask);

            var controller = GetOrganisationController();

            var request = new Models.Organisation.UploadDocumentRequest
            {
                FileInput = file,
                UploadingForUkprn = null,
                ProductIdentifier = 1123
            };

            // Act
            var result = await controller.UploadDocument(request);

            // Assert
            Mock.Verify(
                apiMock,
                Mock.Get(UserInformationProvider));

            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(OrganisationController.DocumentUploadComplete));
        }

        [TestMethod, TestCategory("Unit")]
        public void UploadDocument_WhenUserCannotUploadDocumentForUkprn_ThrowsExceptionTest()
        {
            // Arrange
            var testUserInfo = TestOrganisationUserInfo;
            SetUpSettingsApiClient();
            var file = GetMockFile();

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserInfo())
                .ReturnsAsync(testUserInfo);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.IsCurrentUserExternal())
                .Returns(true);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserChildOrganisations())
                .ReturnsAsync(Enumerable.Empty<Organisation>());

            var controller = GetOrganisationController();
            var request = new Models.Organisation.UploadDocumentRequest { UploadingForUkprn = "ukprn", FileInput = file };

            // Act
            Func<Task> result = async () => await controller.UploadDocument(request);

            // Assert
            result.Should().ThrowAsync<InvalidOperationException>("Cannot upload document for ukprn: ukprn.");
            Mock.Verify(
                Mock.Get(UserInformationProvider));
        }

        [TestMethod, TestCategory("Unit")]
        public async Task UploadDocument_WhenUserCanUploadDocumentForUkprn_ReturnOkTest()
        {
            // Arrange
            var mockUkprn = "10000000";
            var file = GetMockFile();
            SetUpSettingsApiClient();

            var testUserInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserInfo())
                .ReturnsAsync(testUserInfo);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.IsCurrentUserExternal())
                .Returns(true);

            var mockOrganisationId = new OrganisationIdentifier
            {
                Type = OrganisationIdentifierType.Ukprn,
                Value = mockUkprn
            };

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserChildOrganisations())
                .ReturnsAsync(new[]
                {
                    new Organisation
                    {
                        Identifiers = new[]
                        {
                            mockOrganisationId
                        }
                    }
                });

            var apiMock = Mock.Get(_uploadApiClient);

            apiMock
                .Setup(d => d.UploadDocument(It.Is<Services.Models.UploadDocumentRequest>(udr =>
                    udr.FromOrganisation == mockOrganisationId)))
                .Returns(Task.CompletedTask);

            var controller = GetOrganisationController();
            var request = new Models.Organisation.UploadDocumentRequest()
            {
                FileInput = file,
                UploadingForUkprn = mockUkprn,
                ProductIdentifier = 1123
            };

            // Act
            var result = await controller.UploadDocument(request);

            // Assert
            Mock.Verify(
                Mock.Get(UserInformationProvider),
                apiMock);

            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(OrganisationController.DocumentUploadComplete));
        }

        [TestMethod, TestCategory("Unit")]
        public async Task UploadDocument_WhenFileExtensionNotAllowed_RedirectsToSelectDocument()
        {
            // Arrange
            var mockUkprn = "10000000";
            var file = GetMockFile();

            Mock.Get(UserInformationProvider)
              .Setup(u => u.CurrentUserShouldSeeParentView())
              .ReturnsAsync(true);

            var mockOrganisationId = new OrganisationIdentifier
            {
                Type = OrganisationIdentifierType.Ukprn,
                Value = mockUkprn
            };

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.GetCurrentProductVersionForOrganisation(
                 It.IsAny<OrganisationIdentifier>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            Mock.Get(_settingsApiClient)
               .Setup(o => o.GetMaxFileUploadSize())
               .ReturnsAsync(1000);

            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetFileExtensions())
                .ReturnsAsync(Enumerable.Empty<FileExtensionInfo>());

            var controller = GetOrganisationController();
            var request = new Models.Organisation.UploadDocumentRequest()
            {
                FileInput = file,
                UploadingForUkprn = mockUkprn,
                ProductIdentifier = 1123
            };

            // Act
            var result = await controller.UploadDocument(request) as ViewResult;

            // Assert
            Assert.AreEqual(nameof(controller.SelectDocument), result.ViewName);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(ExchangeApiClient),
                Mock.Get(_settingsApiClient));
        }

        [TestMethod, TestCategory("Unit")]
        public async Task UploadDocument_WhenFileExtensionNotAllowedForUser_RedirectsToSelectDocument()
        {
            // Arrange
            var mockUkprn = "10000000";
            var file = GetMockFile();

            Mock.Get(UserInformationProvider)
              .Setup(u => u.CurrentUserShouldSeeParentView())
              .ReturnsAsync(true);

            Mock.Get(UserInformationProvider)
              .Setup(u => u.IsCurrentUserExternal())
              .Returns(true);

            var mockOrganisationId = new OrganisationIdentifier
            {
                Type = OrganisationIdentifierType.Ukprn,
                Value = mockUkprn
            };

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.GetCurrentProductVersionForOrganisation(
                 It.IsAny<OrganisationIdentifier>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            Mock.Get(_settingsApiClient)
               .Setup(o => o.GetMaxFileUploadSize())
               .ReturnsAsync(1000);

            Mock.Get(_settingsApiClient)
               .Setup(o => o.GetFileExtensions())
               .ReturnsAsync(new[]
               {
                    new FileExtensionInfo
                    {
                        Extension = "pdf",
                        Identifier = "pdf",
                        CanExternalUserUpload = false
                    }
               });

            var controller = GetOrganisationController();
            var request = new Models.Organisation.UploadDocumentRequest()
            {
                FileInput = file,
                UploadingForUkprn = mockUkprn,
                ProductIdentifier = 1123
            };

            // Act
            var result = await controller.UploadDocument(request) as ViewResult;

            // Assert
            Assert.AreEqual(nameof(controller.SelectDocument), result.ViewName);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(ExchangeApiClient),
                Mock.Get(_settingsApiClient));
        }

        [TestMethod, TestCategory("Unit")]
        public async Task UploadDocument_WhenFileSizeNotValid_RedirectsToSelectDocument()
        {
            // Arrange
            var mockUkprn = "10000000";
            var file = GetMockFile();

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.GetCurrentProductVersionForOrganisation(
                It.IsAny<OrganisationIdentifier>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            Mock.Get(UserInformationProvider)
              .Setup(u => u.CurrentUserShouldSeeParentView())
              .ReturnsAsync(true);

            var mockOrganisationId = new OrganisationIdentifier
            {
                Type = OrganisationIdentifierType.Ukprn,
                Value = mockUkprn
            };

            Mock.Get(_settingsApiClient)
               .Setup(o => o.GetMaxFileUploadSize())
               .ReturnsAsync(1000);

            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetFileExtensions())
                .ReturnsAsync(Enumerable.Empty<FileExtensionInfo>());

            var controller = GetOrganisationController();
            var request = new Models.Organisation.UploadDocumentRequest()
            {
                FileInput = file,
                UploadingForUkprn = mockUkprn,
                ProductIdentifier = 1123
            };

            // Act
            var result = await controller.UploadDocument(request) as ViewResult;

            // Assert
            Assert.AreEqual(nameof(controller.SelectDocument), result.ViewName);

            Mock.VerifyAll(
              Mock.Get(UserInformationProvider),
              Mock.Get(ExchangeApiClient),
              Mock.Get(_settingsApiClient));
        }

        #endregion


        #region DownloadExternalExchangeDocument

        [TestMethod, TestCategory("Unit")]
        [DataRow("test content 1")]
        [DataRow("test content 2")]
        [DataRow("test content 3")]
        public async Task DownloadExternalExchangeDocument_ReturnsExpectedFileResult(string fileContent)
        {
            // Arrange
            var userInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserInfo())
                .ReturnsAsync(userInfo);

            var fakeFileName = "fakeFileName.pdf";
            var fakeDocumentReference = $"{fakeFileName}|fakeBatchIdentifier|fakeParentBatchIdentifier";

            var fileContentBytes = Encoding.UTF8.GetBytes(fileContent);

            var file = new Services.Models.DownloadedFile
            {
                Name = fakeFileName,
                ContentType = FakeMimeTypeString,
                Content = fileContentBytes
            };

            Mock.Get(ExchangeDocumentDownloadService)
                .Setup(a => a.DownloadExchangeDocumentByReference(fakeDocumentReference, userInfo))
                .ReturnsAsync(file);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.DownloadExternalExchangeDocument(fakeDocumentReference);

            // Assert
            result.Should().BeFileContentResult()
                .WithFileDownloadName(fakeFileName)
                .WithContentType(FakeMimeTypeString);

            (result as FileContentResult).FileContents.Should().BeEquivalentTo(fileContentBytes);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(ExchangeDocumentDownloadService));
        }

        #endregion


        #region ReceivedDocuments

        [TestMethod, TestCategory("Unit")]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 91)]
        [DataRow(123, 43)]
        [DataRow(234, 64)]
        [DataRow(234, 82)]
        public async Task ReceivedDocuments_ForParentOrganisation_MakesExpectedApiCallsAndReturnsExpectedView(
            int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var testOrganisationUserInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentOrganisationIdentifier())
                .ReturnsAsync(testOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.CurrentUserShouldSeeParentView())
                .ReturnsAsync(true);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();
            var testViewModelFilters = GetTestViewModelFilterCategories(numberOfFilters).ToList();

            //SetupFilterMapper(testServiceFilters, testViewModelFilters);
            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var listResult = new ListResult<ExchangeDocument>
            {
                Items = GetTestExchangeDocuments(numberOfPages, false),
                Filters = testServiceFilters,
                TotalPages = numberOfPages,
                TotalItems = expectedNumberOfItems
            };

            var mockExchangeClient = Mock.Get(ExchangeApiClient);
            mockExchangeClient
                .Setup(a => a.GetOrganisationDocuments(
                    It.Is<ExchangeListOrganisationDocumentOptions>(options =>
                        options.DocumentStatusOption == ExchangeDocumentDirection.PublishedByAgency
                        && options.PageSize == TestConfiguration.ListPageSize)))
                .ReturnsAsync(listResult);

            var documentStatusProvider = Mock.Get(DocumentStatusProvider);

            documentStatusProvider
                .Setup(d => d.GetDownloadStatus(It.IsAny<IEnumerable<ExchangeDocumentEvent>>()))
                .Returns(new DocumentStatusInfo
                {
                    Status = DownloadDocumentStatus.Downloaded,
                    DownloadedBy = "the-user-who-downloaded-the-document",
                    DownloadedTime = new DateTime(2020, 9, 1)
                });

            var expectedPagination = new PaginationViewModel
            {
                TotalItems = expectedNumberOfItems,
                TotalPages = numberOfPages,
                Page = 1,
                PageSize = TestConfiguration.ListPageSize
            };

            var listHelper = Mock.Get(_listHelper);
            listHelper
                .Setup(l => l.GetFilterOptions(null, false))
                .Returns((IEnumerable<IFilterOption>)null);

            listHelper
                .Setup(l => l.GetPaginationViewModel(null, listResult))
                .Returns(expectedPagination);

            listHelper
                .Setup(l => l.GetFilterCategories(listResult))
                .Returns(testViewModelFilters);

            listHelper
                .Setup(l => l.AnyDocumentsAvailable(null, listResult))
                .Returns(expectedNumberOfItems > 0);

            SetUpDateTimeDisplayHelper(false);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.ReceivedDocuments(null);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<ReceivedDocuments>()
                .Which.Should().BeEquivalentTo(
                    new ReceivedDocuments
                    {
                        ShowParentView = true,
                        ListItems = GetTestReceivedDocuments(numberOfPages),
                        FilterCategories = testViewModelFilters,
                        Pagination = expectedPagination,
                        AnyDocumentsAvailable = expectedNumberOfItems > 0
                    }, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.Verify(
                Mock.Get(UserInformationProvider),
                mockExchangeClient,
                documentStatusProvider,
                listHelper);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 91)]
        [DataRow(123, 43)]
        [DataRow(234, 64)]
        [DataRow(234, 82)]
        public async Task ReceivedDocuments_ForChildOrganisation_MakesExpectedApiCallsAndReturnsExpectedView(
            int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var testOrganisationUserInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentOrganisationIdentifier())
                .ReturnsAsync(testOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.CurrentUserShouldSeeParentView())
                .ReturnsAsync(false);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();
            var testViewModelFilters = GetTestViewModelFilterCategories(numberOfFilters).ToList();

            //SetupFilterMapper(testServiceFilters, testViewModelFilters);
            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var listResult = new ListResult<ExchangeDocument>
            {
                Items = GetTestExchangeDocuments(numberOfPages, false),
                Filters = testServiceFilters,
                TotalPages = numberOfPages,
                TotalItems = expectedNumberOfItems
            };

            var mockExchangeClient = Mock.Get(ExchangeApiClient);
            mockExchangeClient
                .Setup(a => a.GetOrganisationDocuments(
                    It.Is<ExchangeListOrganisationDocumentOptions>(options =>
                        options.DocumentStatusOption == ExchangeDocumentDirection.PublishedByAgency
                        && options.PageSize == TestConfiguration.ListPageSize)))
                .ReturnsAsync(listResult);

            var documentStatusProvider = Mock.Get(DocumentStatusProvider);

            if (numberOfPages > 0)
            {
                documentStatusProvider
                    .Setup(d => d.GetDownloadStatus(It.IsAny<IEnumerable<ExchangeDocumentEvent>>()))
                    .Returns(new DocumentStatusInfo
                    {
                        Status = DownloadDocumentStatus.Downloaded,
                        DownloadedBy = "the-user-who-downloaded-the-document",
                        DownloadedTime = new DateTime(2020, 9, 1)
                    });
            }

            var expectedPagination = new PaginationViewModel
            {
                TotalItems = expectedNumberOfItems,
                TotalPages = numberOfPages,
                Page = 1,
                PageSize = TestConfiguration.ListPageSize
            };

            var listHelper = Mock.Get(_listHelper);
            listHelper
                .Setup(l => l.GetFilterOptions(null, false))
                .Returns((IEnumerable<IFilterOption>)null);

            listHelper
                .Setup(l => l.GetPaginationViewModel(null, listResult))
                .Returns(expectedPagination);

            listHelper
                .Setup(l => l.GetFilterCategories(listResult))
                .Returns(testViewModelFilters);

            listHelper
                .Setup(l => l.AnyDocumentsAvailable(null, listResult))
                .Returns(expectedNumberOfItems > 0);

            SetUpDateTimeDisplayHelper(false);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.ReceivedDocuments(null);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<ReceivedDocuments>()
                .Which.Should().BeEquivalentTo(
                    new ReceivedDocuments
                    {
                        ShowParentView = false,
                        ListItems = GetTestReceivedDocuments(numberOfPages),
                        FilterCategories = testViewModelFilters,
                        Pagination = expectedPagination,
                        AnyDocumentsAvailable = expectedNumberOfItems > 0
                    }, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                mockExchangeClient,
                documentStatusProvider,
                listHelper);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 91)]
        [DataRow(123, 43)]
        [DataRow(234, 64)]
        [DataRow(234, 82)]
        public async Task ReceivedDocuments_WhenErrorDocumentsRequestHasError_MakesExpectedApiCallsAndReturnsExpectedViewWithError(
             int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var testOrganisationUserInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentOrganisationIdentifier())
                .ReturnsAsync(testOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.CurrentUserShouldSeeParentView())
                .ReturnsAsync(true);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();
            var testViewModelFilters = GetTestViewModelFilterCategories(numberOfFilters).ToList();

            // SetupFilterMapper(testServiceFilters, testViewModelFilters);
            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var listResult = new ListResult<ExchangeDocument>
            {
                Items = GetTestExchangeDocuments(numberOfPages, false),
                Filters = testServiceFilters,
                TotalPages = numberOfPages,
                TotalItems = expectedNumberOfItems
            };

            var mockExchangeClient = Mock.Get(ExchangeApiClient);
            mockExchangeClient
                .Setup(a => a.GetOrganisationDocuments(
                    It.Is<ExchangeListOrganisationDocumentOptions>(options =>
                        options.DocumentStatusOption == ExchangeDocumentDirection.PublishedByAgency
                        && options.PageSize == TestConfiguration.ListPageSize)))
                .ReturnsAsync(listResult);

            var documentStatusProvider = Mock.Get(DocumentStatusProvider);

            if (numberOfPages > 0)
            {
                documentStatusProvider
                    .Setup(d => d.GetDownloadStatus(It.IsAny<IEnumerable<ExchangeDocumentEvent>>()))
                    .Returns(new DocumentStatusInfo
                    {
                        Status = DownloadDocumentStatus.Downloaded,
                        DownloadedBy = "the-user-who-downloaded-the-document",
                        DownloadedTime = new DateTime(2020, 9, 1)
                    });
            }

            var expectedPagination = new PaginationViewModel
            {
                TotalItems = expectedNumberOfItems,
                TotalPages = numberOfPages,
                Page = 1,
                PageSize = TestConfiguration.ListPageSize
            };

            var documentsRequestWithError = new DocumentsRequest
            {
                Error = true,
                ErrorAction = "the-error-action"
            };

            var listHelper = Mock.Get(_listHelper);
            listHelper
                .Setup(l => l.GetFilterOptions(documentsRequestWithError, false))
                .Returns((IEnumerable<IFilterOption>)null);

            listHelper
                .Setup(l => l.GetPaginationViewModel(documentsRequestWithError, listResult))
                .Returns(expectedPagination);

            listHelper
                .Setup(l => l.GetFilterCategories(listResult))
                .Returns(testViewModelFilters);

            listHelper
                .Setup(l => l.AnyDocumentsAvailable(documentsRequestWithError, listResult))
                .Returns(expectedNumberOfItems > 0);

            SetUpDateTimeDisplayHelper(false);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.ReceivedDocuments(documentsRequestWithError);

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<ReceivedDocuments>()
                .Which.Should().BeEquivalentTo(
                    new ReceivedDocuments
                    {
                        ShowParentView = true,
                        ListItems = GetTestReceivedDocuments(numberOfPages),
                        FilterCategories = testViewModelFilters,
                        Pagination = expectedPagination,
                        AnyDocumentsAvailable = expectedNumberOfItems > 0,
                        Error = true,
                        ErrorAction = documentsRequestWithError.ErrorAction
                    }, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                mockExchangeClient,
                documentStatusProvider,
                listHelper);
        }

        #endregion


        #region ReceivedDocumentsData

        [TestMethod, TestCategory("Unit")]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 91)]
        [DataRow(123, 43)]
        [DataRow(234, 64)]
        [DataRow(234, 82)]
        public async Task ReceivedDocumentsData_ForParentOrganisation_MakesExpectedApiCallsAndReturnsExpectedView(
            int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var testOrganisationUserInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentOrganisationIdentifier())
                .ReturnsAsync(testOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.CurrentUserShouldSeeParentView())
                .ReturnsAsync(true);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var listResult = new ListResult<ExchangeDocument>
            {
                Items = GetTestExchangeDocuments(numberOfPages, false),
                Filters = testServiceFilters,
                TotalPages = numberOfPages,
                TotalItems = expectedNumberOfItems
            };

            var mockExchangeClient = Mock.Get(ExchangeApiClient);
            mockExchangeClient
                .Setup(a => a.GetOrganisationDocuments(
                    It.Is<ExchangeListOrganisationDocumentOptions>(options =>
                        options.DocumentStatusOption == ExchangeDocumentDirection.PublishedByAgency
                        && options.PageSize == TestConfiguration.ListPageSize)))
                .ReturnsAsync(listResult);

            var documentStatusProvider = Mock.Get(DocumentStatusProvider);

            if (numberOfPages > 0)
            {
                documentStatusProvider
                    .Setup(d => d.GetDownloadStatus(It.IsAny<IEnumerable<ExchangeDocumentEvent>>()))
                    .Returns(new DocumentStatusInfo
                    {
                        Status = DownloadDocumentStatus.Downloaded,
                        DownloadedBy = "the-user-who-downloaded-the-document",
                        DownloadedTime = new DateTime(2020, 9, 1)
                    });
            }

            var expectedData = new DocumentListUpdateData
            {
                ListItems = GetTestReceivedDocuments(numberOfPages),
                Pagination = GetExpectedPaginationUpdateData(expectedNumberOfItems, numberOfPages)
            };

            var listHelper = Mock.Get(_listHelper);
            listHelper
                .Setup(l => l.GetFilterOptions(null, false))
                .Returns((IEnumerable<IFilterOption>)null);

            listHelper
                .Setup(l => l.GetPaginationViewModel(null, listResult))
                .Returns((PaginationViewModel)null);

            listHelper
                .Setup(l => l.GetFilterCategories(listResult))
                .Returns((IEnumerable<IFilterCategoryViewModel>)null);

            listHelper
                .Setup(l => l.AnyDocumentsAvailable(null, listResult))
                .Returns(expectedNumberOfItems > 0);

            listHelper
                .Setup(l => l.GetDocumentListUpdateData(It.IsAny<IListViewModel>()))
                .Returns(expectedData);

            SetUpDateTimeDisplayHelper(false);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.ReceivedDocumentsData(null);

            // Assert
            result.Should().BeJsonResult()
                .Value.Should().BeOfType<DocumentListUpdateData>()
                .Which.Should().BeEquivalentTo(expectedData);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                mockExchangeClient,
                documentStatusProvider,
                listHelper);
        }

        [TestMethod, TestCategory("Unit")]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(12, 91)]
        [DataRow(123, 43)]
        [DataRow(234, 64)]
        [DataRow(234, 82)]
        public async Task ReceivedDocumentsData_ForChildOrganisation_MakesExpectedApiCallsAndReturnsExpectedView(
            int numberOfPages, int numberOfFilters)
        {
            // Arrange
            var testOrganisationUserInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentOrganisationIdentifier())
                .ReturnsAsync(testOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);

            Mock.Get(UserInformationProvider)
                .Setup(u => u.CurrentUserShouldSeeParentView())
                .ReturnsAsync(false);

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var listResult = new ListResult<ExchangeDocument>
            {
                Items = GetTestExchangeDocuments(numberOfPages, false),
                Filters = testServiceFilters,
                TotalPages = numberOfPages,
                TotalItems = expectedNumberOfItems
            };

            var mockExchangeClient = Mock.Get(ExchangeApiClient);
            mockExchangeClient
                .Setup(a => a.GetOrganisationDocuments(
                    It.Is<ExchangeListOrganisationDocumentOptions>(options =>
                        options.DocumentStatusOption == ExchangeDocumentDirection.PublishedByAgency
                        && options.PageSize == TestConfiguration.ListPageSize)))
                .ReturnsAsync(listResult);

            var documentStatusProvider = Mock.Get(DocumentStatusProvider);

            if (numberOfPages > 0)
            {
                documentStatusProvider
                    .Setup(d => d.GetDownloadStatus(It.IsAny<IEnumerable<ExchangeDocumentEvent>>()))
                    .Returns(new DocumentStatusInfo
                    {
                        Status = DownloadDocumentStatus.Downloaded,
                        DownloadedBy = "the-user-who-downloaded-the-document",
                        DownloadedTime = new DateTime(2020, 9, 1)
                    });
            }

            var expectedData = new DocumentListUpdateData
            {
                ListItems = GetTestReceivedDocuments(numberOfPages),
                Pagination = GetExpectedPaginationUpdateData(expectedNumberOfItems, numberOfPages)
            };

            var listHelper = Mock.Get(_listHelper);
            listHelper
                .Setup(l => l.GetFilterOptions(null, false))
                .Returns((IEnumerable<IFilterOption>)null);

            listHelper
                .Setup(l => l.GetPaginationViewModel(null, listResult))
                .Returns((PaginationViewModel)null);

            listHelper
                .Setup(l => l.GetFilterCategories(listResult))
                .Returns((IEnumerable<IFilterCategoryViewModel>)null);

            listHelper
                .Setup(l => l.AnyDocumentsAvailable(null, listResult))
                .Returns(expectedNumberOfItems > 0);

            listHelper
                .Setup(l => l.GetDocumentListUpdateData(It.IsAny<IListViewModel>()))
                .Returns(expectedData);

            SetUpDateTimeDisplayHelper(false);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.ReceivedDocumentsData(null);

            // Assert
            result.Should().BeJsonResult()
                .Value.Should().BeOfType<DocumentListUpdateData>()
                .Which.Should().BeEquivalentTo(expectedData);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                mockExchangeClient,
                documentStatusProvider);
        }

        #endregion


        #region ReceivedDocumentsDownload

        [TestMethod, TestCategory("Unit")]
        public async Task ReceivedDocumentsDownload_WhenDocumentReferenceListIsEmpty_RedirectsToReceivedDocumentsAction()
        {
            // Arrange
            var documentReferenceList = new DocumentReferenceList
            {
                DocumentReferences = null
            };

            var controller = GetOrganisationController();

            // Act
            var result = await controller.ReceivedDocumentsDownload(documentReferenceList);

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(OrganisationController.ReceivedDocuments))
                .WithControllerName(NameOf<OrganisationController>())
                .WithRouteValue("error", true)
                .WithRouteValue("errorAction", nameof(OrganisationController.ReceivedDocumentsDownload));
        }

        [TestMethod, TestCategory("Unit")]
        public async Task ReceivedDocumentsDownload_WhenDocumentReferenceListContainsItems_DownloadsFile()
        {
            // Arrange
            var userInfo = TestOrganisationUserInfo;

            Mock.Get(UserInformationProvider)
                .Setup(u => u.GetCurrentUserInfo())
                .ReturnsAsync(userInfo);

            var fileName = "fileName.pdf";
            var documentReference = $"{fileName}|batchIdentifier|parentBatchIdentifier";
            var documentReferences = new[] { documentReference };

            var documentReferenceList = new DocumentReferenceList
            {
                DocumentReferences = documentReferences
            };

            var fileContentBytes = new byte[] { 1, 2, 3, 4, 5 };

            var file = new Services.Models.DownloadedFile
            {
                Name = fileName,
                ContentType = FakeMimeTypeString,
                Content = fileContentBytes
            };

            Mock.Get(ExchangeDocumentDownloadService)
                .Setup(a => a.DownloadExchangeDocumentsByReferences(documentReferences, userInfo))
                .ReturnsAsync(file);

            var controller = GetOrganisationController();

            // Act
            var result = await controller.ReceivedDocumentsDownload(documentReferenceList);

            // Assert
            result.Should().BeFileContentResult()
                .WithFileDownloadName(fileName)
                .WithContentType(FakeMimeTypeString);

            (result as FileContentResult).FileContents.Should().BeEquivalentTo(fileContentBytes);

            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(ExchangeDocumentDownloadService));
        }

        #endregion

        private IEnumerable<ReceivedDocument> GetTestReceivedDocuments(
            int numberOfPages, int pageNumber = 1)
        {
            if (numberOfPages == 0)
            {
                return Enumerable.Empty<ReceivedDocument>();
            }

            return Enumerable
                .Range(1, numberOfPages)
                .Select(p => Enumerable
                    .Range(((p - 1) * TestConfiguration.ListPageSize) + 1, TestConfiguration.ListPageSize)
                    .Select(d => new ReceivedDocument
                    {
                        FileName = $"file{d}",
                        ProductName = $"product{p}",
                        RecipientOrganisationName = $"school {d}",
                        SenderName = (p % 2) == 0 ? "firstname lastname" : "principal",
                        ReceivedDateTime = new DateTime(2020, 12, 31, p % 24, d % 60, 59),
                        Version = d
                    })).ElementAt(pageNumber - 1);
        }

        private IFormFile GetMockFile()
        {
            // Setup mock file using a memory stream
            var fileMock = new Mock<IFormFile>();
            var content = "Hello World";
            var fileName = "test.pdf";
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(content);
            writer.Flush();
            memoryStream.Position = 0;
            fileMock.Setup(f => f.OpenReadStream()).Returns(memoryStream);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(memoryStream.Length);
            return fileMock.Object;
        }

        private OrganisationController GetOrganisationController()
        {
            return new OrganisationController(
                UserInformationProvider,
                MimeMappingService,
                Options.Create(TestConfiguration),
                ExchangeApiClient,
                _uploadApiClient,
                _settingsApiClient,
                ExchangeDocumentDownloadService,
                DocumentStatusProvider,
                logger: _logger,
                _dateTimeDisplayHelper,
                _listHelper);
        }

        private UserInfo TestOrganisationUserInfo
            => new UserInfo
            {
                FullName = "Fake user",
                Principal = "fakeuser12345678",
                EmailAddress = "organisationuser@fake.com",
                OrganisationInfo = new OrganisationInfo
                {
                    OrganisationIdentifier = new OrganisationIdentifier
                    {
                        Value = "12345678",
                        Type = OrganisationIdentifierType.Ukprn
                    }
                }
            };

        private UserInfo TestOrganisationUserInfoWithNoOrganisationIdentifier
            => new UserInfo
            {
                FullName = "Fake user",
                Principal = "fakeuser12345678",
                EmailAddress = "organisationuser@fake.com",
                OrganisationInfo = new OrganisationInfo
                {
                    Name = "organisation-with-no-identifier",
                    OrganisationIdentifier = null
                }
            };

        private Organisation GetTestChildOrganisation(int number = 1)
            => new Organisation
            {
                Name = $"child org {number}",
                Identifiers = new List<OrganisationIdentifier>
                {
                    new OrganisationIdentifier
                    {
                        Type = OrganisationIdentifierType.Ukprn,
                        Value = $"childOrg{number}ukprn"
                    }
                }
            };

        private Organisation GetTestParentOrganisation(int numberOfChildren)
            => new Organisation
            {
                Name = "parent org",
                Identifiers = new List<OrganisationIdentifier>
                {
                    new OrganisationIdentifier
                    {
                        Type = OrganisationIdentifierType.Ukprn,
                        Value = $"parentOrgUkprn"
                    }
                },
                ChildOrganisations = Enumerable
                    .Range(1, numberOfChildren)
                    .Select(n => GetTestChildOrganisation(n))
            };

        private IEnumerable<Product> GetTestProducts(int numberOfProducts)
            => Enumerable
                .Range(1, numberOfProducts)
                .Select(p => new Product
                {
                    Identifier = 10000 + p,
                    Name = $"product {10000 + p}"
                });

        private IEnumerable<ExchangeDocument> GetTestExchangeDocuments(
            int numberOfPages, bool isSentByOrganisation, int pageNumber = 1)
        {
            if (numberOfPages == 0)
            {
                return Enumerable.Empty<ExchangeDocument>();
            }

            return Enumerable
                .Range(1, numberOfPages)
                .Select(p => Enumerable
                    .Range(((p - 1) * TestConfiguration.ListPageSize) + 1, TestConfiguration.ListPageSize)
                    .Select(d =>
                    {
                        var exchangeDocument = new ExchangeDocument
                        {
                            DocumentReference = new DocumentReference
                            {
                                FileName = $"file{d}"
                            },
                            ExchangeDirection = isSentByOrganisation
                            ? ExchangeDocumentDirection.SentByOrganisation
                            : ExchangeDocumentDirection.PublishedByAgency,
                            Product = new Product
                            {
                                Identifier = 10000 + p,
                                Name = $"product{p}"
                            },
                            Year = 201920,
                            OrganisationInfo = new OrganisationInfo
                            {
                                OrganisationIdentifier = new OrganisationIdentifier
                                {
                                    Type = OrganisationIdentifierType.Ukprn,
                                    Value = $"{10000000 + d}"
                                },
                                Name = $"school {d}"
                            },
                            EventHistory = new List<ExchangeDocumentEvent>
                            {
                                new ExchangeDocumentEvent
                                {
                                    EventType = ExchangeDocumentEventType.SentByOrganisation,
                                    EventDateTime = new DateTime(2020, 12, 31, p % 24, d % 60, 59),
                                    UserInfo = new UserInfo
                                    {
                                        FullName = (p % 2) == 0 ? "firstname lastname" : null,
                                        Principal = "principal"
                                    }
                                }
                            },
                            Version = d
                        };

                        var previousVersions = CreateTestExchangeDocumentPreviousVersions(exchangeDocument);
                        exchangeDocument.PreviousVersions = previousVersions;

                        return exchangeDocument;
                    })).ElementAt(pageNumber - 1);
        }

        private IEnumerable<ExchangeDocument> CreateTestExchangeDocumentPreviousVersions(ExchangeDocument latestExchangeDocumentVersion)
        {
            return Enumerable.Range(1, latestExchangeDocumentVersion.Version - 1)
                .Select(version => new ExchangeDocument
                {
                    DocumentReference = latestExchangeDocumentVersion.DocumentReference,
                    ExchangeDirection = latestExchangeDocumentVersion.ExchangeDirection,
                    Product = latestExchangeDocumentVersion.Product,
                    Year = latestExchangeDocumentVersion.Year,
                    OrganisationInfo = latestExchangeDocumentVersion.OrganisationInfo,
                    Version = version
                })
                .OrderByDescending(exchangeDocument => exchangeDocument.Version);
        }

        private IEnumerable<SentDocument> GetTestSentDocuments(
            int numberOfPages, int pageNumber = 1)
        {
            if (numberOfPages == 0)
            {
                return Enumerable.Empty<SentDocument>();
            }

            return Enumerable
                .Range(1, numberOfPages)
                .Select(p => Enumerable
                    .Range(((p - 1) * TestConfiguration.ListPageSize) + 1, TestConfiguration.ListPageSize)
                    .Select(d =>
                    {
                        var sentDocument = new SentDocument
                        {
                            FileName = $"file{d}",
                            ProductName = $"product{p}",
                            SenderOrganisationName = $"school {d}",
                            SenderName = (p % 2) == 0 ? "firstname lastname" : "principal",
                            SentDateTime = new DateTime(2020, 12, 31, p % 24, d % 60, 59),
                            Version = d
                        };

                        var previousVersions = CreateTestSentDocumentPreviousVersions(sentDocument);
                        sentDocument.PreviousVersions = previousVersions;

                        return sentDocument;
                    })).ElementAt(pageNumber - 1);
        }

        private IEnumerable<SentDocument> CreateTestSentDocumentPreviousVersions(SentDocument latestSentDocumentVersion)
        {
            return Enumerable.Range(1, latestSentDocumentVersion.Version - 1)
                .Select(version => new SentDocument
                {
                    FileName = latestSentDocumentVersion.FileName,
                    ProductName = latestSentDocumentVersion.ProductName,
                    SenderOrganisationName = latestSentDocumentVersion.SenderOrganisationName,
                    SenderName = latestSentDocumentVersion.SenderName,
                    SentDateTime = latestSentDocumentVersion.SentDateTime,
                    Version = version
                })
                .OrderByDescending(exchangeDocument => exchangeDocument.Version);
        }

        private void SetUpDateTimeDisplayHelper(bool sentenceDisplay)
        {
            var mock = Mock.Get(_dateTimeDisplayHelper);

            if (sentenceDisplay)
            {
                mock
                    .Setup(h => h.ToSentenceTimeAndDateDisplayString(It.IsAny<DateTime>()))
                    .Returns("my date");
            }
            else
            {
                mock
                    .Setup(h => h.ToTimeAndDateDisplayString(It.IsAny<DateTime>()))
                    .Returns("my date");
            }
        }

        private void SetUpSettingsApiClient()
        {
            SetupSettingsApiClientGetMaxFileUploadSize();
            SetupSettingsApiClientGetFileExtensions();
        }

        private void SetupSettingsApiClientGetMaxFileUploadSize()
        {
            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetMaxFileUploadSize())
                .ReturnsAsync(1000);
        }

        private void SetupSettingsApiClientGetFileExtensions()
        {
            Mock.Get(_settingsApiClient)
                 .Setup(o => o.GetFileExtensions())
                 .ReturnsAsync(GetFileExtensionInfo());
        }

        private List<FileExtensionInfo> GetFileExtensionInfo()
        {
            return new List<FileExtensionInfo> { new FileExtensionInfo { Extension = "pdf", CanExternalUserUpload = true } };
        }
    }
}