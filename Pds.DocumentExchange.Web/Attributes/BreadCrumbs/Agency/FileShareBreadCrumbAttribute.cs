using Pds.Core.Web.Models.Hyperlinks;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Shared;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.DTOs;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Agency
{
    /// <summary>
    /// Class representing the file share breadcrumb attribute.
    /// </summary>
    public class FileShareBreadCrumbAttribute : BreadCrumbAttribute
    {
        private readonly bool _isCurrentPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileShareBreadCrumbAttribute"/> class.
        /// </summary>
        /// <param name="isCurrentPage"><inheritdoc cref="BreadCrumbViewModel.IsCurrentPage"/></param>
        public FileShareBreadCrumbAttribute(bool isCurrentPage)
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
                    LinkText = "File share documents",
                    ControllerName = NameOf<AgencyController>(),
                    ActionName = nameof(AgencyController.FileShare)
                }
            };
    }
}