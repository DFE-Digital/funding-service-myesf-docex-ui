using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pds.DocumentExchange.Web.Authentication.Interfaces;
using System.Threading.Tasks;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Controllers
{
    /// <summary>
    /// The account controller.
    /// For local testing of authenticated user actions.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly ISignOutMethod _signOutMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="signOutMethod">The sign-out method.</param>
        public AccountController(ISignOutMethod signOutMethod)
        {
            _signOutMethod = signOutMethod;
        }

        private IActionResult RedirectToLandingPage
            => RedirectToAction(nameof(DocumentExchangeController.Landing), NameOf<DocumentExchangeController>());

        /// <summary>
        /// The login action.
        /// </summary>
        /// <returns>Redirect to the landing page.</returns>
        [Authorize]
        public IActionResult Login()
        {
            return RedirectToLandingPage;
        }

        /// <summary>
        /// The logout action.
        /// </summary>
        /// <returns>The <see cref="SignOutResult"/>.</returns>
        public async Task<IActionResult> Logout()
            => await _signOutMethod.SignOut(this);

        /// <summary>
        /// The post-logout action.
        /// </summary>
        /// <returns>Redirect to the landing page.</returns>
        public IActionResult PostLogout()
        {
            return RedirectToLandingPage;
        }

        /// <summary>
        /// The post-logout redirect action.
        /// </summary>
        /// <returns>Redirect to the landing page.</returns>
        public IActionResult PostLogoutRedirect()
        {
            return RedirectToLandingPage;
        }
    }
}