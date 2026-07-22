using Pds.Core.Web.Models.Hyperlinks;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.DTOs;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Shared
{
    /// <summary>
    /// Class representing the landing page breadcrumb attribute.
    /// </summary>
    public class LandingBreadCrumbAttribute : BreadCrumbAttribute
    {
        private readonly bool _isCurrentPage, _usingServiceStartPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="LandingBreadCrumbAttribute"/> class.
        /// </summary>
        /// <param name="isCurrentPage"><inheritdoc cref="BreadCrumbViewModel.IsCurrentPage"/></param>
        /// <param name="usingServiceStartPage">A value indicating whether the service is using its own service start page.</param>
        public LandingBreadCrumbAttribute(bool isCurrentPage, bool usingServiceStartPage = false)
        {
            _isCurrentPage = isCurrentPage;
            _usingServiceStartPage = usingServiceStartPage;
        }

        /// <inheritdoc/>
        public override BreadCrumbAttribute GetParent(BreadCrumbData breadCrumbData)
        {
            return breadCrumbData.UsingServiceStartPage
                ? (BreadCrumbAttribute)null
                : new HomeBreadCrumbAttribute();
        }

        /// <inheritdoc/>
        public override BreadCrumbViewModel ViewModel
            => new BreadCrumbViewModel
            {
                IsCurrentPage = _isCurrentPage,
                Link = new MvcActionLinkViewModel
                {
                    LinkText = _usingServiceStartPage ? HomeBreadCrumbText : ServiceConstants.ServiceName,
                    ControllerName = NameOf<DocumentExchangeController>(),
                    ActionName = nameof(DocumentExchangeController.Landing)
                }
            };
    }
}