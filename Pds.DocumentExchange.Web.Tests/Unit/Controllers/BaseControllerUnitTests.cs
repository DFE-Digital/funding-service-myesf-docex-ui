using Moq;
using Pds.Core.Utils;
using Pds.Core.Utils.Interfaces;
using Pds.Core.Web.Components.Areas.Lists.Builders;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models.Filters;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Tests.Unit.Controllers
{
    public abstract class BaseControllerUnitTests : BaseControllerTests
    {
        protected IUserInformationProvider UserInformationProvider { get; } = Mock.Of<IUserInformationProvider>(MockBehavior.Strict);

        protected ISystemProvider SystemProvider { get; } = Mock.Of<ISystemProvider>(MockBehavior.Strict);

        protected IDateTimeProvider DateTimeProvider { get; } = Mock.Of<IDateTimeProvider>(MockBehavior.Strict);

        protected IAgencyApiClient AgencyApiClient { get; } = Mock.Of<IAgencyApiClient>(MockBehavior.Strict);

        protected ISupportToolsApiClient SupportToolsApiClient { get; } = Mock.Of<ISupportToolsApiClient>(MockBehavior.Strict);

        protected ISettingsApiClient SettingsApiClient { get; } = Mock.Of<ISettingsApiClient>(MockBehavior.Strict);

        protected IMimeMappingService MimeMappingService { get; } = Mock.Of<IMimeMappingService>(MockBehavior.Strict);

        protected IAgencyDocumentErrorReportBuilder AgencyDocumentErrorReportBuilder { get; } = Mock.Of<IAgencyDocumentErrorReportBuilder>(MockBehavior.Strict);

        protected IRouteValueDictionaryBuilder RouteValuesBuilder { get; } = Mock.Of<IRouteValueDictionaryBuilder>(MockBehavior.Strict);

        protected IMapper Mapper { get; } = Mock.Of<IMapper>(MockBehavior.Strict);

        protected IExchangeDocumentDownloadService ExchangeDocumentDownloadService { get; } = Mock.Of<IExchangeDocumentDownloadService>(MockBehavior.Strict);

        protected IDocumentStatusProvider DocumentStatusProvider { get; } = Mock.Of<IDocumentStatusProvider>(MockBehavior.Strict);

        protected void SetupFilterMapper(List<IFilter> serviceFilters, List<IFilterCategoryViewModel> viewModelFilters)
        {
            var mockMapper = Mock.Get(Mapper);

            for (var i = 0; i < serviceFilters.Count; i++)
            {
                var iLocal = i;

                mockMapper
                   .Setup(m => m.ToFilterCategoryView(serviceFilters[iLocal]))
                   .Returns(viewModelFilters[iLocal]);
            }
        }
    }
}