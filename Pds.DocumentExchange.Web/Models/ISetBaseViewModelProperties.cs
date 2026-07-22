using Pds.Core.Web.Models.Hyperlinks;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Models
{
    /// <summary>
    /// Exposes methods for setting base view model properties.
    /// </summary>
    public interface ISetBaseViewModelProperties
    {
        /// <summary>
        /// Sets the list of breadcrumbs for the page.
        /// </summary>
        /// <param name="breadCrumbs">The list of breadcrumbs to set.</param>
        void SetBreadCrumbs(IList<BreadCrumbViewModel> breadCrumbs);

        /// <summary>
        /// Sets the title and link of the page header.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="link">The link.</param>
        void SetHeader(string title, string link);

        /// <summary>
        /// Sets the link to allow the user to stop viewing as an organisation.
        /// </summary>
        /// <param name="link">The link.</param>
        void SetExitViewAsOrganisationLink(string link);

        /// <summary>
        /// Sets the link to allow the user to sign out.
        /// </summary>
        /// <param name="link">The link.</param>
        void SetLogoutLink(string link);

        /// <summary>
        /// Sets the link to the terms and conditions page.
        /// </summary>
        /// <param name="link">The link.</param>
        void SetTermsAndConditionsLink(string link);

        /// <summary>
        /// Sets the link to the view your subservices page.
        /// </summary>
        /// <param name="link">The link.</param>
        void SetViewYourSubServicesLink(string link);

        /// <summary>
        /// Sets the text to the view your subservices page.
        /// </summary>
        /// <param name="text">The text.</param>
        void SetViewYourSubServicesText(string text);
    }
}