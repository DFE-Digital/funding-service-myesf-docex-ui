using Microsoft.AspNetCore.Mvc.Rendering;
using Pds.Core.Web.Administration.Models;
using Pds.Core.Web.Models;
using Pds.Core.Web.Models.Hyperlinks;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace Pds.DocumentExchange.Web.Areas.Admin.Models
{
    /// <summary>
    /// View model for add edit page.
    /// </summary>
    public class AddEditPageViewModel : BaseSettingEditPageViewModel, ISetBaseViewModelProperties
    {
        private string
            _headerTitle,
            _headerLink,
            _exitViewAsOrganisationLink,
            _logoutLink,
            _tAndCsLink,
            _viewYourSubservicesLink,
            _viewYourSubServicesText;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddEditPageViewModel"/> class.
        /// </summary>
        /// <param name="editSetting">The base edit settings.</param>
        /// <param name="isAddPage">The is add page flag.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="teamsList">The teams list.</param>
        public AddEditPageViewModel(BaseSettingEditViewModel editSetting, IEnumerable<AgencyTeam> teamsList, bool isAddPage, string identifier = "")
            : base(editSetting)
        {
            AgencyTeams = GetAgencyTeamsDropDownList(teamsList);
            IsAddPage = isAddPage;
            Identifier = identifier;
        }

        /// <summary>
        /// Gets or sets the teams dropdown.
        /// </summary>
        public IEnumerable<SelectListItem> AgencyTeams { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the page is add or edit.
        /// </summary>
        public bool IsAddPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether page has errors.
        /// </summary>
        public bool Error { get; set; }

        /// <summary>
        /// Gets or sets product Identifier.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether product Identifier is valid.
        /// </summary>
        public bool IdentifierError { get; set; }

        /// <summary>
        /// Gets or sets product Identifier error message.
        /// </summary>
        public string IdentifierErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets product old Identifier.
        /// </summary>
        public string OldIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the product name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether product name is valid.
        /// </summary>
        public bool NameError { get; set; }

        /// <summary>
        /// Gets or sets the product name error message.
        /// </summary>
        public string NameErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the plural form of product name.
        /// </summary>
        public string PluralName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether product plural name is valid.
        /// </summary>
        public bool PluralNameError { get; set; }

        /// <summary>
        /// Gets or sets product plural name error  message.
        /// </summary>
        public string PluralNameErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the agency teams.
        /// </summary>
        public string AgencyTeam { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether organisations can upload.
        /// </summary>
        public string CanOrganisationsUpload { get; set; }

        /// <summary>
        /// Gets the title. By default it will be used as both the browser title and content title.
        /// </summary>
        protected virtual string Title => IsAddPage ? "Add a New Product" : "Edit Product";

        private IList<BreadCrumbViewModel> _breadCrumbs = null;


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
        /// The teams dropdown list.
        /// </summary>
        /// <param name="teams">The teams list.</param>
        /// <returns>Returns the teams dropdown list.</returns>
        public IEnumerable<SelectListItem> GetAgencyTeamsDropDownList(IEnumerable<AgencyTeam> teams)
        {
            var list = teams.OrderBy(t => t.Name)
                .Select(t =>
                        new SelectListItem
                        {
                            Value = t.Identifier,
                            Text = t.Name
                        }).ToList();
            return new SelectList(list, "Value", "Text");
        }
    }
}