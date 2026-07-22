using AutoMapper;
using Moq;
using Pds.Core.Common.Identity.Models;
using Pds.Core.Identity.Claims.Interfaces;
using Pds.Core.Logging;
using Pds.Core.Utils;
using Pds.Core.Utils.Interfaces;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Web.Automapper;
using Pds.DocumentExchange.Web.Implementations.Providers;
using System.Security.Claims;

namespace Pds.DocumentExchange.Web.Tests.Integration
{
    public abstract class BaseControllerIntegrationTests : BaseControllerTests
    {
        private readonly IMapper _mapper
            = new Mapper(
                new MapperConfiguration(
                    mapper => mapper.AddProfiles(
                        new Profile[]
                        {
                            new UserMapping(),
                            new ListMapping()
                        })));

        protected IMapper Mapper
            => _mapper;

        protected IDateTimeProvider DateTimeProvider
            => _dateTimeProvider;

        private readonly IClaimsBasedIdentityService _identityService
            = Mock.Of<IClaimsBasedIdentityService>(MockBehavior.Strict);

        protected IClaimsBasedIdentityService IdentityService
            => _identityService;

        private readonly IDateTimeProvider _dateTimeProvider
            = Mock.Of<IDateTimeProvider>(MockBehavior.Strict);

        private readonly ISystemProvider _systemProvider
            = Mock.Of<ISystemProvider>(MockBehavior.Strict);

        private readonly IAgencyApiClient _agencyApiClient
            = Mock.Of<IAgencyApiClient>();

        private readonly ISupportToolsApiClient _supportToolsApiClient
            = Mock.Of<ISupportToolsApiClient>();

        private readonly ISettingsApiClient _settingsApiClient
            = Mock.Of<ISettingsApiClient>();

        protected IAgencyApiClient AgencyApiClient
            => _agencyApiClient;

        protected ISupportToolsApiClient SupportToolsApiClient
          => _supportToolsApiClient;

        protected ISettingsApiClient SettingsApiClient
          => _settingsApiClient;

        protected ISystemProvider SystemProvider
        {
            get
            {
                Mock.Get(_systemProvider)
                    .SetupGet(p => p.DateTime)
                    .Returns(_dateTimeProvider);

                return _systemProvider;
            }
        }

        protected ILoggerAdapter<T> MockLoggerAdapter<T>()
            => Mock.Of<ILoggerAdapter<T>>(MockBehavior.Loose);

        protected UserInformationProvider UserInfoProvider
            => new UserInformationProvider(
                    IdentityService,
                    Mapper,
                    null);

        protected void SetupUserIdentity(User user)
        {
            Mock.Get(IdentityService)
                .Setup(i => i.GetUserFromClaims(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);
        }
    }
}