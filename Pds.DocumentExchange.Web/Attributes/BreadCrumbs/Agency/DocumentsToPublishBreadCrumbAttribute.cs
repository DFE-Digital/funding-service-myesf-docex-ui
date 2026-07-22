using Pds.Core.Web.Models.Hyperlinks;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.DTOs;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Agency
{
    /// <summary>
    /// Class representing the 'documents to publish' page breadcrumb attribute.
    /// </summary>
    public class DocumentsToPublishBreadCrumbAttribute : BreadCrumbAttribute
    {
        private readonly bool _isCurrentPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentsToPublishBreadCrumbAttribute"/> class.
        /// </summary>
        /// <param name="isCurrentPage"><inheritdoc cref="BreadCrumbViewModel.IsCurrentPage"/></param>
        public DocumentsToPublishBreadCrumbAttribute(bool isCurrentPage)
        {
            _isCurrentPage = isCurrentPage;
        }

        /// <inheritdoc/>
        public override BreadCrumbAttribute GetParent(BreadCrumbData breadCrumbData)
            => new FileShareBreadCrumbAttribute(false);

        /// <inheritdoc/>
        public override BreadCrumbViewModel ViewModel
            => new BreadCrumbViewModel
            {
                IsCurrentPage = _isCurrentPage,
                Link = new MvcActionLinkViewModel
                {
                    LinkText = "Publish your documents",
                    ControllerName = NameOf<AgencyController>(),
                    ActionName = nameof(AgencyController.DocumentsToPublish)
                }
            };
    }
}