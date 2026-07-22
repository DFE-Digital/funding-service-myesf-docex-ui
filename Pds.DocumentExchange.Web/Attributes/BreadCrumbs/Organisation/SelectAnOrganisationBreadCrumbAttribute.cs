using Pds.Core.Web.Models.Hyperlinks;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Shared;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.DTOs;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Organisation
{
    /// <summary>
    /// Class representing the 'select an organisation' breadcrumb attribute.
    /// </summary>
    public class SelectAnOrganisationBreadCrumbAttribute : BreadCrumbAttribute
    {
        private readonly bool _isCurrentPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectAnOrganisationBreadCrumbAttribute"/> class.
        /// </summary>
        /// <param name="isCurrentPage"><inheritdoc cref="BreadCrumbViewModel.IsCurrentPage"/></param>
        public SelectAnOrganisationBreadCrumbAttribute(bool isCurrentPage)
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
                    LinkText = "Select an organisation",
                    ControllerName = NameOf<OrganisationController>(),
                    ActionName = nameof(OrganisationController.SelectOrganisation)
                }
            };
    }
}