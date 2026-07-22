using Pds.Core.Common.Organisation.Models;
using Pds.Core.Web.Models;
using Pds.DocumentExchange.Services.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Interfaces.Providers
{
    /// <summary>
    /// Provides user information.
    /// </summary>
    public interface IUserInformationProvider
    {
        #region Common user methods

        /// <summary>
        /// Initialises the provider with the given <see cref="ClaimsPrincipal"/>.
        /// </summary>
        /// <param name="claimsPrincipal">The claims principal.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Initialise(ClaimsPrincipal claimsPrincipal);

        /// <summary>
        /// Gets the current user information.
        /// </summary>
        /// <returns>A <see cref="UserInfo"/> object representing the current user.</returns>
        Task<UserInfo> GetCurrentUserInfo();

        /// <summary>
        /// Gets the current user view model.
        /// </summary>
        /// <returns>A <see cref="CurrentUserViewModel"/> object representing the current user.</returns>
        Task<CurrentUserViewModel> GetCurrentUserViewModel();

        /// <summary>
        /// Gets a value indicating whether the current user has access to the document exchange service.
        /// </summary>
        /// <returns>True if the user has access, otherwise false.</returns>
        Task<bool> CurrentUserCanAccessDocumentExchange();

        /// <summary>
        /// Gets the list of permissions for the current user.
        /// </summary>
        /// <returns>The list of permissions.</returns>
        Task<IEnumerable<string>> GetCurrentUserPermissions();

        /// <summary>
        /// Gets a value indicating whether the current user is an external user.
        /// </summary>
        /// <returns>True is the current user is an external user. False is the user is internal. </returns>
        bool IsCurrentUserExternal();

        #endregion


        #region Organisation user methods

        /// <summary>
        /// Gets a value indicating whether the current user is an organisation user,
        /// or is impersonating an organisation user.
        /// </summary>
        /// <returns>A boolean value indicating whether the current user is an organisation user,
        /// or is impersonating an organisation user.</returns>
        Task<bool> CurrentUserIsOrganisationUserOrImpersonating();

        /// <summary>
        /// Gets the identifier for the current user's organisation.
        /// </summary>
        /// <returns>The identifier for the current user's organisation.</returns>
        Task<OrganisationIdentifier> GetCurrentOrganisationIdentifier();

        /// <summary>
        /// Gets a value indicating whether or not the current user should see parent views.
        /// </summary>
        /// <returns>A value indicating whether or not the current user should see parent views.</returns>
        Task<bool> CurrentUserShouldSeeParentView();

        /// <summary>
        /// Gets the child organisations of the current user's organisation.
        /// </summary>
        /// <returns>The collection of child organisations of the current user's organisation.</returns>
        Task<IEnumerable<Organisation>> GetCurrentUserChildOrganisations();

        #endregion


        #region Agency user methods

        /// <summary>
        /// Gets the agency teams for the current user as a csv string,
        /// or a default value if the current user is not an agency user.
        /// </summary>
        /// <returns>The csv string containing the agency teams for the current user, if applicable.</returns>
        Task<string> GetCurrentUserAgencyTeams();

        /// <summary>
        /// Gets a value indicating whether the current user is an advanced agency user.
        /// </summary>
        /// <returns>A value indicating whether the current user is an advanced agency user.</returns>
        Task<bool> CurrentUserIsAdvancedAgencyUser();

        /// <summary>
        /// Gets a value indicating whether the current user can view as an organisation.
        /// </summary>
        /// <returns>A value indicating whether the current user can view as an organisation.</returns>
        Task<bool> CurrentUserCanViewAsOrganisation();

        /// <summary>
        /// Gets a value indicating whether the current user can view as an organisation.
        /// </summary>
        /// <returns>A value indicating whether the current user can view as an organisation.</returns>
        Task<bool> CurrentUserCanAccessToSupportTools();

        #endregion


        #region Admin user methods

        /// <summary>
        /// Gets a value indicating whether the current user is a document exchange admin user.
        /// </summary>
        /// <returns>A value indicating whether the current user is a document exchange admin user.</returns>
        Task<bool> CurrentUserIsAdminUser();

        #endregion
    }
}