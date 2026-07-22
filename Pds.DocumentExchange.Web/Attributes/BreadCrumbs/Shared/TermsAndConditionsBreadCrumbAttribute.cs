using Pds.Core.Web.Models.Hyperlinks;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.DTOs;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Shared
{
    /// <summary>
    /// Class representing the terms and conditions page breadcrumb attribute.
    /// </summary>
    public class TermsAndConditionsBreadCrumbAttribute : BreadCrumbAttribute
    {
        private readonly bool _isCurrentPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="TermsAndConditionsBreadCrumbAttribute"/> class.
        /// </summary>
        /// <param name="isCurrentPage"><inheritdoc cref="BreadCrumbViewModel.IsCurrentPage"/></param>
        public TermsAndConditionsBreadCrumbAttribute(bool isCurrentPage)
        {
            _isCurrentPage = isCurrentPage;
        }

        /// <inheritdoc/>
        public override BreadCrumbAttribute GetParent(BreadCrumbData breadCrumbData)
            => new LandingBreadCrumbAttribute(false, breadCrumbData.UsingServiceStartPage);

        /// <inheritdoc/>
        public override BreadCrumbViewModel ViewModel
            => new BreadCrumbViewModel
            {
                IsCurrentPage = _isCurrentPage,
                Link = new MvcActionLinkViewModel
                {
                    LinkText = "Terms and conditions",
                    ControllerName = NameOf<DocumentExchangeController>(),
                    ActionName = nameof(DocumentExchangeController.TermsAndConditions)
                }
            };
    }
}