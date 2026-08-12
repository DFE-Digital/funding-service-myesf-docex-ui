using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Common.Organisation.Enums;
using Pds.Core.Common.Organisation.Models;
using Pds.Core.Web.Components.Areas.Lists.Builders;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Implementations;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Helpers;
using Pds.DocumentExchange.Web.Implementations.Helpers;
using Pds.DocumentExchange.Web.Implementations.Providers;
using Pds.DocumentExchange.Web.Models.Organisation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Tests.Integration
{
    [TestClass, TestCategory("Integration")]
    public class OrganisationControllerTests : BaseControllerIntegrationTests
    {
        private const string FakeMimeTypeString = "application/pdf";

        private readonly ISettingsApiClient _settingsApiClient
            = Mock.Of<ISettingsApiClient>(MockBehavior.Strict);

        private readonly IUploadApiClient _uploadApiClient
            = Mock.Of<IUploadApiClient>(MockBehavior.Strict);

        private readonly IOrganisationApiClient _organisationApiClient
            = Mock.Of<IOrganisationApiClient>(MockBehavior.Strict);

        #region Home

        [TestMethod]
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
            UserInfo actualUserInfo = null;
            OrganisationIdentifier actualOrganisationIdentifier = null;
            SetupUserIdentity(TestOrganisationUser);

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.GetOrganisationUserSummary(It.IsAny<UserInfo>()))
                .ReturnsAsync((UserInfo userInfo) =>
                {
                    actualUserInfo = userInfo;
                    return new Summary { CountOfNewDocuments = newDocuments };
                });

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
                {
                    actualOrganisationIdentifier = organisationIdentifier;
                    return new Organisation
                    {
                        ChildOrganisations = new List<Organisation>
                        {
                            new Organisation { }
                        }
                    };
                });

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.Home();

            // Assert
            actualUserInfo.Should().BeEquivalentTo(testOrganisationUserInfo);
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
            result.Should().BeViewResult()
                .Model.Should().BeOfType<Home>()
                .Which.Should().BeEquivalentTo(
                    new Home
                    {
                        CountOfNewDocuments = newDocuments ?? 0,
                        SendNewDocumentActionName = nameof(OrganisationController.SelectOrganisation)
                    });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(ExchangeApiClient));
        }

        [TestMethod]
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
            UserInfo actualUserInfo = null;
            OrganisationIdentifier actualOrganisationIdentifier = null;
            SetupUserIdentity(TestOrganisationUser);

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.GetOrganisationUserSummary(It.IsAny<UserInfo>()))
                .ReturnsAsync((UserInfo userInfo) =>
                {
                    actualUserInfo = userInfo;
                    return new Summary { CountOfNewDocuments = newDocuments };
                });

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
                {
                    actualOrganisationIdentifier = organisationIdentifier;
                    return new Organisation();
                });

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.Home();

            // Assert
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
            result.Should().BeViewResult()
                .Model.Should().BeOfType<Home>()
                .Which.Should().BeEquivalentTo(
                    new Home
                    {
                        CountOfNewDocuments = newDocuments ?? 0,
                        SendNewDocumentActionName = nameof(OrganisationController.SelectDocumentType)
                    });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(ExchangeApiClient));
        }

        #endregion


        #region SelectOrganisation

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task SelectOrganisation_ForParentOrganisationUser_ReturnsTheView(bool error)
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);
            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
                {
                    actualOrganisationIdentifier = organisationIdentifier;
                    return new Organisation
                    {
                        ChildOrganisations = new List<Organisation>
                        {
                            new Organisation { }
                        }
                    };
                });

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.SelectOrganisation(error);

            // Assert
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(new SelectOrganisation { Error = error });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(_organisationApiClient));
        }

        [TestMethod]
        public async Task SelectOrganisation_ForChildOrganisationUser_RedirectsToSelectDocumentType()
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);
            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
                {
                    actualOrganisationIdentifier = organisationIdentifier;
                    return new Organisation();
                });

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.SelectOrganisation(false);

            // Assert
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(OrganisationController.SelectDocumentType));

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(_organisationApiClient));
        }

        #endregion


        #region SelectAcademy

        [TestMethod]
        public async Task SelectAcademy_ForChildOrganisationUser_RedirectsToSelectDocumentType()
        {
            // Arrange
            OrganisationIdentifier actualOrganisationIdentifier = null;
            SetupUserIdentity(TestOrganisationUser);

            Mock.Get(_organisationApiClient)
              .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
              .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
              {
                  actualOrganisationIdentifier = organisationIdentifier;
                  return new Organisation();
              });

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.SelectAcademy(null);

            // Assert
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(OrganisationController.SelectDocumentType));

            Mock.VerifyAll(
              Mock.Get(IdentityService),
              Mock.Get(ExchangeApiClient),
              Mock.Get(_organisationApiClient));
        }

        [TestMethod]
        public async Task SelectAcademy_ForNoOptionSelected_RedirectsToSelectOrganisation()
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.SelectAcademy(new SelectAcademy());

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithPreserveMethod(true)
                .WithActionName(nameof(OrganisationController.SelectOrganisation))
                .WithControllerName(NameOf<OrganisationController>())
                .WithRouteValue("error", true);

            Mock.VerifyAll(
              Mock.Get(IdentityService),
              Mock.Get(ExchangeApiClient),
              Mock.Get(_organisationApiClient));
        }

        [TestMethod]
        public async Task SelectAcademy_WhenUserDidNotSelectSendForChildOrganisation_RedirectsToSelectDocumentType()
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.SelectAcademy(new SelectAcademy
            {
                SelectChildAcademy = false
            });

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(OrganisationController.SelectDocumentType));

            Mock.VerifyAll(
               Mock.Get(IdentityService),
               Mock.Get(ExchangeApiClient),
               Mock.Get(_organisationApiClient));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(224)]
        [DataRow(787)]
        [DataRow(142)]
        [DataRow(253)]
        public async Task SelectAcademy_WhenUserSelectedSendForChildOrganisation_ReturnsTheViewModelWithTheChildAcademies(
            int numberOfChildAcademies)
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);
            OrganisationIdentifier actualOrganisationIdentifier = null;

            var testParentOrganisation = GetTestParentOrganisation(numberOfChildAcademies);

            Mock.Get(_organisationApiClient)
             .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
             .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
             {
                 actualOrganisationIdentifier = organisationIdentifier;
                 return new Organisation
                 {
                     ChildOrganisations = GetTestOrganisationsFromApi(numberOfChildAcademies)
                 };
             });

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.SelectAcademy(new SelectAcademy
            {
                SelectChildAcademy = true
            });

            // Assert
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
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
              Mock.Get(IdentityService),
              Mock.Get(ExchangeApiClient),
              Mock.Get(_organisationApiClient));
        }

        [TestMethod]
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

            SetupUserIdentity(TestOrganisationUser);
            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
            .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
            .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
            {
                actualOrganisationIdentifier = organisationIdentifier;
                return new Organisation
                {
                    ChildOrganisations = GetTestOrganisationsFromApi(numberOfChildAcademies)
                };
            });

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.SelectAcademy(new SelectAcademy
            {
                SelectChildAcademy = true,
                SearchTerm = ChildOrgSearchTerm,
                PageNumber = 2,
                NewSearch = true
            });

            // Assert
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
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
              Mock.Get(IdentityService),
              Mock.Get(ExchangeApiClient),
              Mock.Get(_organisationApiClient));
        }

        [TestMethod]
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

            SetupUserIdentity(TestOrganisationUser);
            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
            .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
            .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
            {
                actualOrganisationIdentifier = organisationIdentifier;
                return new Organisation
                {
                    ChildOrganisations = GetTestOrganisationsFromApi(numberOfChildAcademies)
                };
            });

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.SelectAcademy(new SelectAcademy
            {
                SelectChildAcademy = true,
                PageNumber = SelectedPageNumber
            });

            // Assert
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
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
              Mock.Get(IdentityService),
              Mock.Get(ExchangeApiClient),
              Mock.Get(_organisationApiClient));
        }

        [TestMethod]
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

            SetupUserIdentity(TestOrganisationUser);
            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
            .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
            .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
            {
                actualOrganisationIdentifier = organisationIdentifier;
                return new Organisation
                {
                    ChildOrganisations = GetTestOrganisationsFromApi(numberOfChildAcademies)
                };
            });

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.SelectAcademy(new SelectAcademy
            {
                SelectChildAcademy = true,
                SearchTerm = ChildOrgSearchTerm,
                PageNumber = SelectedPageNumber
            });

            // Assert
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
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
              Mock.Get(IdentityService),
              Mock.Get(ExchangeApiClient),
              Mock.Get(_organisationApiClient));
        }

        [TestMethod]
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

            SetupUserIdentity(TestOrganisationUser);
            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
            .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
            .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
            {
                actualOrganisationIdentifier = organisationIdentifier;
                return new Organisation
                {
                    ChildOrganisations = GetTestOrganisationsFromApi(numberOfChildAcademies)
                };
            });

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.SelectAcademy(new SelectAcademy
            {
                SelectChildAcademy = true,
                SearchTerm = "fake search term",
                ClearSearch = true,
                PageNumber = 2
            });

            // Assert
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
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
              Mock.Get(IdentityService),
              Mock.Get(ExchangeApiClient),
              Mock.Get(_organisationApiClient));
        }

        #endregion


        #region SelectDocumentType

        [TestMethod]
        [DataRow(1)]
        [DataRow(224)]
        [DataRow(787)]
        [DataRow(142)]
        [DataRow(346)]
        public async Task SelectDocumentType_ForNullInputViewModel_ReturnsTheExpectedViewModel(
            int numberOfAllowedProducts)
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);
            var testProducts = GetTestProducts(numberOfAllowedProducts);

            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetProductsThatOrganisationsCanUpload())
                .ReturnsAsync(testProducts);

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                .ReturnsAsync(new Organisation());

            var controller = await GetOrganisationController();

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
                Mock.Get(IdentityService),
                Mock.Get(_settingsApiClient));
        }

        [TestMethod]
        public async Task SelectDocumentType_WhenSelectAnOrganisationRequiredButUploadingForUkprnIsNull_RedirectsToSelectAcademy()
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);
            var controller = await GetOrganisationController();
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

            Mock.VerifyAll(
                Mock.Get(IdentityService));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(234)]
        [DataRow(787)]
        [DataRow(143)]
        [DataRow(256)]
        public async Task SelectDocumentType_ForPopulatedInputViewModel_ReturnsTheExpectedViewModel(
            int numberOfAllowedProducts)
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);
            var testProducts = GetTestProducts(numberOfAllowedProducts);

            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetProductsThatOrganisationsCanUpload())
                .ReturnsAsync(testProducts);

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                .ReturnsAsync(new Organisation());

            var controller = await GetOrganisationController();

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
                Mock.Get(IdentityService),
                Mock.Get(_settingsApiClient));
        }

        #endregion


        #region SelectDocument

        [TestMethod]
        public async Task SelectDocument_ForNullInputViewModel_RedirectsToSelectDocumentType()
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);
            SetupSettingsApiClientGetMaxFileUploadSize();

            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetProductsThatOrganisationsCanUpload())
                .ReturnsAsync(Enumerable.Empty<Product>());

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.SelectDocument(null);

            // Assert
            result.Should()
                .BeRedirectToActionResult()
                .WithPreserveMethod(true)
                .WithActionName(nameof(OrganisationController.SelectDocumentType));

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(_settingsApiClient));
        }

        [TestMethod]
        public async Task SelectDocument_WhenProductIdentifierNotSelected_RedirectsToSelectDocumentType()
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.SelectDocument(new SelectDocument { ProductIdentifier = 0 });

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithPreserveMethod(true)
                .WithActionName(nameof(OrganisationController.SelectDocumentType))
                .WithControllerName(NameOf<OrganisationController>())
                .WithRouteValue("error", true);

            Mock.VerifyAll(
                Mock.Get(IdentityService));
        }

        [TestMethod]
        public async Task SelectDocument_WhenProductNotAllowed_RedirectsToSelectDocumentType()
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);
            SetupSettingsApiClientGetMaxFileUploadSize();

            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetProductsThatOrganisationsCanUpload())
                .ReturnsAsync(Enumerable.Empty<Product>());

            var controller = await GetOrganisationController();

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
                Mock.Get(IdentityService),
                Mock.Get(_settingsApiClient));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(23424)]
        [DataRow(787)]
        [DataRow(14342)]
        [DataRow(25346)]
        public async Task SelectDocument_WhenNotUploadingForUkprn_GetsVersionForTheCurrentUsersOrganisation(
            int numberOfAllowedProducts)
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);
            SetUpSettingsApiClient();

            var testProducts = GetTestProducts(numberOfAllowedProducts).ToList();
            var testSelectedProduct = testProducts.Last();
            var testCurrentVersion = 123;

            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetProductsThatOrganisationsCanUpload())
                .ReturnsAsync(testProducts);

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                .ReturnsAsync(new Organisation());

            OrganisationIdentifier actualOrganisationIdentifier = null;
            string actualIdentifier = null;

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.GetCurrentProductVersionForOrganisation(
                    It.IsAny<OrganisationIdentifier>(),
                    It.IsAny<string>()))
                .ReturnsAsync((OrganisationIdentifier organisationIdentifier, string identifier) =>
                {
                    actualOrganisationIdentifier = organisationIdentifier;
                    actualIdentifier = identifier;
                    return testCurrentVersion;
                });

            var controller = await GetOrganisationController();

            var inputViewModel = new SelectDocument
            {
                ProductIdentifier = testSelectedProduct.Identifier
            };

            // Act
            var result = await controller.SelectDocument(inputViewModel);

            // Assert
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
            actualIdentifier.Should().BeEquivalentTo(testSelectedProduct.Identifier.ToString());
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
                Mock.Get(IdentityService),
                Mock.Get(_settingsApiClient),
                Mock.Get(ExchangeApiClient));
        }

        [TestMethod]
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

            SetupUserIdentity(TestOrganisationUser);
            SetUpSettingsApiClient();

            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetProductsThatOrganisationsCanUpload())
                .ReturnsAsync(testProducts);

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                .ReturnsAsync(new Organisation());

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.GetCurrentProductVersionForOrganisation(
                    It.Is<OrganisationIdentifier>(o =>
                        o.Value == testUkprn &&
                        o.Type == OrganisationIdentifierType.Ukprn),
                    testSelectedProduct.Identifier.ToString()))
                .ReturnsAsync(testCurrentVersion);

            var controller = await GetOrganisationController();

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
                    AllowedFileExtensions = GetFileExtensionInfo().Select(f => f.Extension.ToUpperInvariant()).ToList(),
                    MaxFileUploadSize = 1
                });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(ExchangeApiClient),
                Mock.Get(_settingsApiClient));
        }

        #endregion


        #region DocumentUploadComplete

        [TestMethod]
        [DataRow(1)]
        [DataRow(23424)]
        [DataRow(787)]
        [DataRow(14342)]
        [DataRow(25346)]
        public async Task DocumentUploadComplete_ForParentOrganisationUser_SendingAsSelf_SetsTheParentOrganisationName(
            int numberOfChildOrganisations)
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);

            var testOrganisationUserInfo = TestOrganisationUserInfo;
            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
               .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
               .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
               {
                   actualOrganisationIdentifier = organisationIdentifier;
                   return new Organisation
                   {
                       ChildOrganisations = new List<Organisation>
                       {
                            new Organisation { }
                       }
                   };
               });

            var controller = await GetOrganisationController();

            var inputViewModel = new DocumentUploadComplete
            {
                DocumentFileName = "test file name 1.pdf"
            };

            // Act
            var result = await controller.DocumentUploadComplete(inputViewModel);

            // Assert
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(new DocumentUploadComplete
                {
                    DocumentFileName = inputViewModel.DocumentFileName,
                    SelectAnOrganisationRequired = true,
                    OrganisationName = testOrganisationUserInfo.OrganisationInfo.Name,
                    SendNewDocumentActionName = nameof(OrganisationController.SelectOrganisation)
                });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(_organisationApiClient));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(23424)]
        [DataRow(787)]
        [DataRow(14342)]
        [DataRow(25346)]
        public async Task DocumentUploadComplete_ForParentOrganisationUser_SendingAsChild_SetsTheChildOrganisationName(
            int numberOfChildOrganisations)
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);
            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
               .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
               .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
               {
                   actualOrganisationIdentifier = organisationIdentifier;
                   return new Organisation
                   {
                       ChildOrganisations = GetTestOrganisationsFromApi(numberOfChildOrganisations)
                   };
               });

            var testParentOrganisation = GetTestParentOrganisation(numberOfChildOrganisations);
            var testChildOrganisation = testParentOrganisation.ChildOrganisations.Last();

            var controller = await GetOrganisationController();

            var inputViewModel = new DocumentUploadComplete
            {
                DocumentFileName = "test file name 1.pdf",
                UploadingForUkprn = testChildOrganisation.Identifiers.First().Value
            };

            // Act
            var result = await controller.DocumentUploadComplete(inputViewModel);

            // Assert
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
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
                Mock.Get(IdentityService),
                Mock.Get(_organisationApiClient));
        }

        [TestMethod]
        public async Task DocumentUploadComplete_ForChildOrganisationUser_DoesNotSetTheOrganisationName()
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);

            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
               .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
               .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
               {
                   actualOrganisationIdentifier = organisationIdentifier;
                   return new Organisation
                   {
                       ChildOrganisations = new List<Organisation> { }
                   };
               });

            var controller = await GetOrganisationController();

            var inputViewModel = new DocumentUploadComplete
            {
                DocumentFileName = "test file name 1.pdf"
            };

            // Act
            var result = await controller.DocumentUploadComplete(inputViewModel);

            // Assert
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(new DocumentUploadComplete
                {
                    DocumentFileName = inputViewModel.DocumentFileName,
                    SelectAnOrganisationRequired = false,
                    OrganisationName = null,
                    SendNewDocumentActionName = nameof(OrganisationController.SelectDocumentType)
                });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(_organisationApiClient));
        }

        #endregion


        #region SentDocuments

        [TestMethod]
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

            SetupUserIdentity(TestOrganisationUser);

            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
              .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
              .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
              {
                  actualOrganisationIdentifier = organisationIdentifier;
                  return new Organisation
                  {
                      ChildOrganisations = new List<Organisation>
                        {
                            new Organisation { }
                        }
                  };
              });

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();
            var testViewModelFilters = GetTestViewModelFilterCategories(numberOfFilters).ToList();

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var listResult = new ListResult<ExchangeDocument>
            {
                Items = GetTestExchangeDocuments(numberOfPages, true),
                Filters = testServiceFilters,
                TotalPages = numberOfPages,
                TotalItems = expectedNumberOfItems
            };

            ExchangeListOrganisationDocumentOptions actualDocumentOptions = null;
            var expectedDocumentOptions = new ExchangeListOrganisationDocumentOptions
            {
                DocumentStatusOption = ExchangeDocumentDirection.SentByOrganisation,
                PageSize = TestConfiguration.ListPageSize,
                OrganisationIdentifier = testOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier
            };

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.GetOrganisationDocuments(It.IsAny<ExchangeListOrganisationDocumentOptions>()))
                .ReturnsAsync((ExchangeListOrganisationDocumentOptions documentOptions) =>
                {
                    actualDocumentOptions = documentOptions;
                    return listResult;
                });

            var expectedPagination = new PaginationViewModel
            {
                TotalItems = expectedNumberOfItems,
                TotalPages = numberOfPages,
                Page = 1,
                PageSize = TestConfiguration.ListPageSize
            };

            var testSentDocuments = GetTestSentDocuments(numberOfPages);
            var actualDateTime = new DateTime(2020, 01, 01);
            var sendDateTime = new DateTime(2020, 01, 01);

            if (numberOfPages > 0)
            {
                sendDateTime = testSentDocuments.Last().SentDateTime;
                actualDateTime = sendDateTime;

                Mock.Get(SystemProvider.DateTime)
                   .Setup(d => d.ConvertToUKTime(It.IsAny<DateTime>()))
                   .Returns((DateTime inputDateTime) =>
                   {
                       actualDateTime = inputDateTime;
                       return sendDateTime;
                   });

                Mock.Get(SystemProvider.DateTime)
                    .Setup(d => d.Now())
                    .Returns(new DateTime(2020, 12, 15));
            }

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.SentDocuments(null);

            // Assert
            actualDateTime.Should().Be(sendDateTime);
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
            actualDocumentOptions.Should().BeEquivalentTo(expectedDocumentOptions);
            result.Should().BeViewResult()
                .Model.Should().BeOfType<SentDocuments>()
                .Which.Should().BeEquivalentTo(
                    new SentDocuments
                    {
                        ListItems = testSentDocuments,
                        FilterCategories = testViewModelFilters,
                        ShowParentView = true,
                        Pagination = expectedPagination,
                        AnyDocumentsAvailable = expectedNumberOfItems > 0 || testViewModelFilters.Count > 0
                    }, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(_organisationApiClient),
                Mock.Get(ExchangeApiClient),
                Mock.Get(SystemProvider.DateTime));
        }

        [TestMethod]
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

            SetupUserIdentity(TestOrganisationUser);

            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
              .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
              .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
              {
                  actualOrganisationIdentifier = organisationIdentifier;
                  return new Organisation { };
              });

            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();
            var testViewModelFilters = GetTestViewModelFilterCategories(numberOfFilters).ToList();

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var listResult = new ListResult<ExchangeDocument>
            {
                Items = GetTestExchangeDocuments(numberOfPages, true),
                Filters = testServiceFilters,
                TotalPages = numberOfPages,
                TotalItems = expectedNumberOfItems
            };

            ExchangeListOrganisationDocumentOptions actualDocumentOptions = null;
            var expectedDocumentOptions = new ExchangeListOrganisationDocumentOptions
            {
                DocumentStatusOption = ExchangeDocumentDirection.SentByOrganisation,
                PageSize = TestConfiguration.ListPageSize,
                OrganisationIdentifier = testOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier
            };

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.GetOrganisationDocuments(It.IsAny<ExchangeListOrganisationDocumentOptions>()))
                .ReturnsAsync((ExchangeListOrganisationDocumentOptions documentOptions) =>
                {
                    actualDocumentOptions = documentOptions;
                    return listResult;
                });

            var expectedPagination = new PaginationViewModel
            {
                TotalItems = expectedNumberOfItems,
                TotalPages = numberOfPages,
                Page = 1,
                PageSize = TestConfiguration.ListPageSize
            };

            var testSentDocuments = GetTestSentDocuments(numberOfPages);
            var actualDateTime = new DateTime(2020, 01, 01);
            var sendDateTime = new DateTime(2020, 01, 01);

            if (numberOfPages > 0)
            {
                sendDateTime = testSentDocuments.Last().SentDateTime;
                actualDateTime = sendDateTime;

                Mock.Get(SystemProvider.DateTime)
                   .Setup(d => d.ConvertToUKTime(It.IsAny<DateTime>()))
                   .Returns((DateTime inputDateTime) =>
                   {
                       actualDateTime = inputDateTime;
                       return sendDateTime;
                   });

                Mock.Get(SystemProvider.DateTime)
                    .Setup(d => d.Now())
                    .Returns(new DateTime(2020, 12, 15));
            }

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.SentDocuments(null);

            // Assert
            actualDateTime.Should().Be(sendDateTime);
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
            actualDocumentOptions.Should().BeEquivalentTo(expectedDocumentOptions);
            result.Should().BeViewResult()
                .Model.Should().BeOfType<SentDocuments>()
                .Which.Should().BeEquivalentTo(
                    new SentDocuments
                    {
                        ListItems = testSentDocuments,
                        FilterCategories = testViewModelFilters,
                        ShowParentView = false,
                        Pagination = expectedPagination,
                        AnyDocumentsAvailable = expectedNumberOfItems > 0 || testViewModelFilters.Count > 0
                    }, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(_organisationApiClient),
                Mock.Get(ExchangeApiClient),
                Mock.Get(SystemProvider.DateTime));
        }

        #endregion


        #region ReceivedDocuments

        [TestMethod]
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
            SetupUserIdentity(TestOrganisationUser);

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockExchangeClient = SetUpExchangeApiClient(numberOfFilters, numberOfPages, expectedNumberOfItems);

            var documentStatusProvider = new DocumentStatusProvider();

            var exchangeDocumentEvent = SetExchangeDocumentEventInitialValues();
            var events = new[]
            {
                exchangeDocumentEvent
            };

            var expectedDownloadStatusInfo = SetExpectedDownloadStatusInfoInitialValues();
            var actualDocumentStatusInfo = new DocumentStatusInfo();

            var sendDateTime = new DateTime(2020, 01, 01);
            var actualDateTime = new DateTime(2020, 01, 01);

            if (numberOfPages > 0)
            {
                sendDateTime = GetTestExchangeDocuments(numberOfPages, false).Last().EventHistory?.FirstOrDefault(
                    e => e.EventType == ExchangeDocumentEventType.PublishedByAgency)?.EventDateTime ?? DateTime.MinValue;
                actualDateTime = sendDateTime;

                SetUpSystemProviderDateTime(actualDateTime, sendDateTime);

                exchangeDocumentEvent.EventType = ExchangeDocumentEventType.DownloadedByReceiver;
                exchangeDocumentEvent.EventDateTime = new DateTime(2020, 9, 1);
                exchangeDocumentEvent.UserInfo = TestOrganisationUserInfo;

                expectedDownloadStatusInfo.Status = DownloadDocumentStatus.Downloaded;
                expectedDownloadStatusInfo.DownloadedTime = exchangeDocumentEvent.EventDateTime;
                expectedDownloadStatusInfo.DownloadedBy = exchangeDocumentEvent.UserInfo.FullName;
            }

            actualDocumentStatusInfo = documentStatusProvider.GetDownloadStatus(events);

            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
                {
                    actualOrganisationIdentifier = organisationIdentifier;

                    return new Organisation
                    {
                        ChildOrganisations = new List<Organisation>
                        {
                            new Organisation { }
                        }
                    };
                });

            var testViewModelFilters = GetTestViewModelFilterCategories(numberOfFilters).ToList();
            var expectedPagination = GetExpectedPagination(expectedNumberOfItems, numberOfPages);

            var controller = await GetOrganisationController();

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
                        AnyDocumentsAvailable = expectedNumberOfItems > 0 || testViewModelFilters.Count > 0,
                    }, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            actualDateTime.Should().BeSameDateAs(sendDateTime);
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
            actualDocumentStatusInfo.Should().BeEquivalentTo(expectedDownloadStatusInfo);

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(_organisationApiClient),
                mockExchangeClient,
                Mock.Get(SystemProvider.DateTime));
        }

        [TestMethod]
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
            SetupUserIdentity(TestOrganisationUser);

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockExchangeClient = SetUpExchangeApiClient(numberOfFilters, numberOfPages, expectedNumberOfItems);

            var documentStatusProvider = new DocumentStatusProvider();

            var exchangeDocumentEvent = SetExchangeDocumentEventInitialValues();

            var events = new[]
            {
                exchangeDocumentEvent
            };

            var expectedDownloadStatusInfo = SetExpectedDownloadStatusInfoInitialValues();
            var actualDocumentStatusInfo = new DocumentStatusInfo();

            var sendDateTime = new DateTime(2020, 01, 01);
            var actualDateTime = new DateTime(2020, 01, 01);

            if (numberOfPages > 0)
            {
                sendDateTime = GetTestExchangeDocuments(numberOfPages, false).Last().EventHistory?.FirstOrDefault(
                    e => e.EventType == ExchangeDocumentEventType.PublishedByAgency)?.EventDateTime ?? DateTime.MinValue;
                actualDateTime = sendDateTime;

                SetUpSystemProviderDateTime(actualDateTime, sendDateTime);

                exchangeDocumentEvent.EventType = ExchangeDocumentEventType.DownloadedByReceiver;
                exchangeDocumentEvent.EventDateTime = new DateTime(2020, 9, 1);
                exchangeDocumentEvent.UserInfo = TestOrganisationUserInfo;

                expectedDownloadStatusInfo.Status = DownloadDocumentStatus.Downloaded;
                expectedDownloadStatusInfo.DownloadedTime = exchangeDocumentEvent.EventDateTime;
                expectedDownloadStatusInfo.DownloadedBy = exchangeDocumentEvent.UserInfo.FullName;
            }

            actualDocumentStatusInfo = documentStatusProvider.GetDownloadStatus(events);

            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
                {
                    actualOrganisationIdentifier = organisationIdentifier;
                    return new Organisation
                    {
                        ParentOrganisation = new Organisation { }
                    };
                });

            var testViewModelFilters = GetTestViewModelFilterCategories(numberOfFilters).ToList();
            var expectedPagination = GetExpectedPagination(expectedNumberOfItems, numberOfPages);

            var controller = await GetOrganisationController();

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
                        AnyDocumentsAvailable = expectedNumberOfItems > 0 || testViewModelFilters.Count > 0,
                    }, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            actualDateTime.Should().BeSameDateAs(sendDateTime);
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
            actualDocumentStatusInfo.Should().BeEquivalentTo(expectedDownloadStatusInfo);

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(_organisationApiClient),
                mockExchangeClient,
                Mock.Get(SystemProvider.DateTime));
        }

        [TestMethod]
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
            SetupUserIdentity(TestOrganisationUser);

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockExchangeClient = SetUpExchangeApiClient(numberOfFilters, numberOfPages, expectedNumberOfItems);

            var documentStatusProvider = new DocumentStatusProvider();

            var exchangeDocumentEvent = SetExchangeDocumentEventInitialValues();

            var events = new[]
            {
                exchangeDocumentEvent
            };

            var expectedDownloadStatusInfo = SetExpectedDownloadStatusInfoInitialValues();
            var actualDocumentStatusInfo = new DocumentStatusInfo();

            var sendDateTime = new DateTime(2020, 01, 01);
            var actualDateTime = new DateTime(2020, 01, 01);

            if (numberOfPages > 0)
            {
                sendDateTime = GetTestExchangeDocuments(numberOfPages, false).Last().EventHistory?.FirstOrDefault(
                    e => e.EventType == ExchangeDocumentEventType.PublishedByAgency)?.EventDateTime ?? DateTime.MinValue;
                actualDateTime = sendDateTime;

                SetUpSystemProviderDateTime(actualDateTime, sendDateTime);

                exchangeDocumentEvent.EventType = ExchangeDocumentEventType.DownloadedByReceiver;
                exchangeDocumentEvent.EventDateTime = new DateTime(2020, 9, 1);
                exchangeDocumentEvent.UserInfo = TestOrganisationUserInfo;

                expectedDownloadStatusInfo.Status = DownloadDocumentStatus.Downloaded;
                expectedDownloadStatusInfo.DownloadedTime = exchangeDocumentEvent.EventDateTime;
                expectedDownloadStatusInfo.DownloadedBy = exchangeDocumentEvent.UserInfo.FullName;
            }

            actualDocumentStatusInfo = documentStatusProvider.GetDownloadStatus(events);

            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
                {
                    actualOrganisationIdentifier = organisationIdentifier;
                    return new Organisation
                    {
                        ChildOrganisations = new List<Organisation>
                        {
                            new Organisation { }
                        }
                    };
                });

            var testViewModelFilters = GetTestViewModelFilterCategories(numberOfFilters).ToList();
            var expectedPagination = GetExpectedPagination(expectedNumberOfItems, numberOfPages);

            var documentsRequestWithError = new DocumentsRequest
            {
                Error = true,
                ErrorAction = "the-error-action"
            };

            var controller = await GetOrganisationController();

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
                        AnyDocumentsAvailable = expectedNumberOfItems > 0 || testViewModelFilters.Count > 0,
                        Error = true,
                        ErrorAction = documentsRequestWithError.ErrorAction
                    }, options => options.Excluding(model => model.Pagination.BuildPageLinkRouteValues));

            actualDateTime.Should().BeSameDateAs(sendDateTime);
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
            actualDocumentStatusInfo.Should().BeEquivalentTo(expectedDownloadStatusInfo);

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(_organisationApiClient),
                mockExchangeClient,
                Mock.Get(SystemProvider.DateTime));
        }
        #endregion


        #region ReceivedDocumentsData

        [TestMethod]
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
            SetupUserIdentity(TestOrganisationUser);
            var testOrganisationUserInfo = TestOrganisationUserInfo;

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockExchangeClient = SetUpExchangeApiClient(numberOfFilters, numberOfPages, expectedNumberOfItems);

            var documentStatusProvider = new DocumentStatusProvider();

            var exchangeDocumentEvent = SetExchangeDocumentEventInitialValues();
            var events = new[]
            {
                exchangeDocumentEvent
            };

            var expectedDownloadStatusInfo = SetExpectedDownloadStatusInfoInitialValues();
            var actualDocumentStatusInfo = new DocumentStatusInfo();

            var sendDateTime = new DateTime(2020, 01, 01);
            var actualDateTime = new DateTime(2020, 01, 01);

            if (numberOfPages > 0)
            {
                sendDateTime = GetTestExchangeDocuments(numberOfPages, false).Last().EventHistory?.FirstOrDefault(
                    e => e.EventType == ExchangeDocumentEventType.PublishedByAgency)?.EventDateTime ?? DateTime.MinValue;
                actualDateTime = sendDateTime;

                SetUpSystemProviderDateTime(actualDateTime, sendDateTime);

                exchangeDocumentEvent.EventType = ExchangeDocumentEventType.DownloadedByReceiver;
                exchangeDocumentEvent.EventDateTime = new DateTime(2020, 9, 1);
                exchangeDocumentEvent.UserInfo = TestOrganisationUserInfo;

                expectedDownloadStatusInfo.Status = DownloadDocumentStatus.Downloaded;
                expectedDownloadStatusInfo.DownloadedTime = exchangeDocumentEvent.EventDateTime;
                expectedDownloadStatusInfo.DownloadedBy = exchangeDocumentEvent.UserInfo.FullName;
            }

            actualDocumentStatusInfo = documentStatusProvider.GetDownloadStatus(events);

            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
                {
                    actualOrganisationIdentifier = organisationIdentifier;

                    return new Organisation
                    {
                        ChildOrganisations = new List<Organisation>
                        {
                            new Organisation { }
                        }
                    };
                });

            var expectedData = new DocumentListUpdateData
            {
                ListItems = GetTestReceivedDocuments(numberOfPages),
                Pagination = GetExpectedPaginationUpdateData(expectedNumberOfItems, numberOfPages)
            };

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.ReceivedDocumentsData(null);

            // Assert
            result.Should().BeJsonResult()
                .Value.Should().BeOfType<DocumentListUpdateData>()
                .Which.Should().BeEquivalentTo(expectedData);

            actualDateTime.Should().BeSameDateAs(sendDateTime);
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
            actualDocumentStatusInfo.Should().BeEquivalentTo(expectedDownloadStatusInfo);

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(_organisationApiClient),
                mockExchangeClient,
                Mock.Get(SystemProvider.DateTime));
        }

        [TestMethod]
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
            SetupUserIdentity(TestOrganisationUser);
            var testOrganisationUserInfo = TestOrganisationUserInfo;

            var expectedNumberOfItems = numberOfPages * TestConfiguration.ListPageSize;

            var mockExchangeClient = SetUpExchangeApiClient(numberOfFilters, numberOfPages, expectedNumberOfItems);

            var documentStatusProvider = new DocumentStatusProvider();

            var exchangeDocumentEvent = SetExchangeDocumentEventInitialValues();
            var events = new[]
            {
                exchangeDocumentEvent
            };

            var expectedDownloadStatusInfo = SetExpectedDownloadStatusInfoInitialValues();
            var actualDocumentStatusInfo = new DocumentStatusInfo();

            var sendDateTime = new DateTime(2020, 01, 01);
            var actualDateTime = new DateTime(2020, 01, 01);

            if (numberOfPages > 0)
            {
                sendDateTime = GetTestExchangeDocuments(numberOfPages, false).Last().EventHistory?.FirstOrDefault(
                    e => e.EventType == ExchangeDocumentEventType.PublishedByAgency)?.EventDateTime ?? DateTime.MinValue;
                actualDateTime = sendDateTime;

                SetUpSystemProviderDateTime(actualDateTime, sendDateTime);

                exchangeDocumentEvent.EventType = ExchangeDocumentEventType.DownloadedByReceiver;
                exchangeDocumentEvent.EventDateTime = new DateTime(2020, 9, 1);
                exchangeDocumentEvent.UserInfo = TestOrganisationUserInfo;

                expectedDownloadStatusInfo.Status = DownloadDocumentStatus.Downloaded;
                expectedDownloadStatusInfo.DownloadedTime = exchangeDocumentEvent.EventDateTime;
                expectedDownloadStatusInfo.DownloadedBy = exchangeDocumentEvent.UserInfo.FullName;
            }

            actualDocumentStatusInfo = documentStatusProvider.GetDownloadStatus(events);

            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
                {
                    actualOrganisationIdentifier = organisationIdentifier;
                    return new Organisation
                    {
                        ParentOrganisation = new Organisation { }
                    };
                });

            var expectedData = new DocumentListUpdateData
            {
                ListItems = GetTestReceivedDocuments(numberOfPages),
                Pagination = GetExpectedPaginationUpdateData(expectedNumberOfItems, numberOfPages)
            };

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.ReceivedDocumentsData(null);

            // Assert
            result.Should().BeJsonResult()
                .Value.Should().BeOfType<DocumentListUpdateData>()
                .Which.Should().BeEquivalentTo(expectedData);

            actualDateTime.Should().BeSameDateAs(sendDateTime);
            actualOrganisationIdentifier.Should().BeEquivalentTo(TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier);
            actualDocumentStatusInfo.Should().BeEquivalentTo(expectedDownloadStatusInfo);

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(_organisationApiClient),
                mockExchangeClient,
                Mock.Get(SystemProvider.DateTime));
        }

        #endregion


        #region ReceivedDocumentsDownload

        [TestMethod]
        public async Task ReceivedDocumentsDownload_WhenDocumentReferenceListIsEmpty_RedirectsToReceivedDocumentsAction()
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);
            var documentReferenceList = new DocumentReferenceList
            {
                DocumentReferences = null
            };

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.ReceivedDocumentsDownload(documentReferenceList);

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(OrganisationController.ReceivedDocuments))
                .WithControllerName(NameOf<OrganisationController>())
                .WithRouteValue("error", true)
                .WithRouteValue("errorAction", nameof(OrganisationController.ReceivedDocumentsDownload));

            Mock.VerifyAll(
                Mock.Get(IdentityService));
        }

        [TestMethod]
        public async Task ReceivedDocumentsDownload_WhenDocumentReferenceListContainsItems_DownloadsFile()
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);

            var fileName = "fileName.pdf";
            var documentReference = $"{fileName}|batchIdentifier|parentBatchIdentifier";
            var documentReferences = new[] { documentReference };

            var documentReferenceList = new DocumentReferenceList
            {
                DocumentReferences = documentReferences
            };

            var fileContentBytes = new byte[] { 1, 2, 3, 4, 5 };

            var file = new DownloadedFile
            {
                Name = fileName,
                ContentType = FakeMimeTypeString,
                Content = fileContentBytes
            };

            var mockExchangeClient = Mock.Get(ExchangeApiClient);
            mockExchangeClient.Setup(a => a.DownloadDocuments(It.IsAny<ExchangeDocumentDownloadRequest>()))
                .ReturnsAsync(fileContentBytes);

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.ReceivedDocumentsDownload(documentReferenceList);

            // Assert
            result.Should().BeFileContentResult()
                .WithFileDownloadName(fileName)
                .WithContentType(FakeMimeTypeString);

            (result as FileContentResult).FileContents.Should().BeEquivalentTo(fileContentBytes);

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                mockExchangeClient);
        }
        #endregion


        #region UploadDocument

        [TestMethod]
        public async Task UploadDocument_ArgumentNullCheckTest()
        {
            // Arrange
            SetupUserIdentity(TestNoOrganisationUser);
            var controller = await GetOrganisationController();

            // Act
            Func<Task> act = async () => await controller.UploadDocument(null);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
            await act.Should().ThrowAsync<ArgumentNullException>().Where(e => e.Message.Contains("Upload document request requires a value."));
        }

        [TestMethod]
        public async Task UploadDocument_WhenUserOrgIsUnknown_ThrowsException()
        {
            // Arrange
            SetupUserIdentity(TestNoOrganisationUser);
            SetUpSettingsApiClient();
            var file = GetMockFile();

            var controller = await GetOrganisationController();

            // Act
            Func<Task> act = async () => await controller.UploadDocument(new Models.Organisation.UploadDocumentRequest { FileInput = file });

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Unable to retrieve current user's organisation.");
            Mock.VerifyAll(
                Mock.Get(IdentityService));
        }

        [TestMethod]
        public async Task UploadDocument_WhenUserUploadDocument_UploadingForUkprnIsNull_ReturnOkTest()
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);
            SetUpSettingsApiClient();
            var file = GetMockFile();

            Mock.Get(_uploadApiClient)
                .Setup(d => d.UploadDocument(It.Is<Services.Models.UploadDocumentRequest>(udr =>
                    udr.FromOrganisation.Value == TestOrganisationUserInfo.OrganisationInfo.OrganisationIdentifier.Value)))
                .Returns(Task.CompletedTask);

            var controller = await GetOrganisationController();

            var request = new Models.Organisation.UploadDocumentRequest
            {
                FileInput = file,
                UploadingForUkprn = null,
                ProductIdentifier = 1123
            };

            // Act
            var result = await controller.UploadDocument(request);

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(OrganisationController.DocumentUploadComplete));

            Mock.Verify(
                Mock.Get(_uploadApiClient),
                Mock.Get(IdentityService));
        }

        [TestMethod]
        public async Task UploadDocument_WhenUserCannotUploadDocumentForUkprn_ThrowsExceptionTest()
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);
            SetUpSettingsApiClient();
            var file = GetMockFile();

            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
              .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
              .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
              {
                  actualOrganisationIdentifier = organisationIdentifier;
                  return new Organisation
                  {
                      ChildOrganisations = new List<Organisation>
                        {
                            new Organisation
                            {
                                Identifiers = new List<OrganisationIdentifier>
                                {
                                    new OrganisationIdentifier
                                    {
                                        Type = OrganisationIdentifierType.Ukprn,
                                        Value = "some ukprn"
                                    }
                                }
                            }
                        }
                  };
              });

            var controller = await GetOrganisationController();
            var request = new Models.Organisation.UploadDocumentRequest { UploadingForUkprn = "ukprn", FileInput = file };

            // Act
            Func<Task> result = async () => await controller.UploadDocument(request);

            // Assert
            actualOrganisationIdentifier.Should().Be(null);
            await result.Should().ThrowAsync<InvalidOperationException>("Cannot upload document for ukprn: ukprn.");

            Mock.Verify(
                Mock.Get(IdentityService),
                Mock.Get(_organisationApiClient));
        }

        [TestMethod]
        public async Task UploadDocument_WhenUserCanUploadDocumentForUkprn_ReturnOkTest()
        {
            // Arrange
            var mockUkprn = "10000";
            var file = GetMockFile();

            SetupUserIdentity(TestOrganisationUser);
            SetUpSettingsApiClient();

            var mockOrganisationId = new OrganisationIdentifier
            {
                Type = OrganisationIdentifierType.Ukprn,
                Value = mockUkprn
            };

            OrganisationIdentifier actualOrganisationIdentifier = null;

            Mock.Get(_organisationApiClient)
              .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
              .ReturnsAsync((OrganisationIdentifier organisationIdentifier) =>
              {
                  actualOrganisationIdentifier = organisationIdentifier;
                  return new Organisation
                  {
                      ChildOrganisations = new List<Organisation>
                        {
                            new Organisation
                            {
                                Identifiers = new List<OrganisationIdentifier>
                                {
                                    mockOrganisationId
                                }
                            }
                        }
                  };
              });

            Mock.Get(_uploadApiClient)
                .Setup(d => d.UploadDocument(It.Is<Services.Models.UploadDocumentRequest>(udr =>
                    udr.FromOrganisation == mockOrganisationId)))
                .Returns(Task.CompletedTask);

            var controller = await GetOrganisationController();
            var request = new Models.Organisation.UploadDocumentRequest()
            {
                FileInput = file,
                UploadingForUkprn = mockUkprn,
                ProductIdentifier = 1123
            };

            // Act
            var result = await controller.UploadDocument(request);

            // Assert
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(OrganisationController.DocumentUploadComplete));

            Mock.Verify(
                Mock.Get(IdentityService),
                Mock.Get(_organisationApiClient),
                Mock.Get(_uploadApiClient));
        }

        [TestMethod]
        public async Task UploadDocument_WhenFileExtensionNotAllowed_RedirectsToSelectDocument()
        {
            // Arrange
            var file = GetMockFile();
            SetupUserIdentity(TestOrganisationUser);

            var request = new Models.Organisation.UploadDocumentRequest()
            {
                FileInput = file,
                ProductIdentifier = 1123
            };

            Mock.Get(_settingsApiClient)
               .Setup(o => o.GetMaxFileUploadSize())
               .ReturnsAsync(1000);

            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetFileExtensions())
                .ReturnsAsync(Enumerable.Empty<FileExtensionInfo>());

            Mock.Get(ExchangeApiClient)
            .Setup(e => e.GetCurrentProductVersionForOrganisation(
                It.IsAny<OrganisationIdentifier>(), It.IsAny<string>()))
            .ReturnsAsync(1);

            Mock.Get(_organisationApiClient)
                           .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                           .ReturnsAsync(new Organisation());

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.UploadDocument(request) as ViewResult;

            // Assert
            Assert.AreEqual(nameof(controller.SelectDocument), result.ViewName);

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(_settingsApiClient),
                Mock.Get(ExchangeApiClient),
                Mock.Get(_organisationApiClient));
        }

        [TestMethod]
        public async Task UploadDocument_WhenFileExtensionNotAllowedForUser_RedirectsToSelectDocument()
        {
            // Arrange
            var file = GetMockFile();
            SetupUserIdentity(TestOrganisationUser);

            var request = new Models.Organisation.UploadDocumentRequest()
            {
                FileInput = file,
                ProductIdentifier = 1123
            };

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

            Mock.Get(ExchangeApiClient)
            .Setup(e => e.GetCurrentProductVersionForOrganisation(
                It.IsAny<OrganisationIdentifier>(), It.IsAny<string>()))
            .ReturnsAsync(1);

            Mock.Get(_organisationApiClient)
                           .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                           .ReturnsAsync(new Organisation());

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.UploadDocument(request) as ViewResult;

            // Assert
            Assert.AreEqual(nameof(controller.SelectDocument), result.ViewName);

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(_settingsApiClient),
                Mock.Get(ExchangeApiClient),
                Mock.Get(_organisationApiClient));
        }

        [TestMethod]
        public async Task UploadDocument_WhenFileSizeNotValid_RedirectsToSelectDocument()
        {
            // Arrange
            var file = GetMockFile();
            SetupUserIdentity(TestOrganisationUser);

            var request = new Models.Organisation.UploadDocumentRequest()
            {
                FileInput = file,
                ProductIdentifier = 1123
            };

            Mock.Get(_settingsApiClient)
               .Setup(o => o.GetMaxFileUploadSize())
               .ReturnsAsync(0);

            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetFileExtensions())
                .ReturnsAsync(new List<FileExtensionInfo> { new FileExtensionInfo { Extension = "pdf" } });

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.GetCurrentProductVersionForOrganisation(
                It.IsAny<OrganisationIdentifier>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            Mock.Get(_organisationApiClient)
                           .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                           .ReturnsAsync(new Organisation());
            var controller = await GetOrganisationController();

            // Act
            var result = await controller.UploadDocument(request) as ViewResult;

            // Assert
            Assert.AreEqual(nameof(controller.SelectDocument), result.ViewName);

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(_settingsApiClient),
                Mock.Get(ExchangeApiClient),
                Mock.Get(_organisationApiClient));
        }

        [TestMethod]
        public async Task UploadDocument_WhenFileEmpty_RedirectsToSelectDocument()
        {
            // Arrange
            var file = GetMockFile(string.Empty);
            SetupUserIdentity(TestOrganisationUser);

            var request = new Models.Organisation.UploadDocumentRequest()
            {
                FileInput = file,
                ProductIdentifier = 1123
            };

            Mock.Get(_settingsApiClient)
               .Setup(o => o.GetMaxFileUploadSize())
               .ReturnsAsync(0);

            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetFileExtensions())
                .ReturnsAsync(new List<FileExtensionInfo> { new FileExtensionInfo { Extension = "pdf" } });

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.GetCurrentProductVersionForOrganisation(
                It.IsAny<OrganisationIdentifier>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            Mock.Get(_organisationApiClient)
                           .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                           .ReturnsAsync(new Organisation());
            var controller = await GetOrganisationController();

            // Act
            var result = await controller.UploadDocument(request) as ViewResult;

            // Assert
            Assert.AreEqual(nameof(controller.SelectDocument), result.ViewName);

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(_settingsApiClient),
                Mock.Get(ExchangeApiClient),
                Mock.Get(_organisationApiClient));
        }

        [TestMethod]
        public async Task UploadDocument_WhenFileNull_RedirectsToSelectDocument()
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);

            var request = new Models.Organisation.UploadDocumentRequest()
            {
                FileInput = null,
                ProductIdentifier = 1123
            };

            Mock.Get(_settingsApiClient)
               .Setup(o => o.GetMaxFileUploadSize())
               .ReturnsAsync(0);

            Mock.Get(_settingsApiClient)
                .Setup(o => o.GetFileExtensions())
                .ReturnsAsync(new List<FileExtensionInfo> { new FileExtensionInfo { Extension = "pdf" } });

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.GetCurrentProductVersionForOrganisation(
                It.IsAny<OrganisationIdentifier>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            Mock.Get(_organisationApiClient)
                           .Setup(o => o.GetOrganisation(It.IsAny<OrganisationIdentifier>()))
                           .ReturnsAsync(new Organisation());
            var controller = await GetOrganisationController();

            // Act
            var result = await controller.UploadDocument(request) as ViewResult;

            // Assert
            Assert.AreEqual(nameof(controller.SelectDocument), result.ViewName);

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(_settingsApiClient),
                Mock.Get(ExchangeApiClient),
                Mock.Get(_organisationApiClient));
        }

        #endregion


        #region DownloadExternalExchangeDocument

        [TestMethod]
        [DataRow("test content 1")]
        [DataRow("test content 2")]
        [DataRow("test content 3")]
        public async Task DownloadExternalExchangeDocument_ReturnsExpectedFileResult(string fileContent)
        {
            // Arrange
            SetupUserIdentity(TestOrganisationUser);

            var fakeFileName = "fakeFileName.pdf";
            var fakeDocumentReference = $"{fakeFileName}|fakeBatchIdentifier|fakeParentBatchIdentifier";

            var fileContentBytes = Encoding.UTF8.GetBytes(fileContent);

            ExchangeDocumentDownloadRequest actualDownloadRequest = null;

            Mock.Get(ExchangeApiClient)
                .Setup(e => e.DownloadDocuments(It.IsAny<ExchangeDocumentDownloadRequest>()))
                .ReturnsAsync((ExchangeDocumentDownloadRequest request) =>
                {
                    actualDownloadRequest = request;
                    return fileContentBytes;
                });

            var controller = await GetOrganisationController();

            // Act
            var result = await controller.DownloadExternalExchangeDocument(fakeDocumentReference);

            // Assert
            result.Should().BeFileContentResult()
                .WithFileDownloadName(fakeFileName)
                .WithContentType("application/pdf");

            (result as FileContentResult).FileContents.Should().BeEquivalentTo(fileContentBytes);

            actualDownloadRequest.Should()
                .BeEquivalentTo(
                    new ExchangeDocumentDownloadRequest
                    {
                        UserInfo = TestOrganisationUserInfo,
                        ListOptions = new ExchangeListDocumentOptions
                        {
                            DocumentReferences = new[]
                            {
                                new DocumentReference
                                {
                                    FileName = fakeFileName,
                                    BatchIdentifier = "fakeBatchIdentifier",
                                    ParentBatchIdentifier = "fakeParentBatchIdentifier"
                                }
                            }
                        }
                    });

            Mock.VerifyAll(
                Mock.Get(IdentityService),
                Mock.Get(ExchangeApiClient));
        }
        #endregion

        private async Task<OrganisationController> GetOrganisationController()
        {
            var documentReferenceService = new DocumentReferenceService();
            var mimeMappingService = new MimeMappingService();
            var testConfigurationOptions = Options.Create(TestConfiguration);

            var listHelper =
                new ListHelper(
                    new RouteValueDictionaryBuilder(),
                    Mapper,
                    SystemProvider,
                    testConfigurationOptions);

            var userInfoProvider = new UserInformationProvider(
                   IdentityService,
                   Mapper,
                   _organisationApiClient);

            await userInfoProvider.Initialise(null);

            return new OrganisationController(
                userInfoProvider,
                mimeMappingService,
                testConfigurationOptions,
                ExchangeApiClient,
                _uploadApiClient,
                _settingsApiClient,
                new ExchangeDocumentDownloadService(
                    documentReferenceService,
                    ExchangeApiClient,
                    mimeMappingService,
                    SystemProvider),
                new DocumentStatusProvider(),
                logger: MockLoggerAdapter<OrganisationController>(),
                new DateTimeDisplayHelper(SystemProvider.DateTime),
                listHelper);
        }

        private UserInfo TestOrganisationUserInfo
            => new UserInfo
            {
                FullName = "organisationuser test user",
                Principal = "organisationusertestuser",
                EmailAddress = "organisationuser@fake.com",
                IsViewAsOrganisation = false,
                OrganisationInfo = new OrganisationInfo
                {
                    OrganisationIdentifier = new OrganisationIdentifier
                    {
                        Value = "12345678",
                        Type = OrganisationIdentifierType.Ukprn
                    }
                }
            };

        private IFormFile GetMockFile(string content = "Hello World")
        {
            // Setup mock file using a memory stream
            var fileMock = new Mock<IFormFile>();
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

        private IEnumerable<Product> GetTestProducts(int numberOfProducts)
           => Enumerable
               .Range(1, numberOfProducts)
               .Select(p => new Product
               {
                   Identifier = 10000 + p,
                   Name = $"product {10000 + p}"
               });

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

        private List<Organisation> GetTestOrganisationsFromApi(int number)
            => Enumerable
                       .Range(1, number)
                       .Select(n => new Organisation
                       {
                           Name = $"child org {n}",
                           Identifiers = new List<OrganisationIdentifier>
                                {
                                    new OrganisationIdentifier
                                    {
                                        Type = OrganisationIdentifierType.Ukprn,
                                        Value = $"childOrg{n}ukprn"
                                    }
                                }
                       }).ToList();

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

        private Mock<IExchangeApiClient> SetUpExchangeApiClient(int numberOfFilters, int numberOfPages, int expectedNumberOfItems)
        {
            var testServiceFilters = GetTestServiceFilters(numberOfFilters).ToList();
            var testExchangeDocuments = GetTestExchangeDocuments(numberOfPages, false);
            var listResult = new ListResult<ExchangeDocument>
            {
                Items = testExchangeDocuments,
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

            return mockExchangeClient;
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

        private void SetUpSystemProviderDateTime(DateTime actualDateTime, DateTime sendDateTime)
        {
            Mock.Get(SystemProvider.DateTime)
                   .Setup(d => d.ConvertToUKTime(It.IsAny<DateTime>()))
                   .Returns((DateTime inputDateTime) =>
                   {
                       actualDateTime = inputDateTime;
                       return sendDateTime;
                   });

            Mock.Get(SystemProvider.DateTime)
                .Setup(d => d.Now())
                .Returns(new DateTime(2020, 12, 15));
        }

        private ExchangeDocumentEvent SetExchangeDocumentEventInitialValues()
        {
            return new ExchangeDocumentEvent
            {
                EventType = ExchangeDocumentEventType.PublishedByAgency,
                UserInfo = new UserInfo
                {
                    FullName = null,
                    Principal = "principal"
                }
            };
        }

        private DocumentStatusInfo SetExpectedDownloadStatusInfoInitialValues()
        {
            return new DocumentStatusInfo
            {
                Status = DownloadDocumentStatus.New,
                DownloadedTime = new DateTime(0001, 01, 01),
                DownloadedBy = null
            };
        }

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

        private PaginationViewModel GetExpectedPagination(int expectedNumberOfItems, int numberOfPages)
        {
            return new PaginationViewModel
            {
                TotalItems = expectedNumberOfItems,
                TotalPages = numberOfPages,
                Page = 1,
                PageSize = TestConfiguration.ListPageSize
            };
        }
    }
}