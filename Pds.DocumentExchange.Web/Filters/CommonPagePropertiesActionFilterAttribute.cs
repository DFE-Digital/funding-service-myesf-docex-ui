using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Pds.Core.Web.Models;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.Models;
using System.Linq;
using System.Threading.Tasks;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;
using ViewAsOrganisation = Pds.DocumentExchange.Web.Areas.ViewAsOrganisation;

namespace Pds.DocumentExchange.Web.Filters
{
    /// <summary>
    /// Action filter attribute for setting the properties common to all pages.
    /// </summary>
    public class CommonPagePropertiesActionFilterAttribute : ActionFilterAttribute
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly DocumentExchangeConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonPagePropertiesActionFilterAttribute"/> class.
        /// </summary>
        /// <param name="urlHelperFactory"><see cref="IUrlHelperFactory"/>.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        public CommonPagePropertiesActionFilterAttribute(
            IUrlHelperFactory urlHelperFactory,
            IOptions<DocumentExchangeConfiguration> configurationOptions)
        {
            _urlHelperFactory = urlHelperFactory;
            _configuration = configurationOptions.Value;
        }

        /// <inheritdoc/>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionExecutedContext = await next();

            SetBreadCrumbs(actionExecutedContext);
            SetHeaderAndFooterProperties(actionExecutedContext);
        }

        private void SetBreadCrumbs(ActionExecutedContext context)
        {
            if (!(context.Result is ViewResult viewResult) ||
                !(viewResult.ViewData?.Model is ISetBaseViewModelProperties model))
            {
                return;
            }

            foreach (var filter in context.Filters ?? Enumerable.Empty<IFilterMetadata>())
            {
                if (filter is BreadCrumbAttribute breadCrumb)
                {
                    model.SetBreadCrumbs(breadCrumb.GetPath(new DTOs.BreadCrumbData
                    {
                        PageViewModel = model,
                        UsingServiceStartPage = _configuration.ShowServiceStartPage
                    }));

                    return;
                }
            }
        }

        private void SetHeaderAndFooterProperties(ActionExecutedContext context)
        {
            if (context.Result is ViewResult viewResult &&
                viewResult.ViewData?.Model is BasePageViewModel model)
            {
                model.FeedbackLink = _configuration.FeedbackLinkUrl;
                model.MSClarityId = _configuration.MSClarityId;

                if (_configuration.ShowServiceStartPage && model is ISetBaseViewModelProperties setterModel)
                {
                    setterModel.SetHeader(ServiceConstants.ServiceName, ServiceConstants.PathBase);

                    var urlHelper = _urlHelperFactory.GetUrlHelper(context);

                    // This code might have to be moved later if/when DocEx moves back inside of MYESF and both are using DSI:
                    var viewAsOrganisation = urlHelper.ActionLink(
                        nameof(ViewAsOrganisation.Controllers.ViewAsOrganisationController.StopViewingAsAnOrganisation),
                        NameOf<ViewAsOrganisation.Controllers.ViewAsOrganisationController>(),
                        new { Area = ViewAsOrganisation.Constants.AreaName });

                    setterModel.SetExitViewAsOrganisationLink(viewAsOrganisation);

                    setterModel.SetLogoutLink($"{ServiceConstants.PathBase}/account/logout");

                    var termsAndConditions = urlHelper.ActionLink(
                        nameof(DocumentExchangeController.TermsAndConditions),
                        NameOf<DocumentExchangeController>());

                    setterModel.SetTermsAndConditionsLink(termsAndConditions);

                    var accountSettings = urlHelper.ActionLink(
                        nameof(DocumentExchangeController.AccountSettings),
                        NameOf<DocumentExchangeController>());

                    setterModel.SetViewYourSubServicesText("Settings");
                    setterModel.SetViewYourSubServicesLink(accountSettings);
                }
            }
        }
    }
}