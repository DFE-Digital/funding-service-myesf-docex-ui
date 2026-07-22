using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pds.DocumentExchange.Web.Areas.Admin.Controllers;
using Pds.DocumentExchange.Web.Areas.ViewAsOrganisation.Controllers;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Shared;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models;
using Pds.DocumentExchange.Web.Models.DocumentExchange;
using System.Threading.Tasks;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Controllers
{
    /// <summary>
    /// The main Document Exchange controller.
    /// </summary>
    public class DocumentExchangeController : BaseDocumentExchangeController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentExchangeController"/> class.
        /// </summary>
        /// <param name="userInformationProvider"><see cref="IUserInformationProvider"/>.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        public DocumentExchangeController(
            IUserInformationProvider userInformationProvider,
            IOptions<DocumentExchangeConfiguration> configurationOptions)
            : base(userInformationProvider, configurationOptions)
        {
        }

        /// <summary>
        /// The landing page / service start page action.
        /// </summary>
        /// <returns>If the user is not logged in and the start page is enabled, the start page view is returned.
        /// Otherwise, redirects to the relevant action depending on the user.</returns>
        [Route("/")]
        [Route("/start")]
        public async Task<IActionResult> Landing()
        {
            var currentUser = await UserInformationProvider.GetCurrentUserViewModel();

            if (!currentUser.IsLoggedIn)
            {
                if (Configuration.ShowServiceStartPage)
                {
                    return View(new Landing());
                }
                else
                {
                    return RedirectToAction(nameof(AccountController.Login), NameOf<AccountController>());
                }
            }

            if (!await UserInformationProvider.CurrentUserCanAccessDocumentExchange())
            {
                return Unauthorized();
            }

            if (await UserInformationProvider.CurrentUserIsOrganisationUserOrImpersonating())
            {
                return RedirectToAction(nameof(OrganisationController.Home), NameOf<OrganisationController>());
            }

            if (!string.IsNullOrEmpty(await UserInformationProvider.GetCurrentUserAgencyTeams()))
            {
                return RedirectToAgencyTilePage();
            }

            var isAdminUser = await UserInformationProvider.CurrentUserIsAdminUser();
            var isViewAsOrganisationUser = Configuration.ShowServiceStartPage
                && await UserInformationProvider.CurrentUserCanViewAsOrganisation();

            if (isAdminUser)
            {
                if (isViewAsOrganisationUser)
                {
                    return RedirectToAgencyTilePage();
                }

                return RedirectToAction(nameof(SettingsController.Index), NameOf<SettingsController>());
            }

            if (isViewAsOrganisationUser)
            {
                return RedirectToAction(
                    nameof(OrganisationSearchController.SearchForAnOrganisation),
                    NameOf<OrganisationSearchController>(),
                    new { Area = Areas.ViewAsOrganisation.Constants.AreaName });
            }

            return Unauthorized();
        }

        /// <summary>
        /// The user guide page action.
        /// </summary>
        /// <returns>The user guide page view.</returns>
        public async Task<IActionResult> UserGuide()
            => await Task.FromResult(View(new UserGuide()));

        /// <summary>
        /// The terms and conditions page action.
        /// </summary>
        /// <returns>The terms and conditions page view.</returns>
        [TermsAndConditionsBreadCrumb(true)]
        public async Task<IActionResult> TermsAndConditions()
            => await Task.FromResult(View(new TermsAndConditions()));

        /// <summary>
        /// The account settings page action.
        /// </summary>
        /// <returns>The account settings page view.</returns>
        public async Task<IActionResult> AccountSettings()
        {
            if (!await UserInformationProvider.CurrentUserCanAccessDocumentExchange())
            {
                return Unauthorized();
            }

            var permissions = await UserInformationProvider.GetCurrentUserPermissions();
            var model = new AccountSettings
            {
                Permissions = permissions
            };

            return View(model);
        }

        private RedirectToActionResult RedirectToAgencyTilePage()
            => RedirectToAction(nameof(AgencyController.AgencyHome), NameOf<AgencyController>());
    }
}