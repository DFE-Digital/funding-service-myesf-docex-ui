using Pds.Core.Common.Identity.Models;
using Pds.Core.Common.Organisation.Enums;
using Pds.Core.Common.Organisation.Models;
using Pds.Core.Web.Models;
using Pds.DocumentExchange.Services.Models;

namespace Pds.DocumentExchange.Web.Extensions
{
    /// <summary>
    /// Extension class to map <see cref="Pds.Core.Common.Identity.Models.User"/> to other classes.
    /// </summary>
    public static class UserMappings
    {
        /// <summary>
        /// Extension method to convert a User to a UserInfo.
        /// </summary>
        /// <param name="source">The User source object.</param>
        /// <returns>an UserInfo.</returns>
        public static Services.Models.UserInfo ToUserInfo(this User source)
        {
            if (source == null)
            {
                return null;
            }

            var destinationUserInfo = new UserInfo
            {
                Principal = source.Principal,
                FullName = source.FullName,
                EmailAddress = source.Email,
                OrganisationInfo = new OrganisationInfo
                {
                    OrganisationIdentifier = new OrganisationIdentifier
                    {
                        Value = source.Ukprn != null ? source.Ukprn.ToString() : null,
                        Type = OrganisationIdentifierType.Ukprn
                    },

                    //Name = source.ProviderName ?? null
                },
                IsViewAsOrganisation = !source.IsExternalUser && source.Ukprn.HasValue
            };
            return destinationUserInfo;
        }

        /// <summary>
        /// Extension method to convert a User to a CurrentUserViewModel.
        /// </summary>
        /// <param name="source">The User source object.</param>
        /// <returns>a CurrentUserViewModel.</returns>
        public static CurrentUserViewModel ToCurrentUserViewModel(this User source)
        {
            return new CurrentUserViewModel
            {
                IsLoggedIn = source.IsAuthenticated,
                IsExternalUser = source.IsExternalUser,
                Ukprn = source.Ukprn,
                ProviderName = source.ProviderName,
                CanViewAsOrganisation = source.CanViewAsOrganisation,
                FirstName = source.FirstName,
                LastName = source.LastName,
                FullName = source.FullName
            };
        }
    }
}