using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Pds.Core.Web.Models;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Filters
{
    /// <summary>
    /// Action filter attribute for setting the user context.
    /// </summary>
    public class UserContextActionFilterAttribute : ActionFilterAttribute
    {
        private readonly IUserInformationProvider _userInformationProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserContextActionFilterAttribute"/> class.
        /// </summary>
        /// <param name="userInformationProvider"><see cref="IUserInformationProvider"/>.</param>
        public UserContextActionFilterAttribute(
            IUserInformationProvider userInformationProvider)
        {
            _userInformationProvider = userInformationProvider;
        }

        /// <inheritdoc/>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = ((Controller)context.Controller).User;

            await _userInformationProvider.Initialise(user);

            var actionExecutedContext = await next();

            await SetCurrentUser(actionExecutedContext);
        }

        private async Task SetCurrentUser(ActionExecutedContext context)
        {
            if (context.Result is ViewResult viewResult &&
                viewResult.ViewData?.Model is BasePageViewModel model)
            {
                model.CurrentUser ??= await _userInformationProvider.GetCurrentUserViewModel();
            }
        }
    }
}