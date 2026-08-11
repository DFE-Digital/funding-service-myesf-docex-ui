//using AutoMapper;
using Pds.Core.Common.Identity.Models;
using Pds.Core.Common.Organisation.Models;
using Pds.Core.Identity.Claims.Interfaces;
using Pds.Core.Web.Models;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.Attributes;
using Pds.DocumentExchange.Web.Enums;
using Pds.DocumentExchange.Web.Extensions;
using Pds.DocumentExchange.Web.Helpers;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Implementations.Providers
{
    /// <inheritdoc cref="IUserInformationProvider"/>
    public class UserInformationProvider : IUserInformationProvider
    {
        private readonly IClaimsBasedIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly IOrganisationApiClient _organisationApiClient;

        private User _currentUser;
        private IEnumerable<Organisation> _currentUserChildOrganisations;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInformationProvider"/> class.
        /// </summary>
        /// <param name="identityService"><see cref="IClaimsBasedIdentityService"/>.</param>
        /// <param name="mapper"><see cref="IMapper"/>.</param>
        /// <param name="organisationApiClient"><see cref="IOrganisationApiClient"/>.</param>
        public UserInformationProvider(
            IClaimsBasedIdentityService identityService,
            IMapper mapper,
            IOrganisationApiClient organisationApiClient)
        {
            _identityService = identityService;
            _mapper = mapper;
            _organisationApiClient = organisationApiClient;
        }


        #region Common user methods

        /// <inheritdoc/>
        public async Task Initialise(ClaimsPrincipal claimsPrincipal)
        {
            _currentUser = await _identityService.GetUserFromClaims(claimsPrincipal);
            _currentUserChildOrganisations = null;
        }

        /// <inheritdoc/>
        public async Task<UserInfo> GetCurrentUserInfo()

            //=> await Task.Run(() => _mapper.Map<UserInfo>(_currentUser));
            //=> await Task.Run(() => _currentUser.ToUserInfo());
            => await Task.Run(() => _mapper.ToUserInfo(_currentUser));

        /// <inheritdoc/>
        public async Task<CurrentUserViewModel> GetCurrentUserViewModel()

            //=> await Task.Run(() => _mapper.Map<CurrentUserViewModel>(_currentUser));
            //=> await Task.Run(() => _currentUser.ToCurrentUserViewModel());
            => await Task.Run(() => _mapper.ToCurrentUserViewModel(_currentUser));

        /// <inheritdoc />
        public async Task<bool> CurrentUserCanAccessDocumentExchange()
            => await Task.Run(
                () => _currentUser?.Roles?.Intersect(AuthorizationHelper.DocumentExchangeRoles).Any() == true);

        /// <summary>
        /// checks if current user can access support tools.
        /// </summary>
        /// <returns>Bool for current user can access support tools.</returns>
        public async Task<bool> CurrentUserCanAccessToSupportTools()
            => await Task.Run(
                () => _currentUser?.Roles?.Intersect(AuthorizationHelper.DocumentExchangeInternalUserRoles).Any() == true);

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetCurrentUserPermissions()
        {
            var currentUserType = await GetCurrentUserType();

            var attributes = Enum.GetValues(typeof(DocumentExchangeUserType))
                .Cast<DocumentExchangeUserType>()
                .Where(userType => currentUserType.HasFlag(userType))
                .Select(userType => typeof(DocumentExchangeUserType).GetField(userType.ToString()))
                .SelectMany(field => field.GetCustomAttributes(typeof(UserPermissionAttribute), false))
                .Cast<UserPermissionAttribute>();

            return attributes.Select(attribute => attribute.Value);
        }

        /// <inheritdoc/>
        public bool IsCurrentUserExternal()
            => _currentUser.IsExternalUser;

        #endregion


        #region Organisation user methods

        /// <inheritdoc/>
        public async Task<bool> CurrentUserIsOrganisationUserOrImpersonating()
        {
            return await Task.Run(
                () =>
                {
                    if (_currentUser?.IsAuthenticated != true)
                    {
                        return false;
                    }

                    // User is organisation user.
                    if (_currentUser.IsExternalUser)
                    {
                        return true;
                    }

                    // User is impersonating an organisation user.
                    if (_currentUser.CanViewAsOrganisation &&
                        _currentUser.Ukprn.HasValue)
                    {
                        return true;
                    }

                    return false;
                });
        }

        /// <inheritdoc/>
        public async Task<OrganisationIdentifier> GetCurrentOrganisationIdentifier()
        {
            var currentUserInfo = await GetCurrentUserInfo();

            if (string.IsNullOrWhiteSpace(currentUserInfo?.OrganisationInfo?.OrganisationIdentifier?.Value))
            {
                throw new InvalidOperationException(
                    $"Missing organisation identifier for user {currentUserInfo?.Principal}");
            }

            return currentUserInfo.OrganisationInfo.OrganisationIdentifier;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Organisation>> GetCurrentUserChildOrganisations()
        {
            if (_currentUserChildOrganisations == null)
            {
                var currentUserOrganisationId = await GetCurrentOrganisationIdentifier();
                var currentUserOrganisation = await _organisationApiClient.GetOrganisation(currentUserOrganisationId);

                _currentUserChildOrganisations = currentUserOrganisation?.ChildOrganisations;
            }

            return _currentUserChildOrganisations;
        }

        /// <inheritdoc/>
        public async Task<bool> CurrentUserShouldSeeParentView()
            => (await GetCurrentUserChildOrganisations())?.Any() == true;

        #endregion


        #region Agency user methods

        /// <inheritdoc/>
        public async Task<bool> CurrentUserIsAdvancedAgencyUser()
            => await Task.Run(
                () => _currentUser?.Roles?.Contains(AuthorizationHelper.DocumentExchangeAgencyAdvancedUserRole) ==
                      true);

        /// <inheritdoc/>
        public async Task<string> GetCurrentUserAgencyTeams()
        {
            var teams = await GetCurrentUserAgencyTeamsList();

            return string.Join(',', teams ?? Enumerable.Empty<string>());
        }

        /// <inheritdoc/>
        public async Task<bool> CurrentUserCanViewAsOrganisation()
            => await Task.Run(
                () => _currentUser?.Roles?.Contains(AuthorizationHelper.ViewAsOrganisationRole) ==
                      true);

        #endregion


        #region Admin user methods

        /// <inheritdoc />
        public async Task<bool> CurrentUserIsAdminUser()
            => await Task.Run(
                () => _currentUser?.Roles?.Contains(AuthorizationHelper.DocumentExchangeAdminRole) == true);

        #endregion


        #region Helper methods

        private async Task<IEnumerable<string>> GetCurrentUserAgencyTeamsList()
        {
            if (_currentUser?.IsAuthenticated != true ||
                _currentUser.IsExternalUser)
            {
                return null;
            }

            if (await CurrentUserIsAdvancedAgencyUser())
            {
                return AuthorizationHelper.DocumentExchangeAgencyTeamRoles;
            }

            return await Task.Run(
                () => _currentUser.Roles?.Intersect(AuthorizationHelper.DocumentExchangeAgencyTeamRoles));
        }

        private async Task<DocumentExchangeUserType> GetCurrentUserType()
        {
            var currentUserType = DocumentExchangeUserType.None;

            if (await CurrentUserIsAdvancedAgencyUser())
            {
                currentUserType |= DocumentExchangeUserType.Advanced;
            }
            else if (!string.IsNullOrEmpty(await GetCurrentUserAgencyTeams()))
            {
                currentUserType |= DocumentExchangeUserType.AgencyTeam;
            }

            if (await CurrentUserIsAdminUser())
            {
                currentUserType |= DocumentExchangeUserType.Admin;
            }

            if (await CurrentUserCanViewAsOrganisation())
            {
                currentUserType |= DocumentExchangeUserType.ViewAsOrganisation;
            }
            else if (await CurrentUserIsOrganisationUserOrImpersonating())
            {
                var parentUser = await CurrentUserShouldSeeParentView();
                currentUserType |= parentUser
                    ? DocumentExchangeUserType.ParentOrganisation
                    : DocumentExchangeUserType.ChildOrganisation;
            }

            return currentUserType;
        }

        #endregion
    }
}