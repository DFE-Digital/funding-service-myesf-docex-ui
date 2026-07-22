using Pds.Core.Web.Models.Hyperlinks;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.DTOs;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Agency
{
    /// <summary>
    /// Class representing the 'are you sure you want to remove these documents' page breadcrumb attribute.
    /// </summary>
    public class RemoveDocumentsAreYouSureBreadCrumbAttribute : BreadCrumbAttribute
    {
        private readonly bool _isCurrentPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveDocumentsAreYouSureBreadCrumbAttribute"/> class.
        /// </summary>
        /// <param name="isCurrentPage"><inheritdoc cref="BreadCrumbViewModel.IsCurrentPage"/></param>
        public RemoveDocumentsAreYouSureBreadCrumbAttribute(bool isCurrentPage)
        {
            _isCurrentPage = isCurrentPage;
        }

        /// <inheritdoc/>
        public override BreadCrumbAttribute GetParent(BreadCrumbData breadCrumbData)
        {
            if (!(breadCrumbData.PageViewModel is Models.Agency.RemoveDocuments removeDocumentsViewModel))
            {
                throw new System.ArgumentException("View model was not of expected type.");
            }

            if (removeDocumentsViewModel.EntryAction.Equals(nameof(AgencyController.DocumentsToReview)))
            {
                return new DocumentsToReviewBreadCrumbAttribute(false);
            }

            if (removeDocumentsViewModel.EntryAction.Equals(nameof(AgencyController.DocumentsToPublish)))
            {
                return new DocumentsToPublishBreadCrumbAttribute(false);
            }

            throw new System.ArgumentException("Entry action of view model did not match any expected values.");
        }

        /// <inheritdoc/>
        public override BreadCrumbViewModel ViewModel
            => new BreadCrumbViewModel
            {
                IsCurrentPage = _isCurrentPage,
                Link = new MvcActionLinkViewModel
                {
                    LinkText = "Remove documents",
                    ControllerName = NameOf<AgencyController>(),
                    ActionName = nameof(AgencyController.RemoveDocumentsAreYouSure)
                }
            };
    }
}