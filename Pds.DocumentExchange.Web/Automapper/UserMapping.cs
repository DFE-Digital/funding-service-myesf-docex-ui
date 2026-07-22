using AutoMapper;
using Pds.Core.Common.Identity.Models;
using Pds.Core.Common.Organisation.Enums;
using Pds.Core.Common.Organisation.Models;
using Pds.Core.Web.Models;
using Pds.DocumentExchange.Services.Models;

namespace Pds.DocumentExchange.Web.Automapper
{
    /// <summary>
    /// Automapper profile for mapping users.
    /// </summary>
    public class UserMapping : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserMapping"/> class.
        /// </summary>
        public UserMapping()
        {
            CreateMap<User, UserInfo>()
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.OrganisationInfo, opt => opt.MapFrom(src => new OrganisationInfo
                {
                    OrganisationIdentifier = new OrganisationIdentifier
                    {
                        Value = src.Ukprn != null ? src.Ukprn.ToString() : null,
                        Type = OrganisationIdentifierType.Ukprn
                    }
                }))
                .ForMember(dest => dest.IsViewAsOrganisation, opt => opt.MapFrom(src => !src.IsExternalUser && src.Ukprn.HasValue));

            CreateMap<User, CurrentUserViewModel>()
                .ForMember(dest => dest.IsLoggedIn, opt => opt.MapFrom(src => src.IsAuthenticated));
        }
    }
}