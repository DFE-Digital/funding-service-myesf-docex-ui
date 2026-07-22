using Pds.Core.Web.Models.Hyperlinks;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.DTOs;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Attributes.BreadCrumbs.SupportTools
{
    /// <summary>
    /// Class representing the 'Delete a published document' page breadcrumb attribute.
    /// </summary>
    public class DeletePublishedDocumentAreYouSureBreadCrumbAttribute : BreadCrumbAttribute
    {
        private readonly bool _isCurrentPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeletePublishedDocumentAreYouSureBreadCrumbAttribute"/> class.
        /// </summary>
        /// <param name="isCurrentPage"><inheritdoc cref="BreadCrumbViewModel.IsCurrentPage"/></param>
        public DeletePublishedDocumentAreYouSureBreadCrumbAttribute(bool isCurrentPage)
        {
            _isCurrentPage = isCurrentPage;
        }

        /// <inheritdoc/>
        public override BreadCrumbAttribute GetParent(BreadCrumbData breadCrumbData)
         => new DeleteDocumentBreadCrumbAttribute(false);

        /// <inheritdoc/>
        public override BreadCrumbViewModel ViewModel
            => new BreadCrumbViewModel
            {
                IsCurrentPage = _isCurrentPage,
                Link = new MvcActionLinkViewModel
                {
                    LinkText = "Delete a published document",
                    ControllerName = NameOf<SupportToolsController>(),
                    ActionName = nameof(SupportToolsController.DeletePublishedDocument)
                }
            };
    }
}