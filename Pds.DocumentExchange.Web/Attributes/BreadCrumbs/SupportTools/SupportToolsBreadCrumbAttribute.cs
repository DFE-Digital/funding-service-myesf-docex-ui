using Pds.Core.Web.Models.Hyperlinks;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Shared;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.DTOs;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Attributes.BreadCrumbs.SupportTools
{
    /// <summary>
    /// Class representing the 'support tools' page breadcrumb attribute.
    /// </summary>
    public class SupportToolsBreadCrumbAttribute : BreadCrumbAttribute
    {
        private readonly bool _isCurrentPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportToolsBreadCrumbAttribute"/> class.
        /// </summary>
        /// <param name="isCurrentPage"><inheritdoc cref="BreadCrumbViewModel.IsCurrentPage"/></param>
        public SupportToolsBreadCrumbAttribute(bool isCurrentPage)
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
                    LinkText = "Document exchange support tools",
                    ControllerName = NameOf<SupportToolsController>(),
                    ActionName = nameof(SupportToolsController.SupportTools)
                }
            };
    }
}