using Pds.Core.Web.Models.Hyperlinks;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Shared;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.DTOs;
using static Pds.DocumentExchange.Web.Helpers.ControllerHelper;

namespace Pds.DocumentExchange.Web.Attributes.BreadCrumbs.Organisation
{
    /// <summary>
    /// Class representing the 'send your document' breadcrumb attribute.
    /// </summary>
    public class SendYourDocumentBreadCrumbAttribute : BreadCrumbAttribute
    {
        private readonly bool _isCurrentPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendYourDocumentBreadCrumbAttribute"/> class.
        /// </summary>
        /// <param name="isCurrentPage"><inheritdoc cref="BreadCrumbViewModel.IsCurrentPage"/></param>
        public SendYourDocumentBreadCrumbAttribute(bool isCurrentPage)
        {
            _isCurrentPage = isCurrentPage;
        }

        /// <inheritdoc/>
        public override BreadCrumbAttribute GetParent(BreadCrumbData breadCrumbData)
        {
            if (!(breadCrumbData.PageViewModel is Models.Organisation.SendYourDocument sendYourDocumentViewModel))
            {
                throw new System.ArgumentException("View model was not of expected type.");
            }

            if (sendYourDocumentViewModel.ShowParentView)
            {
                return new SelectAnOrganisationBreadCrumbAttribute(false);
            }

            return new LandingBreadCrumbAttribute(false, breadCrumbData.UsingServiceStartPage);
        }

        /// <inheritdoc/>
        public override BreadCrumbViewModel ViewModel
            => new BreadCrumbViewModel
            {
                IsCurrentPage = _isCurrentPage,
                Link = new MvcActionLinkViewModel
                {
                    LinkText = "Send your document",
                    ControllerName = NameOf<OrganisationController>(),
                    ActionName = nameof(OrganisationController.SelectDocumentType)
                }
            };
    }
}