using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pds.Admin.Api.Client.Interfaces;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models;
using System.Threading.Tasks;
using static Pds.DocumentExchange.Web.Helpers.AuthorizationHelper;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Areas.ViewAsOrganisation.Controllers
{
    /// <summary>
    /// Controller providing actions for starting or stopping view as organisation.
    /// </summary>
    [Authorize(Policy = Policies.RequireViewAsOrganisationRole)]
    [Area(Constants.AreaName)]
    [Route("/[Area]/[Action]")]
    public class ViewAsOrganisationController : BaseDocumentExchangeController
    {
        private readonly IViewAsOrganisationApiClient _viewAsOrganisation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAsOrganisationController"/> class.
        /// </summary>
        /// <param name="userInformationProvider"><see cref="IUserInformationProvider"/>.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="viewAsOrganisation"><see cref="IViewAsOrganisationApiClient"/>.</param>
        /// <param name="organisationApiClient"><see cref="IOrganisationApiClient"/>.</param>
        public ViewAsOrganisationController(
            IUserInformationProvider userInformationProvider,
            IOptions<DocumentExchangeConfiguration> configurationOptions,
            IViewAsOrganisationApiClient viewAsOrganisation)
            : base(userInformationProvider, configurationOptions)
        {
            _viewAsOrganisation = viewAsOrganisation;
        }

        /// <summary>
        /// Action for starting view as an organisation.
        /// </summary>
        /// <param name="ukprn">The UKPRN of the organisation to start viewing as.</param>
        /// <param name="providerName">The provider name of the organisation to start viewing as.</param>
        /// <returns>Redirects to the organisation home page.</returns>
        public async Task<IActionResult> StartViewingAsOrganisation(int ukprn, string providerName)
        {
            var currentUser = await UserInformationProvider.GetCurrentUserInfo();
            var principal = currentUser.Principal;
            await _viewAsOrganisation.SetProviderInfo(principal, ukprn, providerName);

            return RedirectToAction(
                nameof(OrganisationController.Home),
                NameOf<OrganisationController>(),
                new { Area = string.Empty });
        }

        /// <summary>
        /// Action for stopping view as an organisation.
        /// </summary>
        /// <returns>Redirects to the organisation search page.</returns>
        public async Task<IActionResult> StopViewingAsAnOrganisation()
        {
            var currentUser = await UserInformationProvider.GetCurrentUserInfo();
            var principal = currentUser.Principal;
            await _viewAsOrganisation.ClearUkprn(principal);

            return RedirectToAction(
                nameof(OrganisationSearchController.SearchForAnOrganisation),
                NameOf<OrganisationSearchController>());
        }
    }
}