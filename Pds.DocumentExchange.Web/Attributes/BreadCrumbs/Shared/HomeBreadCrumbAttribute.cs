using Pds.Core.Web.Models.Hyperlinks;
using Pds.DocumentExchange.Web.DTOs;

namespace Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Shared
{
    /// <summary>
    /// Class representing the home breadcrumb attribute.
    /// </summary>
    public class HomeBreadCrumbAttribute : BreadCrumbAttribute
    {
        /// <inheritdoc/>
        public override BreadCrumbAttribute GetParent(BreadCrumbData breadCrumbData)
            => null;

        /// <inheritdoc/>
        public override BreadCrumbViewModel ViewModel
            => new BreadCrumbViewModel
            {
                IsCurrentPage = false,
                Link = new UrlLinkViewModel
                {
                    LinkText = HomeBreadCrumbText,
                    Url = "/"
                }
            };
    }
}