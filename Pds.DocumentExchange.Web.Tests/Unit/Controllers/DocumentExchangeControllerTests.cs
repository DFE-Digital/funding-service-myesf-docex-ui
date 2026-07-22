using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Web.Models;
using Pds.DocumentExchange.Web.Areas.Admin.Controllers;
using Pds.DocumentExchange.Web.Areas.ViewAsOrganisation.Controllers;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.Models;
using Pds.DocumentExchange.Web.Models.DocumentExchange;
using System.Threading.Tasks;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Tests.Unit.Controllers
{
    [TestClass]
    [TestCategory("Unit")]
    public class DocumentExchangeControllerTests : BaseControllerUnitTests
    {
        [TestMethod]
        public async Task Landing_ForNotLoggedInUser_WhenStartPageEnabled_ReturnsView()
        {
            // Arrange
            SetupCurrentUserAsLoggedIn(false);

            var controller = GetDocumentExchangeController(startPageEnabled: true);

            // Act
            var result = await controller.Landing();

            // Assert
            result.Should().BeViewResult().ModelAs<Landing>();

            Mock.Get(UserInformationProvider).VerifyAll();
        }

        [TestMethod]
        public async Task Landing_ForNotLoggedInUser_WhenStartPageNotEnabled_RedirectsToLoginPage()
        {
            // Arrange
            SetupCurrentUserAsLoggedIn(false);

            var controller = GetDocumentExchangeController(startPageEnabled: false);

            // Act
            var result = await controller.Landing();

            // Assert
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName(NameOf<AccountController>())
                .WithActionName(nameof(AccountController.Login));

            Mock.Get(UserInformationProvider).VerifyAll();
        }

        [TestMethod]
        public async Task Landing_ForLoggedInUserWithoutAccess_ReturnsUnauthorized()
        {
            // Arrange
            SetupCurrentUserAsLoggedIn(true);
            SetupCurrentUserAsDocExUser(false);

            var controller = GetDocumentExchangeController();

            // Act
            var result = await controller.Landing();

            // Assert
            result.Should().BeUnauthorizedResult();

            Mock.Get(UserInformationProvider).VerifyAll();
        }

        [TestMethod]
        public async Task Landing_ForOrganisationUser_RedirectsToOrganisationLandingAction()
        {
            // Arrange
            SetupCurrentUserAsLoggedIn(true);
            SetupCurrentUserAsDocExUser(true);
            SetupCurrentUserAsOrganisationUser(true);

            var controller = GetDocumentExchangeController();

            // Act
            var result = await controller.Landing();

            // Assert
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName(NameOf<OrganisationController>())
                .WithActionName(nameof(OrganisationController.Home));

            Mock.Get(UserInformationProvider).VerifyAll();
        }

        [TestMethod]
        public async Task Landing_ForAgencyUser_RedirectsToAgencyLandingAction()
        {
            // Arrange
            SetupInternalUser();
            SetupCurrentUserAsAgencyUser(true);

            var controller = GetDocumentExchangeController();

            // Act
            var result = await controller.Landing();

            // Assert
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName(NameOf<AgencyController>())
                .WithActionName(nameof(AgencyController.AgencyHome));

            Mock.Get(UserInformationProvider).VerifyAll();
        }

        [TestMethod]
        public async Task Landing_ForAdminOnlyUser_WhenStartPageEnabled_RedirectsToSettingsAction()
        {
            // Arrange
            SetupInternalUser();
            SetupCurrentUserAsAgencyUser(false);
            SetupCurrentUserAsAdminUser(true);
            SetupCurrentUserAsViewAsOrganisationUser(false);

            var controller = GetDocumentExchangeController(startPageEnabled: true);

            // Act
            var result = await controller.Landing();

            // Assert
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName(NameOf<SettingsController>())
                .WithActionName(nameof(SettingsController.Index));

            Mock.Get(UserInformationProvider).VerifyAll();
        }

        [TestMethod]
        public async Task Landing_ForAdminUser_WhenStartPageNotEnabled_RedirectsToSettingsAction()
        {
            // Arrange
            SetupInternalUser();
            SetupCurrentUserAsAgencyUser(false);
            SetupCurrentUserAsAdminUser(true);

            var controller = GetDocumentExchangeController(startPageEnabled: false);

            // Act
            var result = await controller.Landing();

            // Assert
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName(NameOf<SettingsController>())
                .WithActionName(nameof(SettingsController.Index));

            Mock.Get(UserInformationProvider).VerifyAll();
        }

        [TestMethod]
        public async Task Landing_ForViewAsOrganisationOnlyUser_WhenStartPageEnabled_RedirectsToOrganisationSearchAction()
        {
            // Arrange
            SetupInternalUser();
            SetupCurrentUserAsAgencyUser(false);
            SetupCurrentUserAsAdminUser(false);
            SetupCurrentUserAsViewAsOrganisationUser(true);

            var controller = GetDocumentExchangeController(startPageEnabled: true);

            // Act
            var result = await controller.Landing();

            // Assert
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName(NameOf<OrganisationSearchController>())
                .WithActionName(nameof(OrganisationSearchController.SearchForAnOrganisation));

            Mock.Get(UserInformationProvider).VerifyAll();
        }

        [TestMethod]
        public async Task Landing_ForAdminAndViewAsOrganisationUser_WhenStartPageEnabled_RedirectsToAgencyLandingAction()
        {
            // Arrange
            SetupInternalUser();
            SetupCurrentUserAsAgencyUser(false);
            SetupCurrentUserAsAdminUser(true);
            SetupCurrentUserAsViewAsOrganisationUser(true);

            var controller = GetDocumentExchangeController(startPageEnabled: true);

            // Act
            var result = await controller.Landing();

            // Assert
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName(NameOf<AgencyController>())
                .WithActionName(nameof(AgencyController.AgencyHome));

            Mock.Get(UserInformationProvider).VerifyAll();
        }

        [TestMethod]
        public async Task Landing_ForInternalUserWithoutAnyRoles_WhenStartPageEnabled_ReturnsUnauthorized()
        {
            // Arrange
            SetupInternalUser();
            SetupCurrentUserAsAgencyUser(false);
            SetupCurrentUserAsAdminUser(false);
            SetupCurrentUserAsViewAsOrganisationUser(false);

            var controller = GetDocumentExchangeController(startPageEnabled: true);

            // Act
            var result = await controller.Landing();

            // Assert
            result.Should().BeUnauthorizedResult();

            Mock.Get(UserInformationProvider).VerifyAll();
        }

        [TestMethod]
        public async Task Landing_ForInternalUserWithoutAnyRoles_WhenStartPageNotEnabled_ReturnsUnauthorized()
        {
            // Arrange
            SetupInternalUser();
            SetupCurrentUserAsAgencyUser(false);
            SetupCurrentUserAsAdminUser(false);

            var controller = GetDocumentExchangeController(startPageEnabled: false);

            // Act
            var result = await controller.Landing();

            // Assert
            result.Should().BeUnauthorizedResult();

            Mock.Get(UserInformationProvider).VerifyAll();
        }

        [TestMethod]
        public async Task TermsAndConditions_ReturnsExpectedView()
        {
            // Arrange
            var controller = GetDocumentExchangeController();

            // Act
            var result = await controller.TermsAndConditions();

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<TermsAndConditions>();
        }

        [TestMethod]
        public async Task UserGuide_ReturnsExpectedView()
        {
            // Arrange
            var controller = GetDocumentExchangeController();

            // Act
            var result = await controller.UserGuide();

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<UserGuide>();
        }

        [TestMethod]
        public async Task AccountSettings_ForNotAllowedUser_ReturnsUnauthorized()
        {
            // Arrange
            SetupCurrentUserAsDocExUser(false);

            var controller = GetDocumentExchangeController();

            // Act
            var result = await controller.AccountSettings();

            // Assert
            result.Should().BeUnauthorizedResult();
        }

        [TestMethod]
        public async Task AccountSettings_ForAllowedUser_ReturnsExpectedView()
        {
            // Arrange
            SetupCurrentUserAsDocExUser(true);

            var expectedPermissions = new[]
            {
                "something they can do 1",
                "something they can do 2"
            };

            Mock.Get(UserInformationProvider)
                .Setup(p => p.GetCurrentUserPermissions())
                .ReturnsAsync(expectedPermissions);

            var controller = GetDocumentExchangeController();

            // Act
            var result = await controller.AccountSettings();

            // Assert
            result.Should().BeViewResult()
                .Model.Should().BeOfType<AccountSettings>()
                .Which.Permissions.Should().BeEquivalentTo(expectedPermissions);
        }

        private void SetupInternalUser()
        {
            SetupCurrentUserAsLoggedIn(true);
            SetupCurrentUserAsDocExUser(true);
            SetupCurrentUserAsOrganisationUser(false);
        }

        private void SetupCurrentUserAsLoggedIn(bool loggedIn)
        {
            Mock.Get(UserInformationProvider)
                .Setup(p => p.GetCurrentUserViewModel())
                .ReturnsAsync(
                    new CurrentUserViewModel
                    {
                        IsLoggedIn = loggedIn
                    });
        }

        private void SetupCurrentUserAsDocExUser(bool docExUser)
        {
            Mock.Get(UserInformationProvider)
                .Setup(p => p.CurrentUserCanAccessDocumentExchange())
                .ReturnsAsync(docExUser);
        }

        private void SetupCurrentUserAsAdminUser(bool adminUser)
        {
            Mock.Get(UserInformationProvider)
                .Setup(p => p.CurrentUserIsAdminUser())
                .ReturnsAsync(adminUser);
        }

        private void SetupCurrentUserAsOrganisationUser(bool organisationUser)
        {
            Mock.Get(UserInformationProvider)
                .Setup(p => p.CurrentUserIsOrganisationUserOrImpersonating())
                .ReturnsAsync(organisationUser);
        }

        private void SetupCurrentUserAsViewAsOrganisationUser(bool viewAsOrganisationUser)
        {
            Mock.Get(UserInformationProvider)
                .Setup(p => p.CurrentUserCanViewAsOrganisation())
                .ReturnsAsync(viewAsOrganisationUser);
        }

        private void SetupCurrentUserAsAgencyUser(bool agencyUser)
        {
            Mock.Get(UserInformationProvider)
                .Setup(p => p.GetCurrentUserAgencyTeams())
                .ReturnsAsync(agencyUser ? "team1" : null);
        }

        private DocumentExchangeController GetDocumentExchangeController(bool startPageEnabled = false)
            => new DocumentExchangeController(
                UserInformationProvider,
                Options.Create(new DocumentExchangeConfiguration { ShowServiceStartPage = startPageEnabled }));
    }
}