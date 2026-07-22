using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Pds.DocumentExchange.Web.Authentication.Interfaces;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Authentication.Implementations
{
    /// <summary>
    /// The DfE Sign-in sign out method.
    /// </summary>
    public class DfESignInSignOutMethod : ISignOutMethod
    {
        /// <inheritdoc/>
        public async Task<IActionResult> SignOut(Controller controller)
        {
            var result = controller.SignOut(
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);

            return await Task.FromResult<ActionResult>(result);
        }
    }
}