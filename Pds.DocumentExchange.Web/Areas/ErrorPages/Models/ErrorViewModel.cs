using Pds.Core.Web.Areas.ErrorPages.Models;
using Pds.Core.Web.Models.Hyperlinks;
using Pds.DocumentExchange.Web.Models;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Areas.ErrorPages.Models
{
    /// <summary>
    /// View model for an error page.
    /// </summary>
    public class ErrorViewModel : DefaultErrorViewModel, ISetBaseViewModelProperties
    {
        private string
            _headerTitle,
            _headerLink,
            _exitViewAsOrganisationLink,
            _logoutLink,
            _tAndCsLink,
            _viewYourSubservicesLink,
            _viewYourSubServicesText;

        private IList<BreadCrumbViewModel> _breadCrumbs = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorViewModel"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        public ErrorViewModel(int statusCode) : base(statusCode)
        {
        }

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
        public void SetBreadCrumbs(IList<BreadCrumbViewModel> breadCrumbs)
            => _breadCrumbs = breadCrumbs;

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
    }
}