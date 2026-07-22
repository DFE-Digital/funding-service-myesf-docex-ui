using Pds.Core.Web.Models;
using Pds.Core.Web.Models.Hyperlinks;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Models
{
    /// <summary>
    /// A base view model for holding the shared properties of the Document Exchange pages.
    /// </summary>
    public abstract class BaseDocumentExchangePageViewModel : BasePageViewModel, ISetBaseViewModelProperties
    {
        private IList<BreadCrumbViewModel> _breadCrumbs = null;

        private string
            _headerTitle,
            _headerLink,
            _exitViewAsOrganisationLink,
            _logoutLink,
            _tAndCsLink,
            _viewYourSubservicesLink,
            _viewYourSubServicesText;

        #region Base view model overrides

        /// <inheritdoc/>
        public override string HeaderTitle => _headerTitle ?? "Manage your education and skills funding";

        /// <inheritdoc/>
        public override string HeaderLink => _headerLink ?? "/";

        /// <inheritdoc/>
        public override bool ShowHeaderTitle => true;

        /// <inheritdoc/>
        public override bool ShowUkprnAndSchool => false;

        /// <inheritdoc/>
        public override string ExitViewAsOrganisationLink => _exitViewAsOrganisationLink ?? base.ExitViewAsOrganisationLink;

        /// <inheritdoc/>
        public override string LogoutLink => _logoutLink ?? base.LogoutLink;

        /// <inheritdoc/>
        public override string ContentTitle => BasePageViewModel.AddHTMLExpansions(Title);

        /// <inheritdoc/>
        public override string BrowserTitle => Title;

        /// <inheritdoc/>
        public override bool ShowContentTitle => true;

        /// <inheritdoc/>
        public override bool ShowBetaTag => true;

        /// <inheritdoc/>
        public override bool ShowFeedbackLink => true;

        /// <inheritdoc/>
        public override IList<BreadCrumbViewModel> BreadCrumbItems
            => _breadCrumbs ?? new List<BreadCrumbViewModel>();

        /// <inheritdoc/>
        public override string TandCsLink => _tAndCsLink ?? base.TandCsLink;

        /// <inheritdoc/>
        public override string ViewYourSubServicesLink => _viewYourSubservicesLink ?? base.ViewYourSubServicesLink;

        /// <inheritdoc/>
        public override string ViewYourSubServicesText => _viewYourSubServicesText ?? base.ViewYourSubServicesText;

        #endregion


        #region Setters

        /// <inheritdoc/>
        public void SetBreadCrumbs(IList<BreadCrumbViewModel> breadCrumbs)
            => _breadCrumbs = breadCrumbs;

        /// <inheritdoc/>
        public void SetHeader(string title, string link)
        {
            _headerTitle = title;
            _headerLink = link;
        }

        /// <inheritdoc/>
        public void SetExitViewAsOrganisationLink(string link)
        {
            _exitViewAsOrganisationLink = link;
        }

        /// <inheritdoc/>
        public void SetLogoutLink(string link)
        {
            _logoutLink = link;
        }

        /// <inheritdoc/>
        public void SetTermsAndConditionsLink(string link)
        {
            _tAndCsLink = link;
        }

        /// <inheritdoc/>
        public void SetViewYourSubServicesLink(string link)
        {
            _viewYourSubservicesLink = link;
        }

        /// <inheritdoc/>
        public void SetViewYourSubServicesText(string text)
        {
            _viewYourSubServicesText = text;
        }

        #endregion

        /// <summary>
        /// Gets the title. By default it will be used as both the browser title and content title.
        /// </summary>
        protected virtual string Title => ServiceConstants.ServiceName;
    }
}