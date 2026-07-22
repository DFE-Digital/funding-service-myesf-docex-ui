using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Admin.Api.Client.Interfaces;
using Pds.DocumentExchange.Web.Areas.ViewAsOrganisation.Controllers;
using Pds.DocumentExchange.Web.Controllers;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Tests.Unit.Controllers.ViewAsOrganisation
{
    [TestClass]
    [TestCategory("Unit")]
    public class ViewAsOrganisationControllerTests : BaseControllerUnitTests
    {
        private readonly IViewAsOrganisationApiClient _viewAsOrganisation
            = Mock.Of<IViewAsOrganisationApiClient>(MockBehavior.Strict);

        [TestMethod]
        public async Task StartViewingAsOrganisation_SetsUkprnAndReturnsExpectedRedirect()
        {
            // Arrange
            var principal = "fake-user-123";
            var ukprn = 12345678;
            var providerName = "Test";
            SetupUserInfo(principal);

            Mock.Get(_viewAsOrganisation)
                .Setup(client => client.SetProviderInfo(principal, ukprn, providerName))
                .Returns(Task.CompletedTask);

            var controller = GetController();

            // Act
            var actual = await controller.StartViewingAsOrganisation(ukprn, providerName);

            // Assert
            actual.Should().BeRedirectToActionResult()
                .WithActionName(nameof(OrganisationController.Home));

            VerifyAllMocks();
        }

        [TestMethod]
        public async Task StopViewingAsAnOrganisation_ClearsUkprnAndReturnsExpectedRedirect()
        {
            // Arrange
            var principal = "fake-user-123";
            SetupUserInfo(principal);

            Mock.Get(_viewAsOrganisation)
                .Setup(client => client.ClearUkprn(principal))
                .Returns(Task.CompletedTask);

            var controller = GetController();

            // Act
            var actual = await controller.StopViewingAsAnOrganisation();

            // Assert
            actual.Should().BeRedirectToActionResult()
                .WithActionName(nameof(OrganisationSearchController.SearchForAnOrganisation));

            VerifyAllMocks();
        }

        private ViewAsOrganisationController GetController()
        {
            return new ViewAsOrganisationController(
                UserInformationProvider,
                Options.Create(TestConfiguration),
                _viewAsOrganisation);
        }

        private void SetupUserInfo(string principal)
        {
            Mock.Get(UserInformationProvider)
                .Setup(uip => uip.GetCurrentUserInfo())
                .ReturnsAsync(new Services.Models.UserInfo
                {
                    Principal = principal
                });
        }

        private void VerifyAllMocks()
        {
            Mock.VerifyAll(
                Mock.Get(UserInformationProvider),
                Mock.Get(_viewAsOrganisation));
        }
    }
}