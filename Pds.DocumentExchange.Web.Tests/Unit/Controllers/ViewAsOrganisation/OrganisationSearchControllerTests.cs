using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Common.Organisation.Enums;
using Pds.Core.Common.Organisation.Models;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Web.Areas.ViewAsOrganisation.Controllers;
using Pds.DocumentExchange.Web.Areas.ViewAsOrganisation.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewAsOrganisationArea = Pds.DocumentExchange.Web.Areas.ViewAsOrganisation;

namespace Pds.DocumentExchange.Web.Tests.Unit.Controllers.ViewAsOrganisation
{
    [TestClass]
    [TestCategory("Unit")]
    public class OrganisationSearchControllerTests : BaseControllerUnitTests
    {
        private readonly IOrganisationApiClient _organisationApiClient
            = Mock.Of<IOrganisationApiClient>(MockBehavior.Strict);

        [TestMethod]
        public async Task SearchForAnOrganisation_WhenNoError_ReturnsExpectedView()
        {
            // Arrange
            var controller = GetController();

            // Act
            var actual = await controller.SearchForAnOrganisation();

            // Assert
            actual.Should().BeViewResult()
                .Model.Should().BeAssignableTo<SearchForAnOrganisation>()
                .Which.Error.Should().BeFalse();
        }

        [TestMethod]
        public async Task SearchForAnOrganisation_WhenError_ReturnsExpectedView()
        {
            // Arrange
            var controller = GetController();

            // Act
            var actual = await controller.SearchForAnOrganisation(true);

            // Assert
            actual.Should().BeViewResult()
                .Model.Should().BeAssignableTo<SearchForAnOrganisation>()
                .Which.Error.Should().BeTrue();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public async Task OrganisationSearchResults_ForNullOrEmptySearch_RedirectsToSearchActionWithError(string searchTerm)
        {
            // Arrange
            var controller = GetController();

            // Act
            var actual = await controller.OrganisationSearchResults(
                new OrganisationSearchResults
                {
                    SearchTerm = searchTerm
                });

            // Assert
            actual.Should().BeRedirectToActionResult()
                .WithActionName(nameof(OrganisationSearchController.SearchForAnOrganisation))
                .WithRouteValue("error", true);
        }

        [TestMethod]
        public async Task OrganisationSearchResults_ForEightDigits_LooksUpTheUkprn()
        {
            // Arrange
            var ukprn = "12345678";
            var organisation = GetTestOrganisation();

            var controller = GetController();

            Mock.Get(_organisationApiClient)
                .Setup(client => client.GetOrganisation(It.Is<OrganisationIdentifier>(
                    id => id.Type == OrganisationIdentifierType.Ukprn
                    && id.Value == ukprn)))
                .ReturnsAsync(organisation);

            // Act
            var actual = await controller.OrganisationSearchResults(
                new OrganisationSearchResults
                {
                    SearchTerm = ukprn
                });

            // Assert
            actual.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(
                    new OrganisationSearchResults
                    {
                        SearchTerm = ukprn,
                        HasMoreResults = false,
                        Organisations = new[] { (organisation.Identifiers.First().Value, organisation.Name) }
                    });

            Mock.VerifyAll(Mock.Get(_organisationApiClient));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(ViewAsOrganisationArea.Constants.MaxSearchResults - 1)]
        [DataRow(ViewAsOrganisationArea.Constants.MaxSearchResults)]
        [DataRow(ViewAsOrganisationArea.Constants.MaxSearchResults + 1)]
        public async Task OrganisationSearchResults_ForSearchTerm_PerformsNameSearch(int numberOfResults)
        {
            // Arrange
            var shouldHaveMoreResults = numberOfResults >= ViewAsOrganisationArea.Constants.MaxSearchResults;

            var searchTerm = "search term";

            var organisations = Enumerable.Range(0, numberOfResults).Select(GetTestOrganisation).ToList();

            var controller = GetController();

            Mock.Get(_organisationApiClient)
                .Setup(client => client.Search(
                    searchTerm,
                    ViewAsOrganisationArea.Constants.MaxSearchResults))
                .ReturnsAsync(organisations);

            // Act
            var actual = await controller.OrganisationSearchResults(
                new OrganisationSearchResults
                {
                    SearchTerm = searchTerm
                });

            // Assert
            actual.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(
                    new OrganisationSearchResults
                    {
                        SearchTerm = searchTerm,
                        HasMoreResults = shouldHaveMoreResults,
                        Organisations = organisations
                            .Select(organisation => (organisation.Identifiers.First().Value, organisation.Name))
                            .ToList()
                    });

            Mock.VerifyAll(Mock.Get(_organisationApiClient));
        }

        private OrganisationSearchController GetController()
        {
            return new OrganisationSearchController(
                UserInformationProvider,
                Options.Create(TestConfiguration),
                _organisationApiClient);
        }

        private Organisation GetTestOrganisation(int number = 1)
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
    }
}